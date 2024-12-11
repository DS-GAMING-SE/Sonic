using EntityStates;
using IL.RoR2.ConVar;
using Rewired;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class GrandSlamFinal : BaseSkillState
    {
        protected string hitboxName = "Stomp";

        protected DamageType damageType = DamageType.Generic;
        protected float damageCoefficient = Modules.StaticValues.grandSlamFinalDamageCoefficient;
        protected float procCoefficient = StaticValues.grandSlamFinalProcCoefficient;
        protected virtual float basePushForce
        {
            get { return 3000f; }
        }
        protected Vector3 bonusForce = Vector3.down;
        protected float attackRecoil = 11f;
        protected float startUpTime = 0.5f;
        private float baseMaxAttackTime = 1f;
        protected float maxAttackTime = 0.75f;
        protected float hitStopDuration=0.2f;
        protected float endTime=0.5f;
        protected float baseSpeedMultiplier = 200f;

        public HurtBox target;
        protected Vector3 targetDirection;

        protected string swingSoundString = "";
        protected string hitSoundString = "Play_strong_impact";
        protected string muzzleString = "SwingBottom";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab = Modules.Assets.meleeImpactEffect;
        protected NetworkSoundEventIndex impactSound = Modules.Assets.grandSlamHitSoundEvent.index;

        private float earlyExitTime;
        public float duration;
        protected bool hasFired;
        private bool hasHit;
        private float hitPauseTimer;
        private OverlapAttack attack;
        protected bool inHitPause;
        private bool hasHopped;
        protected float stopwatch;
        protected Animator animator;
        private BaseState.HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;
        private float speedMultiplier;
        private bool animationEnded=false;
        private bool effectFired = false;

        protected float startUpVelocityMax = 60f;
        protected float startUpVelocityMin = 3f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            this.maxAttackTime = this.baseMaxAttackTime;
            this.hasFired = false;
            this.hasHit = false;
            this.hitboxName = "Stomp";
            base.PlayAnimation("FullBody, Override", "GrandSlam", "Roll.playbackRate", this.startUpTime*1.15f);
            Util.PlaySound("Play_swing_low", base.gameObject);

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);

            PrepareOverlapAttack();
        }

        public override void OnExit()
        {
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            if (!this.animationEnded)
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
                base.PlayAnimation("Body", "Backflip");
            }
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }


        protected virtual void OnHitEnemyAuthority()
        {
            if (!effectFired)
            {
                //Util.PlaySound(this.hitSoundString, base.gameObject);
                EffectManager.SimpleMuzzleFlash(Modules.Assets.grandSlamHitEffect, base.gameObject, this.muzzleString, true);
                effectFired = true;
            }
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.up * 9f;
            this.hasHit = true;
            this.stopwatch = 0f;
            if (!this.inHitPause && this.hitStopDuration > 0f)
            {
                this.storedVelocity = base.characterMotor.velocity;
                this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Slash.playbackRate");
                this.hitPauseTimer = this.hitStopDuration / this.attackSpeedStat;
                this.inHitPause = true;
            }
        }

        private void FireAttack()
        {
            if (base.isAuthority)
            {
                if (this.attack.Fire())
                {
                    base.AddRecoil(-2f * this.attackRecoil, -0.5f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                    this.OnHitEnemyAuthority();
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.hitPauseTimer -= Time.fixedDeltaTime;

            if (this.hitPauseTimer <= 0f && this.inHitPause)
            {
                base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                this.inHitPause = false;
                base.characterMotor.velocity = this.storedVelocity;
                animationEnded = true;
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
                base.PlayAnimation("Body", "Backflip");
            }

            if (!this.inHitPause)
            {
                this.stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                if (this.animator) this.animator.SetFloat("Swing.playbackRate", 0f);
            }

            if (base.isAuthority)
            {
                if (!hasHit)
                {
                    if (fixedAge <= this.startUpTime)
                    {
                        base.characterMotor.velocity = GetStartUpVelocity();
                    }
                    else if (fixedAge <= this.startUpTime+this.maxAttackTime)
                    {   
                        if (!hasFired)
                        {
                            hasFired = true;
                            EndChrysalis();
                        }
                        if (this.target!=null)
                        {
                            targetDirection = (this.target.transform.position - base.characterMotor.transform.position).normalized;
                            speedMultiplier = Mathf.Clamp((this.target.transform.position - base.characterMotor.transform.position).magnitude * 10, 25, baseSpeedMultiplier);
                        }
                        else
                        {
                            speedMultiplier = baseSpeedMultiplier;
                            targetDirection = Vector3.down;
                        }
                        if (Vector3.Dot(targetDirection,Vector3.down)<0.3f)
                        {
                            targetDirection = Vector3.down;
                        }
                        base.characterMotor.velocity = targetDirection * speedMultiplier;
                        FireAttack();
                    }
                    else
                    {
                        base.characterMotor.velocity = Vector3.zero;
                        this.outer.SetNextStateToMain();
                        return;
                    }
                    if (base.characterMotor.isGrounded&&this.startUpTime<fixedAge)
                    {
                        base.characterMotor.velocity = Vector3.zero;
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
                else if (this.stopwatch >= this.endTime)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        protected virtual Vector3 GetStartUpVelocity()
        {
            return Vector3.up * (Mathf.Lerp(startUpVelocityMax, startUpVelocityMin, fixedAge / this.startUpTime));
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
            this.attack.damageType.damageSource = DamageSource.Special;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = this.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = this.procCoefficient;
            this.attack.hitEffectPrefab = this.hitEffectPrefab;
            this.attack.forceVector = this.bonusForce;
            this.attack.pushAwayForce = basePushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;
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
            return hasHit ? InterruptPriority.Skill : InterruptPriority.Frozen;
        }
    }
}