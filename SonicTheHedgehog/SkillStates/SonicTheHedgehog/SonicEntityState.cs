using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using HedgehogUtils.Emotes;
using SonicTheHedgehog.SkillStates.Emotes;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicEntityState : GenericMainWithIdleEmote
    {
        private HomingTracker homingTracker;
        protected EntityStateMachine weaponStateMachine;

        // WHY AREN'T JUMP ANIMATIONS NETWORKED AGUAHGUESHGUAGHIUSNHGJKSHS
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                homingTracker = GetComponent<HomingTracker>();
                homingTracker.visible = true;

                weaponStateMachine = EntityStateMachine.FindByCustomName(gameObject, "Weapon");
            }
            if (base.modelAnimator.isInitialized)
            {
                if (base.isGrounded && base.characterBody.isSprinting && base.inputBank.moveVector != Vector3.zero)
                {
                    base.PlayCrossfade("Body", "Sprint", 0.3f);
                }
                else if (base.isGrounded)
                {
                    if (base.modelAnimator.GetBool("isMoving"))
                    {
                        base.PlayCrossfade("Body", "Run", 0.1f);
                    }
                    else
                    {
                        base.PlayCrossfade("Body", "Idle", 0.3f);
                    }
                }
                else if (base.modelAnimator.GetBool("isBall"))
                {
                    base.PlayAnimation("Body", "Ball");
                }
                else
                {
                    base.PlayCrossfade("Body", "AscendDescend", 0.3f);
                }
            }

            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = true;
            }

            if (base.characterMotor)
            {
                base.characterMotor.onHitGroundAuthority += OnHitGround;
            }
        }

        public override void SetNextStateToIdleExtra()
        {
            if (weaponStateMachine) weaponStateMachine.SetNextState(new IdleEmote());
        }

        public override bool ShouldInterrupt()
        {
            return HedgehogUtils.Helpers.IsDoingSomething(characterMotor, inputBank, true, modelAnimator.GetFloatString("isSuperFloat") == 1f, false, false) || !weaponStateMachine.IsInMainState();
        }

        public override void ProcessJump() // Why do I have to sync the jump animations myself how is this not a thing by default how has no one noticed they weren't networked
        {
            if (base.isAuthority && this.hasCharacterMotor && this.jumpInputReceived && base.characterBody &&
                base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
            {
                base.GetModelAnimator().SetBool("isBall", true);
            }

            base.ProcessJump();
        }

        public override void OnExit()
        {
            if (base.isAuthority)
            {
                homingTracker.visible = false;
            }
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
            }
            if (base.modelAnimator)
            {
                base.modelAnimator.SetBool("isBall", false);
            }

            if (base.characterMotor)
            {
                base.characterMotor.onHitGroundAuthority -= OnHitGround;
            }

            base.OnExit();
        }

        private void OnHitGround(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            if (base.modelAnimator.GetBool("isBall"))
            {
                base.modelAnimator.SetBool("isBall", false);
            }
        }
    }
}