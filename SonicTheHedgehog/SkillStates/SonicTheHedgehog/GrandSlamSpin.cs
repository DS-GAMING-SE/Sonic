using EntityStates;
using R2API.Networking.Interfaces;
using Rewired;
using RoR2;
using RoR2.Audio;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class GrandSlamSpin : BaseSkillState
    {
        protected string hitboxName = "Ball";

        protected DamageType damageType = DamageType.Stun1s;
        protected float damageCoefficient = Modules.StaticValues.grandSlamDashDamageCoefficient;
        protected float procCoefficient = 0.5f;
        protected float pushForce = 0f;
        protected Vector3 bonusForce = Vector3.up;
        protected int baseAttackCount=4;
        protected int maxAttackCount;
        protected int attackCount;
        protected float attackRecoil = 3f;
        protected float attackDuration=0.75f;

        public HurtBox target;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound;

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

        public override void OnEnter()
        {
            base.OnEnter();
            this.hasFired = false;
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            this.maxAttackCount = (int) Math.Ceiling(base.characterBody.attackSpeed*baseAttackCount);
            this.hitboxName = "LargeBall";
            //base.PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", this.attackDuration);
            Util.PlaySound("HenryRoll", base.gameObject);

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);
        }

        public override void OnExit()
        {
            base.OnExit();
            this.animator.SetBool("attacking", false);
        }


        protected virtual void OnHitEnemyAuthority()
        {
             Util.PlaySound(this.hitSoundString, base.gameObject);
        }

        private void FireAttack()
        {
                if (base.isAuthority)
                {
                    this.attackCount++;
                    if (this.attack.Fire())
                    {
                        base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                        this.OnHitEnemyAuthority();
                    }
                }
        }

        protected virtual void SetNextState()
        {
            this.outer.SetNextState(new GrandSlamFinal
            {
                target = this.target
            });
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.stopwatch += Time.fixedDeltaTime;

            if (base.isAuthority)
            {
                if (fixedAge <= this.attackDuration||attackCount<maxAttackCount)
                {
                    base.characterMotor.velocity = Vector3.zero;
                    if (fixedAge>=(this.attackDuration/this.maxAttackCount)*(attackCount+1))
                    {
                        PrepareOverlapAttack();
                        this.FireAttack();
                    }
                }
                else
                {
                    SetNextState();
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
            return InterruptPriority.Frozen;
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