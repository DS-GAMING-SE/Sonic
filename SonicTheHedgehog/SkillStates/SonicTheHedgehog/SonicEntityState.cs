using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;
using EmotesAPI;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicEntityState : GenericCharacterMain
    {
        private float idleExtraTimer;
        private int idleExtraCount;

        private const float idleExtraDefault = 8;

        private HomingTracker homingTracker;

        // WHY AREN'T JUMP ANIMATIONS NETWORKED AGUAHGUESHGUAGHIUSNHGJKSHS
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                homingTracker = GetComponent<HomingTracker>();
                homingTracker.visible = true;
            }
            idleExtraTimer = idleExtraDefault;
            idleExtraCount = 0;
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

        /*public override void Update()
        {
            base.Update();
            if (base.isAuthority && superSonicComponent && base.characterBody.isPlayerControlled && !base.characterBody.HasBuff(Buffs.superSonicBuff)) // Adding isPlayerControlled I guess fixed super transforming all Sonics
            {
                if (Config.SuperTransformKey().Value.IsPressed())
                {
                    if (FormHandler.instance.CanTransform())
                    {
                        Debug.Log("Attempt Super Transform");
                        superSonicComponent.Transform(this.outer);
                    }
                }
            }
        }*/

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            IdleExtraAnimation();
        }

        private void OnHitGround(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            if (base.modelAnimator.GetBool("isBall"))
            {
                base.modelAnimator.SetBool("isBall", false);
            }
        }

        private void IdleExtraAnimation()
        {
            if (base.characterBody.inputBank.moveVector != Vector3.zero || !base.characterMotor.isGrounded ||
                base.characterBody.inputBank.jump.down || base.modelAnimator.GetFloat("isSuperFloat") >= 1)
            {
                idleExtraTimer = idleExtraDefault;
                idleExtraCount = 0;
            }
            else
            {
                idleExtraTimer -= Time.fixedDeltaTime;
                if (idleExtraTimer <= 0)
                {
                    base.PlayAnimation("Body", "IdleExtra");
                    idleExtraCount += 1;
                    idleExtraTimer = idleExtraDefault * (idleExtraCount * 1.5f);
                }
            }
        }
    }
}