using EntityStates;
using R2API;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements.Experimental;

namespace SonicTheHedgehog.SkillStates
{
    public class Boost : GenericCharacterMain
    {
        public static float duration = 0.4f;
        public static float extendedDuration = 0.6f;
        public static float initialSpeedCoefficient = 1.3f;
        public static float finalSpeedCoefficient = 1f;
        public static float boostMeterDrain = 0.88f;
        public static float airBoostY = 8;
        public static float screenShake = 3.5f;

        private string jumpSoundString = "Play_jump";
        public static string boostSoundString = "Play_boost";
        public static string boostChangeSoundString = "Play_boost_change";
        public static float dodgeFOV = EntityStates.Commando.DodgeState.dodgeFOV;

        private Vector3 forwardDirection;
        private BoostLogic boostLogic;
        private TemporaryOverlay temporaryOverlay;
        private float boostEffectCooldown;
        private ICharacterFlightParameterProvider flight;

        public bool powerBoosting = false;

        private bool boosting = false;

        private bool checkBoostEffects = false;
        private bool boostChangedEffect = false;

        public override void OnEnter()
        {
            base.OnEnter();
            flight = base.characterBody.GetComponent<ICharacterFlightParameterProvider>();
            base.GetModelAnimator().SetBool("isBoosting", true);
            boostLogic = GetComponent<BoostLogic>();
            if (base.characterBody.healthComponent.health / base.characterBody.healthComponent.fullHealth >= 0.9f && Moving())
            {
                powerBoosting = true;
            }
            OnPowerBoostChanged();
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = true;
            }
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(Modules.Buffs.boostBuff);
                base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.6f * duration);
            }
            if (!base.isGrounded)
            {
                if (base.isAuthority && base.inputBank && base.characterDirection)
                {
                    this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero || Flying()) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
                }

                if (base.characterMotor && base.characterDirection)
                {
                    base.characterMotor.velocity.x = this.forwardDirection.x * base.characterBody.moveSpeed;
                    base.characterMotor.velocity.z = this.forwardDirection.z * base.characterBody.moveSpeed;
                    if (Flying())
                    {
                        base.characterMotor.velocity.y = this.forwardDirection.y * base.characterBody.moveSpeed;
                    }
                    else
                    {
                        base.characterMotor.velocity.y = Mathf.Max(airBoostY, base.characterMotor.velocity.y);
                    }
                    base.PlayCrossfade("Body", "AirBoost", "Roll.playbackRate", duration, duration/3);
                    boosting = true;
                    OnBoostingChanged();
                }
                if (!Flying())
                {
                    base.skillLocator.utility.DeductStock(1);
                }
            }
            else
            {
                if (base.inputBank.moveVector!=Vector3.zero)
                {
                    base.PlayCrossfade("Body", "Boost", 0.1f);
                    boosting = true;
                    OnBoostingChanged();
                }
                else
                {
                    base.PlayCrossfade("Body", "BoostIdle", 0.3f);
                }
            }
            
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();

            boostEffectCooldown -= Time.fixedDeltaTime;

            UpdateBoosting();
            UpdatePowerBoosting();
            if (checkBoostEffects)
            {
                checkBoostEffects = false;
                PlayBoostEffects();
            }

            if (Moving())
            {
                if (!base.HasBuff(Modules.Buffs.superSonicBuff))
                {
                    if (NetworkServer.active)
                    {
                        boostLogic.RemoveBoost(boostMeterDrain);
                    }
                    boostLogic.boostDraining = true;
                }
                else
                {
                    boostLogic.boostDraining = false;
                }
                
                if (base.characterDirection)
                {
                    base.characterBody.isSprinting = true;
                    base.characterDirection.moveVector = this.forwardDirection;
                }
            }
            else
            {
                boostLogic.boostDraining = false;
                if (base.isGrounded)
                {
                    base.characterBody.isSprinting = false;
                }
            }

            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.fovOverride = Mathf.Lerp(dodgeFOV, 60f, base.fixedAge / duration);
            }

            UpdateDirection();

            if (base.isAuthority && base.fixedAge < extendedDuration && !base.isGrounded)
            {
                //base.characterMotor.velocity.y = Mathf.Max(airBoostY, base.characterMotor.velocity.y);
                base.characterMotor.velocity.x = this.forwardDirection.x * base.characterBody.moveSpeed;
                base.characterMotor.velocity.z = this.forwardDirection.z * base.characterBody.moveSpeed;
                if (Flying())
                {
                    base.characterMotor.velocity.y = this.forwardDirection.y * base.characterBody.moveSpeed;
                }
                else
                {
                    base.characterMotor.velocity.y = Mathf.Max(airBoostY, base.characterMotor.velocity.y);
                }
            }
            if (base.isAuthority && base.fixedAge>duration && !base.inputBank.skill3.down)
            {
                this.outer.SetNextStateToMain();
                return;
            }
            if (base.isAuthority && (boostLogic.boostMeter<=0 || !boostLogic.boostAvailable))
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void UpdatePowerBoosting()
        {
            if (!powerBoosting && base.characterBody.healthComponent.health / base.characterBody.healthComponent.fullHealth >= 0.9f && Moving())
            {
                base.characterBody.MarkAllStatsDirty();
                powerBoosting = true;
                OnPowerBoostChanged();
                return;
            }
            if (powerBoosting && (base.characterBody.healthComponent.health / base.characterBody.healthComponent.fullHealth < 0.9f || !Moving()))
            {
                base.characterBody.MarkAllStatsDirty();
                powerBoosting = false;
                OnPowerBoostChanged();
                return;
            }
        }

        private void UpdateBoosting()
        {
            if (!boosting && Moving())
            {
                boosting = true;
                OnBoostingChanged();
                return;
            }
            if (boosting && !Moving())
            {
                boosting = false;
                OnBoostingChanged();
                return;
            }
        }

        private void OnPowerBoostChanged()
        {
            boostLogic.powerBoosting = powerBoosting;
            if (powerBoosting)
            {
                if (boostEffectCooldown <= 0 && base.isAuthority)
                {
                    checkBoostEffects = true;
                }
                else if (temporaryOverlay)
                {
                    temporaryOverlay.AddToCharacerModel(base.GetModelTransform().GetComponent<CharacterModel>());
                }
            }
            else
            {
                if (boostEffectCooldown <= 0 && boosting && base.isAuthority)
                {
                    checkBoostEffects = true;
                }
                else
                {
                    checkBoostEffects = true;
                }
            }
        }

        private void OnBoostingChanged()
        {
            checkBoostEffects = true;
            boostChangedEffect = true;
        }

        private void PlayBoostEffects()
        {
            if (boosting && boostEffectCooldown <= 0)
            {
                if (powerBoosting)
                {
                    boostEffectCooldown = 0.6f;

                    base.AddRecoil(-1f * screenShake, 1f * screenShake, -0.5f * screenShake, 0.5f * screenShake);
                    Util.PlaySound(boostChangedEffect ? Boost.boostSoundString : Boost.boostChangeSoundString, base.gameObject);
                    EffectManager.SimpleMuzzleFlash(Assets.powerBoostFlashEffect, base.gameObject, "BallHitbox", true);

                    if (temporaryOverlay)
                    {
                        EntityState.Destroy(temporaryOverlay);
                    }
                    Transform modelTransform = base.GetModelTransform();
                    if (modelTransform)
                    {
                        temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.duration = 0.2f;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 3f, 0.25f);
                        temporaryOverlay.destroyComponentOnEnd = false;
                        temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matOnHelfire");
                        temporaryOverlay.enabled = true;
                        temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
                    }
                }
                else
                {
                    boostEffectCooldown = 0.6f;

                    base.AddRecoil(-0.5f * screenShake, 0.5f * screenShake, -0.25f * screenShake, 0.25f * screenShake);
                    Util.PlaySound(boostChangedEffect ? Boost.boostSoundString : Boost.boostChangeSoundString, base.gameObject);
                    EffectManager.SimpleMuzzleFlash(Assets.boostFlashEffect, base.gameObject, "BallHitbox", true);
                    if (temporaryOverlay)
                    {
                        temporaryOverlay.RemoveFromCharacterModel();
                    }
                }
            }
            else
            {
                if (temporaryOverlay && !powerBoosting)
                {
                    temporaryOverlay.RemoveFromCharacterModel();
                }
            }
            boostChangedEffect = false;
        }

        private void UpdateDirection()
        {
            if (base.inputBank)
            {
                this.forwardDirection = base.inputBank.moveVector != Vector3.zero ? base.inputBank.moveVector : base.characterDirection.forward;
            }
        }

        public override void ProcessJump()
        {
            if (base.isAuthority && this.hasCharacterMotor && this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
            {
                Util.PlaySound(jumpSoundString, base.gameObject);
            }
            base.ProcessJump();
        }

        public override void OnExit()
        {
            base.GetModelAnimator().SetBool("isBoosting", false);
            boostLogic.boostDraining = false;
            boostLogic.powerBoosting = false;
            boosting = false;
            if (base.cameraTargetParams) base.cameraTargetParams.fovOverride = -1f;
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
            }
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(Modules.Buffs.boostBuff);
            }
            if (temporaryOverlay)
            {
                EntityState.Destroy(temporaryOverlay);
            }
            base.OnExit();
        }

        private bool Flying()
        {
            return flight != null && flight.isFlying;
        }

        private bool Moving()
        {
            return base.characterBody.inputBank.moveVector != Vector3.zero || (!base.isGrounded && (!Flying() || base.fixedAge < extendedDuration));
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.forwardDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.forwardDirection = reader.ReadVector3();
        }
    }
}