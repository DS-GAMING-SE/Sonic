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
        private SuperSonicComponent superSonicComponent;

        private float idleExtraTimer;
        private int idleExtraCount;

        private bool emoting = false;

        private const float idleExtraDefault = 8;

        private string jumpSoundString = "Play_jump";

        // WHY AREN'T JUMP ANIMATIONS NETWORKED AGUAHGUESHGUAGHIUSNHGJKSHS
        public override void OnEnter()
        {
            base.OnEnter();
            superSonicComponent = base.GetComponent<SuperSonicComponent>();
            idleExtraTimer = idleExtraDefault;
            idleExtraCount = 0;
            if (base.isGrounded && base.characterBody.isSprinting)
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

            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = true;
            }

            if (SonicTheHedgehogPlugin.emoteAPILoaded)
            {
                EmoteAPI(true);
            }

            if (base.characterMotor)
            {
                base.characterMotor.onHitGroundAuthority += OnHitGround;
            }
        }

        public override void
            ProcessJump() // Why do I have to sync the jump animations myself how is this not a thing by default how has no one noticed they weren't networked
        {
            if (base.isAuthority && this.hasCharacterMotor && this.jumpInputReceived && base.characterBody &&
                base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
            {
                Util.PlaySound(jumpSoundString, base.gameObject);
                base.GetModelAnimator().SetBool("isBall", true);
            }

            base.ProcessJump();
        }

        public override void OnExit()
        {
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
                base.modelAnimator.SetBool("isBall", false);
            }

            if (SonicTheHedgehogPlugin.emoteAPILoaded)
            {
                EmoteAPI(false);
            }

            if (base.characterMotor)
            {
                base.characterMotor.onHitGroundAuthority -= OnHitGround;
            }

            base.OnExit();
        }

        public override void Update()
        {
            base.Update();
            if (base.isAuthority && superSonicComponent &&
                base.characterBody
                    .isPlayerControlled) // Adding isPlayerControlled I guess fixed super transforming all Sonics
            {
                if (Input.GetKeyDown("v"))
                {
                    Inventory inventory = base.characterBody.inventory;
                    if (superSonicComponent.CanTransform(inventory))
                    {
                        Debug.Log("Attempt Super Transform");
                        superSonicComponent.Transform(this.outer, inventory);
                    }
                }
            }
        }

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
                base.characterBody.inputBank.jump.down || emoting)
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

        private void EmoteAPI(bool subscribe)
        {
            if (subscribe)
            {
                CustomEmotesAPI.animChanged += Emoting;
            }
            else
            {
                CustomEmotesAPI.animChanged -= Emoting;
            }
        }

        private void Emoting(String anim, BoneMapper bones)
        {
            emoting = !anim.Equals("none");
        }
    }
}