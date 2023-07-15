using EntityStates;
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
        public static float screenShake = 2f;

        public static string dodgeSoundString = "HenryRoll";
        public static float dodgeFOV = EntityStates.Commando.DodgeState.dodgeFOV;

        private Vector3 forwardDirection;
        private BoostLogic boostLogic;


        public override void OnEnter()
        {
            base.OnEnter();
            base.GetModelAnimator().SetBool("isBoosting", true);
            boostLogic = GetComponent<BoostLogic>();
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
                    //base.characterMotor.rootMotion.x = this.forwardDirection.x * base.characterBody.moveSpeed*Time.fixedDeltaTime;
                    base.characterMotor.velocity.x = this.forwardDirection.x * base.characterBody.moveSpeed;
                    base.characterMotor.velocity.z = this.forwardDirection.z * base.characterBody.moveSpeed;
                    base.characterMotor.velocity.y = Mathf.Max(airBoostY, base.characterMotor.velocity.y);
                    //base.characterMotor.rootMotion.z = this.forwardDirection.z * base.characterBody.moveSpeed*Time.fixedDeltaTime;
                    base.PlayCrossfade("Body", "AirBoost", duration);
                    base.AddRecoil(-1f * screenShake, 1f * screenShake, -0.5f * screenShake, 0.5f * screenShake);
                }
                base.skillLocator.utility.DeductStock(1);
                //base.PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", Boost.duration);
                Util.PlaySound(Boost.dodgeSoundString, base.gameObject);
            }
            else
            {
                if (base.inputBank.moveVector!=Vector3.zero)
                {
                    base.PlayCrossfade("Body", "Boost", 0.1f);
                    base.AddRecoil(-1f * screenShake, 1f * screenShake, -0.5f * screenShake, 0.5f * screenShake);
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

            ProcessJump();

            if (!base.HasBuff(Modules.Buffs.superSonicBuff) && base.characterBody.inputBank.moveVector!=Vector3.zero && NetworkServer.active)
            {
                boostLogic.RemoveBoost(boostMeterDrain);
            }
            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.fovOverride = Mathf.Lerp(dodgeFOV, 60f, base.fixedAge / duration);
            }

            UpdateDirection();

            if (base.characterDirection&&base.inputBank.moveVector!=Vector3.zero)
            {
                base.characterBody.isSprinting = true;
                base.characterDirection.moveVector = this.forwardDirection;
                //if (base.characterMotor && !base.characterMotor.disableAirControlUntilCollision)
                //{
                //    base.characterMotor.rootMotion += base.characterDirection.forward*base.characterBody.moveSpeed * Time.fixedDeltaTime;
                //}
            }

            if (base.isGrounded && base.inputBank.moveVector == Vector3.zero)
            {
                base.characterBody.isSprinting = false;
                //if (base.inputBank.moveVector == Vector3.zero)
                //{
                //    base.characterBody.isSprinting = false;
                    //base.PlayCrossfade("Body", "BoostIdle", 0.3f);
                //}
                //else
                //{
                    //base.PlayCrossfade("Body", "Boost", 0.3f);
                //}
                //base.characterBody.isSprinting = false;
            }

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
            //if (base.characterMotor&&!base.characterMotor.disableAirControlUntilCollision && base.inputBank.moveVector != Vector3.zero)
            //{
            //    base.characterMotor.velocity.x = base.characterDirection.forward.x * base.characterBody.moveSpeed;
            //    base.characterMotor.velocity.z = base.characterDirection.forward.z * base.characterBody.moveSpeed;
            //}
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(Modules.Buffs.boostBuff);
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