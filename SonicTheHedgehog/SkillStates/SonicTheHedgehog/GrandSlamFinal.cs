using EntityStates;
using Rewired;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Modules;
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
        protected float procCoefficient = 1.5f;
        protected float basePushForce = 3000f;
        protected float superPushForce = 9000f;
        protected Vector3 bonusForce = Vector3.down;
        protected float attackRecoil = 11f;
        protected float startUpTime = 0.5f;
        protected float maxAttackTime = 0.75f;
        protected float hitStopDuration=0.2f;
        protected float endTime=0.5f;
        protected float baseSpeedMultiplier = 6;

        public HurtBox target;
        protected Vector3 targetDirection;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound;

        private float earlyExitTime;
        public float duration;
        private bool hasFired;
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

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            this.hasFired = false;
            this.hasHit = false;
            this.hitboxName = "Stomp";
            base.PlayAnimation("FullBody, Override", "GrandSlam", "Roll.playbackRate", this.startUpTime*1.15f);
            Util.PlaySound("HenryRoll", base.gameObject);

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);

            PrepareOverlapAttack();
        }

        public override void OnExit()
        {
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            if (!this.hasHit)
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
            Util.PlaySound(this.hitSoundString, base.gameObject);
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
                        base.characterMotor.velocity = Vector3.up*(Mathf.Lerp(60f,3f,fixedAge/this.startUpTime));
                    }
                    else if (fixedAge <= this.startUpTime+this.maxAttackTime)
                    {   
                        if (!hasFired)
                        {
                            hasFired = true;
                        }
                        if (this.target!=null)
                        {
                            targetDirection = (this.target.transform.position - base.characterMotor.transform.position).normalized;
                            speedMultiplier = Mathf.Clamp((this.target.transform.position - base.characterMotor.transform.position).magnitude, 1, baseSpeedMultiplier);
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
                        base.characterMotor.velocity = targetDirection * base.characterBody.moveSpeed*speedMultiplier;
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
            this.attack.pushAwayForce = base.characterBody.HasBuff(Modules.Buffs.superSonicBuff) ? superPushForce : basePushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return hasHit ? InterruptPriority.Skill : InterruptPriority.Frozen;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.target);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            if (reader.ReadHurtBoxReference().ResolveHurtBox() != null)
            {
                this.target = reader.ReadHurtBoxReference().ResolveHurtBox();
            }
            else
            {
                this.target = null;
            }
        }
    }
}