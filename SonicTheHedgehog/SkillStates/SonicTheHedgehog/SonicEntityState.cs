using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicEntityState : GenericCharacterMain
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isGrounded && base.characterBody.isSprinting)
            {
                base.PlayCrossfade("Body", "Sprint", 0.3f);
            }
            else if (base.isGrounded && base.modelAnimator.GetBool("isMoving"))
            {
                base.PlayCrossfade("Body", "Run", 0.1f);
            }
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
            }
        }



        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
        }
    }
}