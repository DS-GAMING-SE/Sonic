using EntityStates;
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
    public class GrandSlamDash : BaseSkillState
    {  
        protected string hitboxName = "Ball";

        protected DamageType damageType = DamageType.Stun1s | DamageType.NonLethal;
        protected float damageCoefficient = Modules.StaticValues.grandSlamSpinDamageCoefficient;
        protected float procCoefficient = StaticValues.grandSlamSpinProcCoefficient;
        protected float pushForce = 0f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseAttackStartTime = 0.5f;
        protected float attackStartTime;
        protected float attackRecoil = 3f;
        protected float hitHopVelocity=6f;
        protected bool cancelled = false;
        protected float noTargetDistancePercentage = 0.7f;

        protected float maxDashRange;
        protected float dashSpeed;
        protected float estimatedDashTime;
        protected float dashOvershoot;
        protected HurtBox target = null;
        private Vector3 targetDirection;

        protected string chargeSoundString = "Play_spindash_charge";
        protected string launchSoundString = "Play_spindash_release";
        protected string hitSoundString = "Play_homing_impact";
        protected string muzzleString = "SwingCenter";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab = Assets.meleeImpactEffect;
        protected NetworkSoundEventIndex impactSound = Assets.homingHitSoundEvent.index;

        private float earlyExitTime;
        public float duration;
        private bool hasFired;
        private float hitPauseTimer;
        private OverlapAttack attack;
        protected bool inHitPause;
        private bool hasHopped;
        protected float stopwatch;
        protected Animator animator;
        private BaseState.HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;
        private bool hasHit=false;
        private HomingTracker homingTracker;

        public override void OnEnter()
        {
            base.OnEnter();
            this.homingTracker = base.characterBody.GetComponent<HomingTracker>();
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(Modules.Buffs.ballBuff);
            }
            this.dashSpeed = homingTracker.Speed();
            this.hasFired = false;
            this.attackStartTime = baseAttackStartTime / base.characterBody.attackSpeed;
            this.dashOvershoot = 1.5f;
            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            if (homingTracker)
            {
                this.target = homingTracker.GetTrackingTarget();
            }
            this.targetDirection = base.GetAimRay().direction.normalized * homingTracker.MaxRange() * this.noTargetDistancePercentage;
            if (this.target!=null)
            {
                this.targetDirection = (this.target.transform.position - base.transform.position);
            }
            if (dashSpeed > 0)
            {
                this.estimatedDashTime = (targetDirection.magnitude / dashSpeed) * dashOvershoot;
            }
            else
            {
                attackStartTime *= 2;
                this.estimatedDashTime = 0;
            }
            if (base.isAuthority)
            {
                base.characterMotor.Motor.ForceUnground();
            }
            if (this.attackStartTime >= 0.2f)
            {
                Util.PlayAttackSpeedSound(this.chargeSoundString, base.gameObject, base.attackSpeedStat);
            }
            EndChrysalis();
            this.hitboxName = "Ball";
            base.PlayAnimation("FullBody, Override", "Ball");

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);
            base.characterMotor.disableAirControlUntilCollision = false;

            PrepareOverlapAttack();
        }

        public override void OnExit()
        {
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(Modules.Buffs.ballBuff);
            }
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }


        protected virtual void OnHitEnemyAuthority()
        {
            //Util.PlaySound(this.hitSoundString, base.gameObject);
            if (!hasHit)
            {
                hasHit= true;
                EffectManager.SimpleEffect(Assets.homingAttackHitEffect, base.gameObject.transform.position, Util.QuaternionSafeLookRotation(targetDirection), true);
            }    
            SetNextState();
        }

        private void FireAttack()
        {

                if (base.isAuthority)
                {
                    List<HurtBox> hitList = new List<HurtBox>();
                    if (this.attack.Fire(hitList))
                    {
                        base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                        if (this.target==null)
                        {
                            this.target = hitList.FirstOrDefault();
                        }
                        base.characterMotor.velocity=Vector3.zero;
                        this.OnHitEnemyAuthority();
                    }
                }
        }

        protected virtual void SetNextState()
        {
            this.outer.SetNextState(new GrandSlamSpin
            {
                target = this.target
            });
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.stopwatch += Time.fixedDeltaTime;


            if (fixedAge < this.attackStartTime)
            {
                base.characterMotor.velocity = Vector3.Lerp(base.characterMotor.velocity, Vector3.zero, fixedAge / (this.attackStartTime * 0.6f));
            }
            else if (fixedAge < this.estimatedDashTime + this.attackStartTime && fixedAge > this.attackStartTime)
            {
                if (this.target != null)
                {
                    targetDirection = this.target.transform.position - base.transform.position;
                }
                if (!hasFired)
                {
                    hasFired = true;
                    Util.PlaySound(launchSoundString, base.gameObject);
                    if (base.isAuthority)
                    {
                        EffectManager.SimpleEffect(Assets.homingAttackLaunchEffect, base.gameObject.transform.position, Util.QuaternionSafeLookRotation(targetDirection), true);
                    }
                }
                base.characterDirection.forward = targetDirection.normalized;
                if (fixedAge >= this.attackStartTime + (this.estimatedDashTime * 0.75f)) // Slow to stop at end
                {
                    this.dashSpeed = Mathf.Lerp(homingTracker.Speed(), 0, (fixedAge - this.attackStartTime + (this.estimatedDashTime * 0.75f)) / (this.attackStartTime + this.estimatedDashTime));
                }
                if (this.dashSpeed * Time.fixedDeltaTime > targetDirection.magnitude * 3) // Slow when approaching enemy at high speed
                {
                    this.dashSpeed = Mathf.Max(targetDirection.magnitude * 3, 15);
                }
                base.characterDirection.forward = targetDirection.normalized;
                base.characterMotor.velocity = targetDirection.normalized * dashSpeed;
                this.FireAttack();
            }
            if (fixedAge > this.estimatedDashTime + this.attackStartTime && base.isAuthority)
            {
                base.characterMotor.velocity = Vector3.zero;
                this.outer.SetNextStateToMain();
                return;
            }
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

        public void PrepareOverlapAttack()
        {
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
            }

            this.attack = new OverlapAttack();
            this.attack.damageType = this.damageType;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = this.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = this.procCoefficient;
            this.attack.hitEffectPrefab = this.hitEffectPrefab;
            this.attack.forceVector = this.bonusForce;
            this.attack.pushAwayForce = this.pushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}