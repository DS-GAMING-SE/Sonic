using EntityStates;
using R2API;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Characters;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
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
        public BoostLogic boostLogic;

        private TemporaryOverlayInstance temporaryOverlay;
        //private GameObject trailObject;

        private float boostEffectCooldown;
        private ICharacterFlightParameterProvider flight;

        private static float boostCameraDistance = -13;
        private CharacterCameraParamsData boostingCameraParams = new CharacterCameraParamsData
        {
            maxPitch = 70f,
            minPitch = -70f,
            pivotVerticalOffset = 1.1f,
            idealLocalCameraPos = new Vector3(0f, 0f, boostCameraDistance),
            wallCushion = 0.1f
        };
        private CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;

        public bool powerBoosting = false;

        public bool boosting = false;

        private bool checkBoostEffects = false;
        private bool boostChangedEffect = false;

        protected virtual bool drainBoostMeter
        {
            get { return true; }
        }

        protected virtual BuffDef buff
        {
            get { return Buffs.boostBuff; }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.skillLocator.utility.onSkillChanged += OnSkillChanged;
            flight = base.characterBody.GetComponent<ICharacterFlightParameterProvider>();
            base.GetModelAnimator().SetBool("isBoosting", true);
            if (base.characterMotor)
            {
                base.characterMotor.onHitGroundAuthority += OnHitGround;
            }
            boostLogic = GetComponent<BoostLogic>();
            if (ShouldPowerBoost())
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
                base.characterBody.AddBuff(buff);
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
                    base.PlayCrossfade("Body", "BoostIdleEnter", 0.3f);
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
                if (drainBoostMeter)
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

        protected virtual bool ShouldPowerBoost()
        {
            return base.characterBody.healthComponent.health / base.characterBody.healthComponent.fullHealth >= 0.9f && Moving();
        }

        protected virtual void UpdatePowerBoosting()
        {
            if (!powerBoosting && ShouldPowerBoost())
            {
                base.characterBody.MarkAllStatsDirty();
                powerBoosting = true;
                OnPowerBoostChanged();
                return;
            }
            if (powerBoosting && !ShouldPowerBoost())
            {
                base.characterBody.MarkAllStatsDirty();
                powerBoosting = false;
                OnPowerBoostChanged();
                return;
            }
        }

        protected virtual void UpdateBoosting()
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

        protected void OnPowerBoostChanged()
        {
            boostLogic.powerBoosting = powerBoosting;
            if (powerBoosting)
            {
                if (boostEffectCooldown <= 0 && base.isAuthority)
                {
                    checkBoostEffects = true;
                }
                else
                {
                    CreateTemporaryOverlay();
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

        protected void OnBoostingChanged()
        {
            checkBoostEffects = true;
            boostChangedEffect = true;
        }

        protected virtual void PlayBoostEffects()
        {
            if (boosting && boostEffectCooldown <= 0)
            {
                if (boostChangedEffect)
                {
                    this.camOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                    {
                        cameraParamsData = this.boostingCameraParams,
                        priority = 0f
                    }, 0.1f);
                    ScepterReset();
                }

                boostEffectCooldown = 0.6f;

                if (powerBoosting)
                {
                    Util.PlaySound(boostChangedEffect ? GetSoundString() : Boost.boostChangeSoundString, base.gameObject);
                    if (base.isAuthority)
                    {
                        base.AddRecoil(-1f * screenShake, 1f * screenShake, -0.5f * screenShake, 0.5f * screenShake);
                        EffectManager.SimpleMuzzleFlash(GetEffectPrefab(true), base.gameObject, "BallHitbox", true);
                    }
                    CreateTemporaryOverlay();
                }
                else
                {
                    Util.PlaySound(boostChangedEffect ? GetSoundString() : Boost.boostChangeSoundString, base.gameObject);
                    if (base.isAuthority)
                    {
                        base.AddRecoil(-0.5f * screenShake, 0.5f * screenShake, -0.25f * screenShake, 0.25f * screenShake);
                        EffectManager.SimpleMuzzleFlash(GetEffectPrefab(false), base.gameObject, "BallHitbox", true);
                    }

                    RemoveTemporaryOverlay();
                }
            }
            else
            {
                base.cameraTargetParams.RemoveParamsOverride(this.camOverrideHandle, duration * 2f);
                if (!powerBoosting)
                {
                    RemoveTemporaryOverlay();
                }
            }
            boostChangedEffect = false;
        }

        private void CreateTemporaryOverlay()
        {
            if (temporaryOverlay != null && temporaryOverlay.ValidateOverlay()) { return; }
            Transform modelTransform = base.GetModelTransform();
            if (!modelTransform) { return; }
            CharacterModel model = modelTransform.GetComponent<CharacterModel>();
            if (model)
            {
                temporaryOverlay = TemporaryOverlayManager.AddOverlay(model.gameObject);
                temporaryOverlay.originalMaterial = GetOverlayMaterial();
                temporaryOverlay.animateShaderAlpha = true; // Why does animateShaderAlpha make the entire TemporaryOverlayManager (and by extension the ENTIRE GAME) break when I try to destroy an overlay
                temporaryOverlay.duration = 0.2f;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 2f, 3f, 0.4f);
                temporaryOverlay.destroyComponentOnEnd = false;
                temporaryOverlay.destroyObjectOnEnd = false;
                temporaryOverlay.inspectorCharacterModel = model;
                temporaryOverlay.Start();
            }
        }

        private void RemoveTemporaryOverlay()
        {
            if (temporaryOverlay != null)
            {
                temporaryOverlay.animateShaderAlpha = false; // this is necessary trust me
                temporaryOverlay.Destroy();
            }
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
                base.modelAnimator.SetBool("isBall", true);
            }
            base.ProcessJump();
        }

        private void OnHitGround(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            if (base.modelAnimator.GetBool("isBall"))
            {
                base.modelAnimator.SetBool("isBall", false);
            }
        }

        public override void OnExit()
        {
            base.GetModelAnimator().SetBool("isBoosting", false);
            boostLogic.boostDraining = false;
            boostLogic.powerBoosting = false;
            boosting = false;
            base.cameraTargetParams.RemoveParamsOverride(this.camOverrideHandle, duration * 1.5f);
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
            }
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(buff);
            }

            RemoveTemporaryOverlay();

            base.characterBody.skillLocator.utility.onSkillChanged -= OnSkillChanged;
            base.OnExit();
        }

        protected bool Flying()
        {
            return flight != null && flight.isFlying;
        }

        protected bool Moving()
        {
            return base.characterBody.inputBank.moveVector != Vector3.zero || (!base.isGrounded && (!Flying() || base.fixedAge < extendedDuration));
        }

        public virtual void ScepterDamage()
        {

        }

        public virtual void ScepterReset()
        {

        }

        public virtual string GetSoundString()
        {
            return "Play_boost";
        }

        public virtual GameObject GetEffectPrefab(bool power)
        {
            if (power)
            {
                return Modules.Assets.powerBoostFlashEffect;
            }
            else
            {
                return Modules.Assets.boostFlashEffect;
            }
        }

        public virtual Material GetOverlayMaterial()
        {
            return LegacyResourcesAPI.Load<Material>("Materials/matOnHelfire");
        }

        public virtual void OnSkillChanged(GenericSkill skill)
        {
            if (typeof(Boost).IsAssignableFrom(skill.activationState.stateType))
            {
                outer.SetNextState(EntityStateCatalog.InstantiateState(skill.activationState.stateType));
            }
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