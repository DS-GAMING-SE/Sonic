using EntityStates;
using R2API;
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

namespace SonicTheHedgehog.SkillStates.Cyloop
{
    public class QuickCyloopDash : BaseSkillState
    {
        protected float attackStartTime;
        protected float attackEndTime;
        protected bool cancelled = false;

        protected float maxHomingAttackRange;
        protected float homingAttackSpeed;
        protected float estimatedHomingAttackTime;
        protected float homingAttackOvershoot;
        public HurtBox target;
        private Vector3 targetDirection;
        protected bool homingAttackHit;

        protected string homingAttackSoundString = "Play_sonicthehedgehog_homing_attack";

        public float duration;
        protected Animator animator;
        private HomingTracker homingTracker;

        public override void OnEnter()
        {
            base.OnEnter();
            this.homingTracker = base.characterBody.GetComponent<HomingTracker>();
            this.target = homingTracker.GetTrackingTarget(true);
            this.maxHomingAttackRange = homingTracker.MaxRange();
            this.homingAttackSpeed = homingTracker.Speed();
            this.homingAttackOvershoot = 1.65f;
            EntityStateMachine.FindByCustomName(gameObject, "Weapon").SetNextStateToMain();

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(Modules.Buffs.ballBuff);
            }
            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            this.targetDirection = Vector3.zero;
            if (this.target != null)
            {
                this.targetDirection = (this.target.transform.position - base.transform.position);
            }
            Util.PlaySound(homingAttackSoundString, base.gameObject);
            this.estimatedHomingAttackTime = (targetDirection.magnitude / homingAttackSpeed) * homingAttackOvershoot;
            if (base.isAuthority)
            {
                homingTracker.visible = true;
                homingTracker.locked = true;
                base.characterMotor.Motor.ForceUnground();
                if (targetDirection != Vector3.zero)
                {
                    EffectManager.SimpleEffect(Modules.Assets.homingAttackLaunchEffect, base.gameObject.transform.position, Util.QuaternionSafeLookRotation(targetDirection), true);
                }
            }
            EndChrysalis();
            base.PlayAnimation("FullBody, Override", "Ball");
            base.characterMotor.disableAirControlUntilCollision = false;

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
        }

        public override void OnExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (base.characterBody.HasBuff(Modules.Buffs.ballBuff))
            {
                if (NetworkServer.active)
                {
                    base.characterBody.RemoveBuff(Modules.Buffs.ballBuff);
                }
            }
            if (base.characterBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.IgnoreFallDamage))
            {
                base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }

            if (base.isAuthority)
            {
                homingTracker.locked = false;
                homingTracker.visible = false;
            }
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (fixedAge < this.estimatedHomingAttackTime)
                {
                    this.homingAttackSpeed = homingTracker.Speed();
                    if (this.target != null)
                    {
                        targetDirection = this.target.transform.position - base.characterMotor.transform.position;
                        //this.homingAttackSpeed = Mathf.Min(homingAttackSpeed, targetDirection.magnitude * 3f); Don't overshoot somehow
                    }
                    base.characterMotor.velocity = targetDirection.normalized * this.homingAttackSpeed;
                    base.characterDirection.forward = targetDirection.normalized;
                    if (base.isGrounded)
                    {
                        base.characterMotor.Motor.ForceUnground();
                    }
                    if (target && target.healthComponent && targetDirection.magnitude <= target.healthComponent.body.radius + StaticValues.quickCyloopExtraRadius + 0.5f)
                    {
                        SetNextState();
                    }
                }
                else
                {
                    base.characterMotor.velocity = Vector3.zero;
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        protected virtual void SetNextState()
        {
            this.outer.SetNextState(new QuickCyloop { target = this.target });
        }

        private void EndChrysalis()
        {
            JetpackController chrysalis = JetpackController.FindJetpackController(base.gameObject);
            if (chrysalis)
            {
                if (chrysalis.stopwatch >= chrysalis.duration && NetworkServer.active)
                {
                    UnityEngine.Object.Destroy(chrysalis.gameObject);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}