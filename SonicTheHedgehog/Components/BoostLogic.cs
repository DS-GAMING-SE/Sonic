﻿using RoR2;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.Components
{
    public class BoostLogic : NetworkBehaviour
    {
        // I HATE NETWORKING I HATE NETWORKING I HATE NETWORKING I HATE NETWORKING I HATE NETWORKING I HATE NETWORKING I HATE NETWORKING I HATE NETWORKING I HATE NETWORKING I HATE NETWORKING 
        public CharacterBody body;

        [SyncVar(hook = nameof(OnBoostMeterChanged))]
        public float boostMeter;

        [SyncVar]
        public float maxBoostMeter;

        public float boostRegen;

        [SyncVar(hook = nameof(OnBoostAvailableChanged))]
        public bool boostAvailable = true;

        public float predictedMeter;

        public bool alwaysMaxBoost;

        public bool boostDraining = false;
        public bool powerBoosting = false;

        public bool boostExists;
        public bool scepterBoostExists;

        private const float baseMaxBoostMeter=100f;
        private const float boostMeterPerFlatReduction = 100/3f;
        private const float baseBoostRegen = 0.38f;
        public const float boostRegenPerBandolier = 25f;
        public const float boostRunRechargeCap = 5f;

        public Queue<HealthComponent> recentlyHitHealthComponents = new Queue<HealthComponent>();
        public Queue<float> recentlyHitTimes = new Queue<float>();
        public float scepterTimer = 0;

        private void Start()
        {
            body=GetComponent<CharacterBody>();
            body.characterMotor.onHitGroundAuthority += ResetAirBoost;
            body.skillLocator.utility.onSkillChanged += OnSkillChanged;
            BoostExists();
            CalculateBoostVariables();
            this.NetworkboostMeter = maxBoostMeter;
        }
        
        public void FixedUpdate()
        {
            if (boostExists)
            {
                PredictMeter();
                if (NetworkServer.active)
                {
                    if (this.boostRegen >= Boost.boostMeterDrain || alwaysMaxBoost)
                    {
                        this.AddBoost(maxBoostMeter);
                    }
                    else
                    {
                        this.AddBoost(boostRegen);
                    }
                }

                if (this.scepterBoostExists)
                {
                    this.scepterTimer += Time.fixedDeltaTime;
                    if (this.recentlyHitHealthComponents.Count() > 0)
                    {
                        for (int i = 0; i < this.recentlyHitHealthComponents.Count(); i++)
                        {
                            if (this.scepterTimer >= recentlyHitTimes.Peek())
                            {
                                recentlyHitHealthComponents.Dequeue();
                                recentlyHitTimes.Dequeue();
                            }
                        }

                    }
                }
            }
        }

        public void CalculateBoostVariables()
        {
            if (body && boostExists)
            {
                this.boostRegen = baseBoostRegen / body.skillLocator.utility.cooldownScale;
                this.maxBoostMeter = Mathf.Round((baseMaxBoostMeter + (boostMeterPerFlatReduction * Mathf.Max(body.skillLocator.utility.flatCooldownReduction, -2))) / 10) * 10;
                this.NetworkmaxBoostMeter = maxBoostMeter;
                if ((body.characterMotor.isGrounded || (body.GetComponent<ICharacterFlightParameterProvider>().isFlying)) && body.skillLocator.utility.stock != body.skillLocator.utility.maxStock && boostAvailable)
                {
                    body.skillLocator.utility.stock = body.skillLocator.utility.maxStock;
                }
            }
        }

        public void OnSkillChanged(GenericSkill skill)
        {
            Debug.Log("Change");
            BoostExists();
        }

        private void BoostExists()
        {
            bool prevBoostExists = boostExists;
            boostExists = typeof(Boost).IsAssignableFrom(body.skillLocator.utility.activationState.stateType);
            scepterBoostExists = typeof(ScepterBoost).IsAssignableFrom(body.skillLocator.utility.activationState.stateType);
            if (!prevBoostExists)
            {
                this.NetworkboostAvailable = true;
            }
        }

        private void OnDestroy()
        {
            body.characterMotor.onHitGroundAuthority -= ResetAirBoost;
            body.skillLocator.utility.onSkillChanged -= OnSkillChanged;
        }

        [Server]
        public void AddBoost(float amount)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("AddBoost run on client");
                return;
            }
            this.NetworkboostMeter = Mathf.Clamp(this.boostMeter + amount, 0, this.maxBoostMeter);

            if (!boostAvailable && boostMeter>=Math.Min(baseMaxBoostMeter,this.maxBoostMeter))
            {
                this.NetworkboostAvailable = true;
                //body.skillLocator.utility.stock = body.skillLocator.utility.maxStock;
            }
        }

        [Server]
        public void RemoveBoost(float amount)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("RemoveBoost run on client");
                return;
            }
            this.NetworkboostMeter = Mathf.Clamp(this.boostMeter - amount, 0, this.maxBoostMeter);

            if (boostMeter<=0)
            {
                this.NetworkboostAvailable = false;
                //body.skillLocator.utility.stock = 0;
            }
        }

        public void ResetAirBoost(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            if (boostExists && boostAvailable)
            {
                body.skillLocator.utility.stock = body.skillLocator.utility.maxStock;
            }
        }

        private void OnBoostMeterChanged(float newBoostMeter)
        {
            predictedMeter = newBoostMeter;
            this.NetworkboostMeter = newBoostMeter;
        }

        private void OnBoostAvailableChanged(bool newBoostAvailable)
        {
            if (body)
            {
                if (newBoostAvailable)
                {
                    body.skillLocator.utility.stock = body.skillLocator.utility.maxStock;
                }
                else
                {
                    body.skillLocator.utility.stock = 0;
                }
            }
            this.NetworkboostAvailable = newBoostAvailable;
        }

        private void PredictMeter()
        {
            if (this.boostRegen >= Boost.boostMeterDrain || alwaysMaxBoost)
            {
                predictedMeter = maxBoostMeter;
            }
            else
            {
                predictedMeter = Mathf.Clamp(this.predictedMeter + (boostDraining ? boostRegen-Boost.boostMeterDrain : boostRegen), 0, this.maxBoostMeter);
            }
        }

        public void AddTracker(HealthComponent healthComponent)
        {
            this.recentlyHitHealthComponents.Enqueue(healthComponent);
            this.recentlyHitTimes.Enqueue(this.scepterTimer + StaticValues.scepterBoostICD);
        }

        // I have no clue how to network I am just copying viend and red mist
        // now doing the one thing more dangerous than copying, trying to be original
        public float NetworkboostMeter
        {
            get
            {
                return this.boostMeter;
            }
            [param: In]
            set
            {
                if (NetworkServer.localClientActive && !base.syncVarHookGuard)
                {
                    base.syncVarHookGuard = true;
                    this.OnBoostMeterChanged(value);
                    //NetworkboostMeter = value;
                    base.syncVarHookGuard = false;
                }
                base.SetSyncVar<float>(value, ref this.boostMeter, 1U);
            }
        }

        public float NetworkmaxBoostMeter
        {
            get
            {
                return this.maxBoostMeter;
            }
            [param: In]
            set
            {
                if (NetworkServer.localClientActive && !base.syncVarHookGuard)
                {
                    base.syncVarHookGuard = true;
                    NetworkmaxBoostMeter = value;
                    base.syncVarHookGuard = false;
                }
                base.SetSyncVar<float>(value, ref this.maxBoostMeter, 2U);
            }
        }

        public bool NetworkboostAvailable
        {
            get
            {
                return this.boostAvailable;
            }
            [param: In]
            set
            {
                if (NetworkServer.localClientActive && !base.syncVarHookGuard)
                {
                    base.syncVarHookGuard = true;
                    this.OnBoostAvailableChanged(value);
                    //NetworkboostAvailable = value;
                    base.syncVarHookGuard = false;
                }
                base.SetSyncVar<bool>(value, ref this.boostAvailable, 4U);
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool forceAll)
        {
            if (forceAll)
            {
                writer.Write(this.boostMeter);
                writer.Write(this.maxBoostMeter);
                writer.Write(this.boostAvailable);
                return true;
            }
            bool flag = false;
            if ((base.syncVarDirtyBits & 1U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(this.boostMeter);
            }
            if ((base.syncVarDirtyBits & 2U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(this.maxBoostMeter);
            }
            if ((base.syncVarDirtyBits & 4U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(this.boostAvailable);
            }
            if (!flag)
            {
                writer.WritePackedUInt32(base.syncVarDirtyBits);
            }
            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                this.boostMeter = reader.ReadSingle();
                //this.OnBoostMeterChanged(reader.ReadSingle());
                this.maxBoostMeter = reader.ReadSingle();
                this.boostAvailable = reader.ReadBoolean();
                //this.OnBoostAvailableChanged(reader.ReadBoolean());
                return;
            }
            int num = (int)reader.ReadPackedUInt32();
            if ((num & 1U) != 0U)
            {
                //this.boostMeter = reader.ReadSingle();
                this.OnBoostMeterChanged(reader.ReadSingle());
            }
            if ((num & 2U) != 0U)
            {
                this.maxBoostMeter = reader.ReadSingle();
            }
            if ((num & 4U) != 0U)
            {
                //this.boostAvailable = reader.ReadBoolean();
                this.OnBoostAvailableChanged(reader.ReadBoolean());
            }
        }
        private void UNetVersion()
        {
        }
    }
}