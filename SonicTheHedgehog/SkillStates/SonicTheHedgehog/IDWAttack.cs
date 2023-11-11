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
    public class IDWAttack : BaseSkillState
    {  
        protected static DamageType damageType = DamageType.Stun1s;
        protected static float damageCoefficient = Modules.StaticValues.idwAttackDamageCoefficient;
        protected static float procCoefficient = StaticValues.idwAttackProcCoefficient;
        protected static float attackDuration = 2f;
        protected static int baseAttackCount = 10;
        protected float pushForce = 0f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseSearchTime = 0.65f;
        protected float searchTime;
        protected float attackRecoil = 3f;
        protected float hitHopVelocity=6f;
        protected bool cancelled = false;

        protected HurtBox target = null;
        protected bool targetLocked = false;

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
        private int maxAttackCount;
        private int attackCount;

        private HomingTracker homingTracker;
        private SuperSonicComponent superSonicComponent;

        private Transform modelTransform;
        private CharacterModel characterModel;

        public override void OnEnter()
        {
            base.OnEnter();
            this.homingTracker = base.characterBody.GetComponent<HomingTracker>();
            this.superSonicComponent = base.characterBody.GetComponent<SuperSonicComponent>();
            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
            }
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.Intangible);
            }
            this.hasFired = false;
            this.searchTime = baseSearchTime / base.characterBody.attackSpeed;
            if (homingTracker)
            {
                this.target = homingTracker.GetTrackingTarget();
                if (target!=null)
                {
                    TargetLocked();
                }
            }
            if (base.isAuthority)
            {
                base.characterMotor.Motor.ForceUnground();
            }
            base.PlayAnimation("FullBody, Override", "Ball");

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);
            base.characterMotor.disableAirControlUntilCollision = false;
            base.characterBody.SetAimTimer(this.searchTime * 2);

            PrepareOverlapAttack();
        }

        public override void OnExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
            }
            if (this.hasFired)
            {
                if (this.characterModel)
                {
                    this.characterModel.invisibilityCount--;
                }
            }
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }


        protected virtual void OnHitEnemyAuthority()
        {

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

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.stopwatch += Time.fixedDeltaTime;

            if (this.fixedAge <= this.searchTime)
            {
                base.characterMotor.velocity = Vector3.Lerp(base.characterMotor.velocity, Vector3.zero, fixedAge / (this.searchTime * 0.8f));
                if (this.target != null && !this.targetLocked)
                {
                    TargetLocked();
                }
            }
            if (this.fixedAge > this.searchTime && this.fixedAge <= attackDuration + this.searchTime && base.isAuthority && this.targetLocked)
            {
                if (!this.hasFired)
                {
                    hasFired = true;
                    if (this.characterModel)
                    {
                        this.characterModel.invisibilityCount++;
                    }
                    this.maxAttackCount = (int)Math.Ceiling(base.characterBody.attackSpeed * baseAttackCount);
                    base.characterBody.SetAimTimer(attackDuration * 1.5f);
                }
                if (this.fixedAge > (attackDuration / this.maxAttackCount * (this.attackCount + 1)) + this.searchTime)
                {
                    this.attackCount++;
                }
            }
            if (base.isAuthority && (this.fixedAge > this.searchTime && !targetLocked) || (this.fixedAge > attackDuration + this.searchTime))
            {
                base.characterMotor.velocity = Vector3.zero;
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void TargetLocked()
        {
            this.targetLocked = true;
            if (superSonicComponent)
            {
                superSonicComponent.IDWAttackActivated();
            }
        }

        public void PrepareOverlapAttack()
        {
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                //hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
            }

            this.attack = new OverlapAttack();
            this.attack.damageType = damageType;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = damageCoefficient * this.damageStat;
            this.attack.procCoefficient = procCoefficient;
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
    }
}