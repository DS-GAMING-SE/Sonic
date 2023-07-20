using EntityStates;
using R2API;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.SkillStates;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SonicTheHedgehog.Components
{
    public class BoostLogic : NetworkBehaviour
    {
        private CharacterMotor.HitGroundInfo hitGroundInfo;
        public CharacterBody body;
        public GameObject boostUI;
        public Slider boostUIFill;
        public float boostMeter;
        public float maxBoostMeter;
        public float boostRegen;
        public bool boostAvailable;
        private const float baseMaxBoostMeter=100f;
        private const float boostMeterPerFlatReduction = 10f;
        private const float baseBoostRegen = 0.38f;

        private void Start()
        {
            body=GetComponent<CharacterBody>();
            body.characterMotor.onHitGroundAuthority += ResetAirBoost;
            CalculateBoostVariables();
            this.NetworkmaxBoostMeter = maxBoostMeter;
            this.NetworkboostMeter = maxBoostMeter;
            this.boostAvailable = true;
        }
        
        public void FixedUpdate()
        {
            //this.maxBoostMeter = baseMaxBoostMeter + (boostMeterPerFlatReduction * body.skillLocator.utility.flatCooldownReduction);
            //this.boostRegen = baseBoostRegen / body.skillLocator.utility.cooldownScale;
            if (NetworkServer.active)
            {
                this.AddBoost(boostRegen);
            }
        }

        public void CalculateBoostVariables()
        {
            this.boostRegen = baseBoostRegen / body.skillLocator.utility.cooldownScale;
            this.maxBoostMeter = baseMaxBoostMeter + (boostMeterPerFlatReduction * body.skillLocator.utility.flatCooldownReduction);
            this.NetworkmaxBoostMeter = maxBoostMeter;
            if (body.characterMotor.isGrounded && body.skillLocator.utility.stock!=body.skillLocator.utility.maxStock && boostAvailable)
            {
                body.skillLocator.utility.stock = body.skillLocator.utility.maxStock;
            }
        }


        private void OnDestroy()
        {
            body.characterMotor.onHitGroundAuthority -= ResetAirBoost;
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

            if (!boostAvailable&&boostMeter>=Math.Min(baseMaxBoostMeter,maxBoostMeter))
            {
                this.boostAvailable = true;
                body.skillLocator.utility.stock = body.skillLocator.utility.maxStock;
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
                this.boostAvailable = false;
                body.skillLocator.utility.stock = 0;
            }
        }

        public void ResetAirBoost(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            if (boostAvailable)
            {
                body.skillLocator.utility.stock = body.skillLocator.utility.maxStock;
            }
            this.hitGroundInfo = hitGroundInfo;
        }



        // I have no fucking clue how to network I am just copying viend and red mist
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
                    NetworkboostMeter = value;
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

        public override bool OnSerialize(NetworkWriter writer, bool forceAll)
        {
            if (forceAll)
            {
                writer.Write(this.boostMeter);
                writer.Write(this.maxBoostMeter);
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
                this.maxBoostMeter = reader.ReadSingle();
                return;
            }
            int num = (int)reader.ReadPackedUInt32();
            if ((num & 1U) != 0U)
            {
                this.boostMeter = reader.ReadSingle();
            }
            if ((num & 2U) != 0U)
            {
                this.maxBoostMeter = reader.ReadSingle();
            }
        }
    }
}