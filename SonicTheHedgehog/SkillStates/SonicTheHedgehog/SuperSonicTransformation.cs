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
        protected float baseDuration = 2.4f;
        protected float transformationDuration = Modules.StaticValues.superSonicDuration;
        protected bool effectFired = false;
        protected SuperSonicComponent superSonic;
        protected string transformSoundString = "Play_super_transform";

        public float duration;
        protected Animator animator;

        private static float cameraDistance = -7;
        private CharacterCameraParamsData cameraParams = new CharacterCameraParamsData
        {
            maxPitch = 70f,
            minPitch = -70f,
            pivotVerticalOffset = 0.5f,
            idealLocalCameraPos = new Vector3(0f, 0f, cameraDistance),
            wallCushion = 0.1f
        };
        private CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;

        public override void OnEnter()
        {
            base.OnEnter();
            this.superSonic= base.GetComponent<SuperSonicComponent>();
            if (!base.HasBuff(Modules.Buffs.superSonicBuff))
            {
                this.duration = this.baseDuration;
                base.PlayAnimation("FullBody, Override", "Transform", "Roll.playbackRate", this.duration);
                if (NetworkServer.active)
                {
                    base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, duration, 1);
                }

                this.camOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = this.cameraParams,
                    priority = 1f
                }, duration / 2f);
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
            if (fixedAge >= this.duration / 2 && !effectFired && this.superSonic)
            {
                effectFired = true;
                Util.PlaySound(this.transformSoundString, base.gameObject);
                if (base.isAuthority)
                {
                    this.superSonic.superSonicState.SetNextState(new SuperSonic());
                    base.cameraTargetParams.RemoveParamsOverride(this.camOverrideHandle, 0.2f);
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