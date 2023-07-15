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
    public class GrandSlamDash : BaseSkillState
    {
        protected string hitboxName = "Ball";

        protected DamageType damageType = DamageType.Stun1s;
        protected float damageCoefficient = Modules.StaticValues.grandSlamDashDamageCoefficient;
        protected float procCoefficient = 0.5f;
        protected float pushForce = 0f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseAttackStartTime = 0.5f;
        protected float attackStartTime;
        protected float attackRecoil = 3f;
        protected float hitHopVelocity=6f;
        protected bool cancelled = false;

        protected float maxDashRange;
        protected float dashSpeed;
        protected float estimatedDashTime;
        protected float dashOvershoot;
        protected HurtBox target;
        private Vector3 targetDirection;

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
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(Modules.Buffs.ballBuff);
            }
            this.dashSpeed = (base.characterBody.moveSpeed * base.characterBody.sprintingSpeedMultiplier) * 5;
            this.maxDashRange = 15f + (base.characterBody.moveSpeed * base.characterBody.sprintingSpeedMultiplier) * 2f;
            this.hasFired = false;
            this.attackStartTime = baseAttackStartTime / base.characterBody.attackSpeed;
            this.dashOvershoot = 1.5f;
            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            BullseyeSearch search = new BullseyeSearch();
            search.searchOrigin = base.GetAimRay().origin;
            search.searchDirection = base.GetAimRay().direction;
            search.maxDistanceFilter = this.maxDashRange;
            search.maxAngleFilter = 13;
            TeamMask mask = TeamMask.GetEnemyTeams(base.teamComponent.teamIndex);
            search.teamMaskFilter = mask;
            search.sortMode = BullseyeSearch.SortMode.Angle;
            search.RefreshCandidates();
            this.target = search.GetResults().FirstOrDefault<HurtBox>();
            this.targetDirection = (base.GetAimRay().direction.normalized*maxDashRange)/2.2f;
            if (this.target!=null)
            {
                this.targetDirection = (this.target.transform.position - base.transform.position);
            }
            base.characterMotor.Motor.ForceUnground();
            this.estimatedDashTime = (targetDirection.magnitude / dashSpeed) * dashOvershoot;
            this.hitboxName = "Ball";
            //base.PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", this.estimatedDashTime);
            Util.PlaySound("HenryRoll", base.gameObject);

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);

            PrepareOverlapAttack();
        }

        public override void OnExit()
        {
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(Modules.Buffs.ballBuff);
            }
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }


        protected virtual void OnHitEnemyAuthority()
        {
             Util.PlaySound(this.hitSoundString, base.gameObject);
                  
             SetNextState();
        }

        private void FireAttack()
        {

                if (base.isAuthority)
                {
                    if (this.attack.Fire())
                    {
                        base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                        if (this.target==null)
                        {
                            this.target = this.attack.overlapList.FirstOrDefault().hurtBox;
                        }
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

            if (base.isAuthority)
            {
                if (fixedAge < this.attackStartTime)
                {
                    base.characterMotor.velocity = Vector3.Lerp(base.characterMotor.velocity, Vector3.zero, fixedAge/(this.attackStartTime * 0.6f));
                }
                //else if (!dashCalced)
                //{
                //    dashCalced= true;
                //    this.dashSpeed = (base.characterBody.moveSpeed * base.characterBody.sprintingSpeedMultiplier) * 5;
                //    this.maxDashRange = 15f + (base.characterBody.moveSpeed * base.characterBody.sprintingSpeedMultiplier) * 2f;
                //}
                else if (fixedAge < this.estimatedDashTime+this.attackStartTime && fixedAge > this.attackStartTime)
                {
                    if (this.target != null)
                    {
                        targetDirection = this.target.transform.position - base.transform.position;
                    }
                    base.characterMotor.velocity = targetDirection.normalized * dashSpeed;
                    base.characterDirection.forward = targetDirection.normalized;
                    this.FireAttack();
                }
                if (fixedAge>this.estimatedDashTime+this.attackStartTime)
                {
                    base.characterMotor.velocity = Vector3.zero;
                    base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
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
            this.attack.pushAwayForce = this.pushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.target);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            if (reader.ReadHurtBoxReference().ResolveHurtBox()!=null)
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