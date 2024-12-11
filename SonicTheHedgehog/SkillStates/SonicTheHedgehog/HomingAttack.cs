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

namespace SonicTheHedgehog.SkillStates
{
    public class HomingAttack : BaseSkillState
    {
        protected string hitboxName;

        protected DamageType damageType = DamageType.Generic;
        protected float damageCoefficient;
        protected float procCoefficient;
        protected float pushForce = 50f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float attackStartTime;
        protected float attackEndTime;
        protected float hitStopDuration;
        protected float attackRecoil;
        protected float hitHopVelocity;
        protected bool cancelled = false;

        protected float maxHomingAttackRange;
        protected float homingAttackSpeed;
        protected float estimatedHomingAttackTime;
        protected float homingAttackOvershoot;
        protected virtual float baseHomingAttackEndLag
        {
            get { return 0.3f; }
        }
        protected float homingAttackEndLag;
        protected float homingAttackHitHopVelocity = 12;
        public HurtBox target;
        private Vector3 targetDirection;
        protected bool homingAttackHit;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string homingAttackSoundString = "Play_homing_attack";
        protected string muzzleString;
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab = Modules.Assets.meleeImpactEffect;
        protected NetworkSoundEventIndex impactSound = Modules.Assets.meleeHitSoundEvent.index;
        protected GameObject homingAttackEffect;

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
        private bool animationEnded=false;
        private ICharacterFlightParameterProvider flight;
        private HomingTracker homingTracker;
        private bool effectPlayed = false;

        public static event Action<HomingAttack, HurtBox> onAuthorityHitEnemy;

        public override void OnEnter()
        {
            base.OnEnter();
            this.flight = base.characterBody.GetComponent<ICharacterFlightParameterProvider>();
            this.homingTracker = base.characterBody.GetComponent<HomingTracker>();
            //this.homingAttackEffect = UnityEngine.GameObject.Instantiate<GameObject>(Assets.homingAttackTrailEffect, base.FindModelChild("MainHurtbox"));
            this.hasFired = false;
            this.maxHomingAttackRange = homingTracker.MaxRange();
            this.homingAttackSpeed = homingTracker.Speed();
            this.homingAttackOvershoot = 1.4f;

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(Modules.Buffs.ballBuff);
            }
            this.damageCoefficient = Modules.StaticValues.homingAttackDamageCoefficient;
            this.procCoefficient = StaticValues.homingAttackProcCoefficient;
            this.attackRecoil = 3;
            this.hitStopDuration = 0.1f;
            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            this.targetDirection = Vector3.zero;
            if (this.target != null)
            {
                this.targetDirection = (this.target.transform.position - base.transform.position);
            }
            Util.PlaySound(homingAttackSoundString, base.gameObject);
            if (base.isAuthority)
            {
                base.characterMotor.Motor.ForceUnground();
                if (targetDirection != Vector3.zero)
                {
                    EffectManager.SimpleEffect(Modules.Assets.homingAttackLaunchEffect, base.gameObject.transform.position, Util.QuaternionSafeLookRotation(targetDirection), true);
                }
            }
            EndChrysalis();
            this.estimatedHomingAttackTime = (targetDirection.magnitude / homingAttackSpeed) * homingAttackOvershoot;
            this.homingAttackEndLag = baseHomingAttackEndLag / this.attackSpeedStat;
            this.hitboxName = "Ball";
            //this.hitSoundString = "Play_homing_impact";
            this.impactSound = Modules.Assets.homingHitSoundEvent.index;
            base.PlayAnimation("FullBody, Override", "Ball");
            base.characterMotor.disableAirControlUntilCollision = false;

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);

            PrepareOverlapAttack();
        }

        public override void OnExit()
        {
            if (!this.hasFired && !this.cancelled) this.FireAttack();
            if (!animationEnded || cancelled)
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
            }
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

            if (this.homingAttackEffect)
            {
                Destroy(this.homingAttackEffect);
            }
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }

        protected virtual void PlayHomingAttackHitEffect()
        {
            EffectManager.SimpleEffect(Modules.Assets.homingAttackHitEffect, base.gameObject.transform.position, Util.QuaternionSafeLookRotation(targetDirection), true);
            if (this.homingAttackEffect)
            {
                this.homingAttackEffect.transform.parent = null;
            }
        }

        protected virtual void OnHitEnemyAuthority()
        {
            if (!effectPlayed)
            {
                //Util.PlaySound(this.hitSoundString, base.gameObject);
                PlayHomingAttackHitEffect();
                effectPlayed = true;
            }

            if (base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
                if (!Flying())
                {
                    base.SmallHop(base.characterMotor, homingAttackHitHopVelocity);
                }
            }

            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(Modules.Buffs.ballBuff);
            }
            this.stopwatch = 0f;
            this.hasFired = true;
            this.hasHopped = true;

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
                List<HurtBox> hitResults = new List<HurtBox>();
                if (this.attack.Fire(hitResults))
                {
                    base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                    foreach (HurtBox hurtBox in hitResults)
                    {
                        if (onAuthorityHitEnemy != null) onAuthorityHitEnemy.Invoke(this, hurtBox);
                    }
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
                base.PlayAnimation("Body", "Backflip", "Slash.playbackRate", this.homingAttackEndLag * 2);
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
                if (this.hasFired)
                {
                    if (this.stopwatch >= this.homingAttackEndLag)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
                else
                {
                    if (fixedAge < this.estimatedHomingAttackTime)
                    {
                        this.homingAttackSpeed = homingTracker.Speed();
                        if (this.target != null)
                        {
                            targetDirection = this.target.transform.position - base.characterMotor.transform.position;
                        }
                        if (this.homingAttackSpeed * Time.fixedDeltaTime > targetDirection.magnitude * 3) // Slow when approaching enemy at high speeds
                        {
                            this.homingAttackSpeed = Mathf.Max(targetDirection.magnitude * 3, 15);
                        }
                        base.characterMotor.velocity = targetDirection.normalized * this.homingAttackSpeed;
                        base.characterDirection.forward = targetDirection.normalized;
                        if (base.isAuthority && base.isGrounded)
                        {
                            base.characterMotor.Motor.ForceUnground();
                        }
                        if (fixedAge>this.estimatedHomingAttackTime/2.5f)
                        {
                            this.FireAttack();
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

        private bool Flying()
        {
            return flight != null && flight.isFlying;
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
            this.attack.damageType.damageSource = DamageSource.Primary;
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