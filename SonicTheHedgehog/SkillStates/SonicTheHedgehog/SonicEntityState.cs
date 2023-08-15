using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;
using EmotesAPI;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicEntityState : GenericCharacterMain
    {
        private float idleExtraTimer;
        private int idleExtraCount;
        private bool emoting = false;
        private const float idleExtraDefault=8;
        // WHY AREN'T JUMP ANIMATIONS NETWORKED AGUAHGUESHGUAGHIUSNHGJKSHS
        public override void OnEnter()
        {
            base.OnEnter();
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
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = true;
            }
            if (SonicTheHedgehogPlugin.emoteAPILoaded)
            {
                EmoteAPI(true);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
            }
            if (SonicTheHedgehogPlugin.emoteAPILoaded)
            {
                EmoteAPI(false);
            }
        }



        public override void FixedUpdate()
        {
            base.FixedUpdate();
            IdleExtraAnimation();
        }

        private void IdleExtraAnimation()
        {
            if (base.characterBody.inputBank.moveVector!=Vector3.zero || !base.characterMotor.isGrounded || base.characterBody.inputBank.jump.down || emoting)
            {
                idleExtraTimer = idleExtraDefault;
                idleExtraCount = 0;
            }
            else
            {
                idleExtraTimer -= Time.fixedDeltaTime;
                if (idleExtraTimer<=0)
                {
                    base.PlayAnimation("Body", "IdleExtra");
                    idleExtraCount += 1;
                    idleExtraTimer = idleExtraDefault*(idleExtraCount*1.5f);
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