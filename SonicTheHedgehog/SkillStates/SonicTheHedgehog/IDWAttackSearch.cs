using EntityStates;
using HedgehogUtils.Forms.SuperForm;
using HedgehogUtils.Forms;
using Rewired;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class IDWAttackSearch : BaseSkillState
    {  
        protected static float baseSearchTime = 1f;
        protected float searchTime;

        protected HurtBox target = null;
        protected bool targetLocked = false;
        protected Vector3 targetPosition = Vector3.zero;

        protected string chargeSoundString = "Play_sonicthehedgehog_spindash_charge";
        protected string launchSoundString = "Play_sonicthehedgehog_spindash_release";
        protected string hitSoundString = "Play_sonicthehedgehog_homing_impact";
        protected string muzzleString = "SwingCenter";

        public float duration;
        protected bool inHitPause;
        protected float stopwatch;
        protected Animator animator;

        private HomingTracker homingTracker;
        private SuperSonicComponent superSonicComponent;

        public override void OnEnter()
        {
            base.OnEnter();
            this.homingTracker = base.characterBody.GetComponent<HomingTracker>();
            this.superSonicComponent = base.characterBody.GetComponent<SuperSonicComponent>();
            this.searchTime = baseSearchTime / base.characterBody.attackSpeed;
            SearchForTarget();
            if (base.isAuthority)
            {
                base.characterMotor.Motor.ForceUnground();
            }
            base.PlayAnimation("FullBody, Override", "IDWStart", "Slash.playbackRate", searchTime);

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);
            base.characterMotor.disableAirControlUntilCollision = false;
            Util.PlaySound("Play_sonicthehedgehog_swing", base.gameObject);
            base.StartAimMode(this.searchTime, false);
        }

        public override void OnExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.stopwatch += Time.fixedDeltaTime;

            if (!Forms.GetIsInForm(base.characterBody, SuperFormDef.superFormDef))
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (this.fixedAge <= this.searchTime) //search
            {
                base.characterMotor.velocity = Vector3.Lerp(base.characterMotor.velocity, Vector3.zero, fixedAge / (this.searchTime * 0.8f));
                if (this.target == null && !this.targetLocked)
                {
                    SearchForTarget();
                }
            }
            else
            {
                if (this.targetLocked)
                {
                    this.outer.SetNextState(new IDWAttack { target = target });
                }
                else
                {
                    if (base.isAuthority && (this.fixedAge > this.searchTime))
                    {
                        base.characterMotor.velocity = Vector3.zero;
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
            }
        }

        private void TargetLocked()
        {
            this.targetLocked = true;
            if (base.isAuthority)
            {
                EntityStateMachine superStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "SonicForms");
                if (superStateMachine)
                {
                    /*if (superStateMachine.state.GetType() == typeof(SuperSonic))
                    {
                        ((SuperSonic)superStateMachine.state).IDWAttackActivated();
                    }*/
                }
            }
        }

        private void SearchForTarget()
        {
            if (homingTracker)
            {
                this.target = homingTracker.GetTrackingTarget();
                if (this.target != null)
                {
                    TargetLocked();
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}