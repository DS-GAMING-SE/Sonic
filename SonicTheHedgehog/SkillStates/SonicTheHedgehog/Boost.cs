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
        public static float screenShake = 2.5f;

        public static string dodgeSoundString = "HenryRoll";
        public static float dodgeFOV = EntityStates.Commando.DodgeState.dodgeFOV;

        private Vector3 forwardDirection;
        private BoostLogic boostLogic;
        private TemporaryOverlay temporaryOverlay;
        private float boostEffectCooldown;

        public bool powerBoosting = false;


        public override void OnEnter()
        {
            base.OnEnter();
            base.GetModelAnimator().SetBool("isBoosting", true);
            boostLogic = GetComponent<BoostLogic>();
            if (base.characterBody.healthComponent.health / base.characterBody.healthComponent.fullHealth >= 0.9f && base.characterBody.inputBank.moveVector != Vector3.zero)
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
                    this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
                }

                if (base.characterMotor && base.characterDirection)
                {
                    base.characterMotor.velocity.x = this.forwardDirection.x * base.characterBody.moveSpeed;
                    base.characterMotor.velocity.z = this.forwardDirection.z * base.characterBody.moveSpeed;
                    base.characterMotor.velocity.y = Mathf.Max(airBoostY, base.characterMotor.velocity.y);
                    base.PlayCrossfade("Body", "AirBoost", "Roll.playbackRate", duration, duration/3);
                }
                base.skillLocator.utility.DeductStock(1);
                Util.PlaySound(Boost.dodgeSoundString, base.gameObject);
            }
            else
            {
                if (base.inputBank.moveVector!=Vector3.zero)
                {
                    base.PlayCrossfade("Body", "Boost", 0.1f);
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

            UpdatePowerBoosting();

            ProcessJump();

            if (base.characterBody.inputBank.moveVector!=Vector3.zero)
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
            else if (base.isGrounded)
            {
                base.characterBody.isSprinting = false;
            }

            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.fovOverride = Mathf.Lerp(dodgeFOV, 60f, base.fixedAge / duration);
            }

            UpdateDirection();

            if (base.isAuthority&&((base.fixedAge < duration)||(base.fixedAge<extendedDuration&&base.inputBank.skill3.down))&&!base.isGrounded)
            {
                //base.characterMotor.velocity.y = Mathf.Max(airBoostY, base.characterMotor.velocity.y);
                base.characterMotor.velocity.x = this.forwardDirection.x * base.characterBody.moveSpeed;
                base.characterMotor.velocity.z = this.forwardDirection.z * base.characterBody.moveSpeed;
                base.characterMotor.velocity.y = Mathf.Max(airBoostY, base.characterMotor.velocity.y);
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
            if (!powerBoosting && base.characterBody.healthComponent.health / base.characterBody.healthComponent.fullHealth >= 0.9f && base.characterBody.inputBank.moveVector != Vector3.zero)
            {
                base.characterBody.RecalculateStats();
                powerBoosting = true;
                OnPowerBoostChanged();
                return;
            }
            if (powerBoosting && (base.characterBody.healthComponent.health / base.characterBody.healthComponent.fullHealth < 0.9f || base.characterBody.inputBank.moveVector == Vector3.zero))
            {
                base.characterBody.RecalculateStats();
                powerBoosting = false;
                OnPowerBoostChanged();
                return;
            }
        }

        private void OnPowerBoostChanged()
        {
            if (powerBoosting)
            {
                if (boostEffectCooldown <= 0)
                {
                    boostEffectCooldown = 0.6f;
                    base.AddRecoil(-1f * screenShake, 1f * screenShake, -0.5f * screenShake, 0.5f * screenShake);
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
                else if (temporaryOverlay)
                {
                    temporaryOverlay.AddToCharacerModel(base.GetModelTransform().GetComponent<CharacterModel>());
                }
            }
            else
            {
                if (temporaryOverlay)
                {
                    temporaryOverlay.RemoveFromCharacterModel();
                }
            }
        }

        private void UpdateDirection()
        {
            if (base.inputBank)
            {
                Vector2 vector = Util.Vector3XZToVector2XY(base.inputBank.moveVector);
                if (vector != Vector2.zero)
                {
                    vector.Normalize();
                    this.forwardDirection = new Vector3(vector.x, 0f, vector.y).normalized;
                }
            }
        }

        public override void OnExit()
        {
            base.GetModelAnimator().SetBool("isBoosting", false);
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