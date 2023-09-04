using EntityStates;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Components;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class SuperSonicTransformation : BaseSkillState
    {
        protected float baseDuration = 2f;
        protected float transformationDuration = Modules.StaticValues.superSonicDuration;
        protected bool effectFired = false;
        protected SuperSonicComponent superSonic;
        protected string transformSoundString = "Play_super_transform";

        public float duration;
        protected Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.superSonic= base.GetComponent<SuperSonicComponent>();
            if (!base.HasBuff(Modules.Buffs.superSonicBuff))
            {
                this.duration = this.baseDuration;
                if (NetworkServer.active)
                {
                    base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, duration, 1);
                }
                //base.PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", baseDuration);
            }
            else
            {
                effectFired = true;
                this.outer.SetNextStateToMain();
                return;
            }

        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.velocity = Vector3.zero;
            if (fixedAge >= this.duration/2 && !effectFired && this.superSonic)
            {
                effectFired = true;
                Util.PlaySound(this.transformSoundString, base.gameObject);
                if (base.isAuthority)
                {
                    this.superSonic.superSonicState.SetNextState(new SuperSonic());
                }
            }
           
            
            if (fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Vehicle;
        }
    }
}