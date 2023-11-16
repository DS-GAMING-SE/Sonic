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
        protected static DamageType damageType = DamageType.Stun1s | DamageType.AOE;
        protected static float damageCoefficient = Modules.StaticValues.idwAttackDamageCoefficient;
        protected static float procCoefficient = StaticValues.idwAttackProcCoefficient;
        protected static float attackDuration = 2f;
        protected static float range = 20;
        protected static int baseAttackCount = 10;
        protected float pushForce = 400f;
        protected static float baseSearchTime = 1f;
        protected static float baseEndLag = 1.2f;
        protected float endLag;
        protected float searchTime;

        protected HurtBox target = null;
        protected bool targetLocked = false;
        protected Vector3 targetPosition = Vector3.zero;

        protected string chargeSoundString = "Play_spindash_charge";
        protected string launchSoundString = "Play_spindash_release";
        protected string hitSoundString = "Play_homing_impact";
        protected string muzzleString = "SwingCenter";

        public float duration;
        private bool hasFired;
        protected bool inHitPause;
        protected float stopwatch;
        protected Animator animator;
        private int maxAttackCount;
        private int attackCount;
        private bool invisible;

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
            this.hasFired = false;
            this.searchTime = baseSearchTime / base.characterBody.attackSpeed;
            this.endLag = baseEndLag;
            SearchForTarget();
            if (base.isAuthority)
            {
                base.characterMotor.Motor.ForceUnground();
            }
            base.PlayAnimation("FullBody, Override", "ParryEnter");

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);
            base.characterMotor.disableAirControlUntilCollision = false;

            base.StartAimMode(this.searchTime, false);
        }

        public override void OnExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (invisible)
            {
                if (this.characterModel)
                {
                    this.characterModel.invisibilityCount--;
                }
                if (NetworkServer.active)
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
                }
            }
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.stopwatch += Time.fixedDeltaTime;

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
                    if (this.fixedAge <= attackDuration + this.searchTime) // attack
                    {
                        if (!this.hasFired)
                        {
                            hasFired = true;
                            if (this.characterModel)
                            {
                                this.characterModel.invisibilityCount++;
                                invisible = true;
                            }
                            this.maxAttackCount = (int)Math.Ceiling(base.characterBody.attackSpeed * baseAttackCount);
                            if (NetworkServer.active)
                            {
                                base.characterBody.AddBuff(RoR2Content.Buffs.Intangible);
                            }
                        }
                        if (base.isAuthority && this.fixedAge > (attackDuration / this.maxAttackCount * (this.attackCount + 1)) + this.searchTime)
                        {
                            if (target)
                            {
                                targetPosition = target.transform.position;
                            }
                            FireBlastAttack();
                            this.attackCount++;
                        }
                    }

                    if (this.fixedAge > attackDuration + this.searchTime) //end lag start
                    {
                        if (invisible)
                        {
                            if (this.characterModel)
                            {
                                this.characterModel.invisibilityCount--;
                            }
                            invisible = false;
                            if (NetworkServer.active)
                            {
                                base.characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
                            }
                            this.endLag = baseEndLag / base.characterBody.attackSpeed;
                            base.PlayAnimation("FullBody, Override", "ParryRelease");
                            base.characterDirection.forward = base.characterDirection.forward * -1;
                            base.characterDirection.moveVector = base.characterDirection.moveVector * -1;
                        }
                    }

                    if (base.isAuthority && (this.fixedAge > attackDuration + this.searchTime + this.endLag)) //end
                    {
                        base.characterMotor.velocity = Vector3.zero;
                        this.outer.SetNextStateToMain();
                        return;
                    }
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

        private void FireBlastAttack()
        {
            if (base.isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = range;
                blastAttack.procCoefficient = procCoefficient;
                blastAttack.position = targetPosition;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.baseDamage = base.characterBody.damage * damageCoefficient;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.damageType = damageType;
                blastAttack.baseForce = -pushForce;
                blastAttack.teamIndex = base.teamComponent.teamIndex;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blastAttack.Fire();
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