using EntityStates;
using R2API;
using Rewired;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Modules;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicMelee : BaseSkillState
    {
        public int swingIndex=0;

        protected string hitboxName;

        protected DamageType damageType = DamageType.Generic;
        protected float damageCoefficient;
        protected float procCoefficient = 1f;
        protected float pushForce = 50f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float attackStartTime;
        protected float attackEndTime;
        protected float hitStopDuration;
        protected float attackRecoil;
        protected float hitHopVelocity;
        protected bool cancelled = false;
        protected float meleeDisplacement = 0.3f;
        protected float displacementDIMultiplier = 1.5f;

        protected float maxHomingAttackRange;
        protected float homingAttackSpeed;
        protected bool homingAttack;
        protected float estimatedHomingAttackTime;
        protected float homingAttackOvershoot;
        protected float baseHomingAttackEndLag = 0.3f;
        protected float superHomingAttackEndLag = 0.1f;
        protected float homingAttackEndLag;
        protected float homingAttackHitHopVelocity = 10;
        private HurtBox target;
        private Vector3 targetDirection;
        protected bool homingAttackHit;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString;
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab = Assets.meleeImpactEffect;
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
        private bool animationEnded=false;
        private ICharacterFlightParameterProvider flight;
        private bool effectPlayed = false;

        public override void OnEnter()
        {
            base.OnEnter();
            flight = base.characterBody.GetComponent<ICharacterFlightParameterProvider>();
            this.hasFired = false;
            this.homingAttack = false;
            this.maxHomingAttackRange = 15f+(base.characterBody.moveSpeed*base.characterBody.sprintingSpeedMultiplier)*2f;
            this.homingAttackSpeed = (base.characterBody.moveSpeed*base.characterBody.sprintingSpeedMultiplier) * 5;
            this.homingAttackOvershoot = 1.4f;

            BullseyeSearch search = new BullseyeSearch();
            search.searchOrigin = base.GetAimRay().origin;
            search.searchDirection = base.GetAimRay().direction;
            search.maxDistanceFilter = this.maxHomingAttackRange;
            search.minDistanceFilter = 8;
            search.maxAngleFilter = 10;
            TeamMask mask = TeamMask.GetEnemyTeams(base.teamComponent.teamIndex);
            search.teamMaskFilter = mask;
            search.sortMode = BullseyeSearch.SortMode.Angle;
            search.RefreshCandidates();

            SphereSearch sphereSearch= new SphereSearch();
            sphereSearch.origin = base.characterBody.transform.position+base.GetAimRay().direction;
            sphereSearch.radius = 3;
            sphereSearch.mask = LayerIndex.entityPrecise.mask;
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(mask);

            if (search.GetResults().Any()&&!sphereSearch.GetHurtBoxes().Any()) // Homing Attack
            {
                if (NetworkServer.active)
                {
                    base.characterBody.AddBuff(Modules.Buffs.ballBuff);
                }
                this.damageCoefficient = Modules.StaticValues.homingAttackDamageCoefficient;
                this.attackRecoil = 3;
                this.hitStopDuration = 0.07f;
                this.target = search.GetResults().First<HurtBox>();
                this.homingAttack = true;
                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                this.targetDirection = Vector3.zero;
                if (this.target!=null)
                {
                    this.targetDirection = (this.target.transform.position - base.transform.position);
                }
                if (base.isAuthority)
                {
                    Util.PlaySound("HenryRoll", base.gameObject);
                    base.characterMotor.Motor.ForceUnground();
                    if (targetDirection!=Vector3.zero)
                    {
                        EffectManager.SimpleEffect(Assets.homingAttackLaunchEffect, base.gameObject.transform.position, Util.QuaternionSafeLookRotation(targetDirection), true);
                    }
                }
                EndChrysalis();
                this.estimatedHomingAttackTime = (targetDirection.magnitude / homingAttackSpeed) * homingAttackOvershoot;
                this.homingAttackEndLag = (base.characterBody.HasBuff(Modules.Buffs.superSonicBuff) ? superHomingAttackEndLag : baseHomingAttackEndLag) / this.attackSpeedStat;
                this.hitboxName = "Ball";
                base.PlayAnimation("FullBody, Override", "Ball");
                base.characterMotor.disableAirControlUntilCollision = false;
            }
            else // Normal Melee
            {
                
                this.hitboxName = "Sword";
                this.damageCoefficient = swingIndex == 4 ? Modules.StaticValues.finalMeleeDamageCoefficient : Modules.StaticValues.meleeDamageCoefficient;
                this.attackRecoil = swingIndex == 4 ? 2.5f : 0.6f;
                this.duration = swingIndex == 4 ? Modules.StaticValues.finalMeleeBaseSpeed / this.attackSpeedStat : Modules.StaticValues.meleeBaseSpeed / this.attackSpeedStat;
                this.earlyExitTime = swingIndex == 4 ? Modules.StaticValues.finalMeleeBaseSpeed * 0.2f / this.attackSpeedStat : Modules.StaticValues.meleeBaseSpeed*0.1f / this.attackSpeedStat;
                this.attackStartTime = swingIndex == 4 ? Modules.StaticValues.finalMeleeBaseSpeed * 0.6f / this.attackSpeedStat : Modules.StaticValues.meleeBaseSpeed * 0.45f / this.attackSpeedStat;
                this.attackEndTime = swingIndex == 4 ? Modules.StaticValues.finalMeleeBaseSpeed * 0.8f / this.attackSpeedStat : Modules.StaticValues.meleeBaseSpeed * 0.7f / this.attackSpeedStat;
                this.hitStopDuration = swingIndex == 4 ? 0.15f : 0.04f;
                this.hitHopVelocity= Flying() ? 0 : 3+(3/this.attackSpeedStat);
                StartAimMode();
                PlayAttackAnimation();
            }

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);

            PrepareOverlapAttack();
        }

        protected virtual void PlayAttackAnimation()
        {
            string anim;
            if (swingIndex==4)
            {
                anim = "FinalKick";
            }
            else
            {
                anim = swingIndex % 2 == 0 ? "Punch" : "Kick";
            }
            base.PlayAnimation("FullBody, Override", anim, "Slash.playbackRate", this.duration*0.65f);
        }

        public override void OnExit()
        {
            if (!this.hasFired && !this.cancelled) this.FireAttack();
            if ((homingAttack && !animationEnded) || cancelled)
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
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }

        protected virtual void PlaySwingEffect()
        {
            this.muzzleString = swingIndex == 4 ? "SwingFinal" : "SwingCenter";
            EffectManager.SimpleMuzzleFlash(Modules.Assets.meleeHitEffect, base.gameObject, this.muzzleString, true);
        }

        protected virtual void PlayHomingAttackHitEffect()
        {
            EffectManager.SimpleEffect(Assets.homingAttackHitEffect, base.gameObject.transform.position, Util.QuaternionSafeLookRotation(targetDirection), true);
        }

        protected virtual void OnHitEnemyAuthority()
        {
            if (homingAttack)
            {
                if (!effectPlayed)
                {
                    Util.PlaySound(this.hitSoundString, base.gameObject);
                    PlayHomingAttackHitEffect();
                    effectPlayed = true;
                }

                if (base.characterMotor)
                {
                    base.characterMotor.velocity = Vector3.zero;
                    base.characterMotor.velocity.y = Flying() ? 0 : homingAttackHitHopVelocity;
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
            else
            {
                if (!effectPlayed)
                {
                    Util.PlaySound(this.hitSoundString, base.gameObject);
                    PlaySwingEffect();
                    effectPlayed = true;
                }
                if (!this.hasHopped)
                {
                    if (base.characterMotor && !base.characterMotor.isGrounded && this.hitHopVelocity > 0f)
                    {
                        base.SmallHop(base.characterMotor, this.hitHopVelocity);
                    }

                    this.hasHopped = true;
                }

                if (!this.inHitPause && this.hitStopDuration > 0f)
                {
                    this.storedVelocity = base.characterMotor.velocity;
                    this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Slash.playbackRate");
                    this.hitPauseTimer = this.hitStopDuration / this.attackSpeedStat;
                    this.inHitPause = true;
                }
            }
            
        }

        private void FireAttack()
        {
            if (homingAttack)
            {
                if (base.isAuthority)
                {
                    if (this.attack.Fire())
                    {
                        base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                        this.OnHitEnemyAuthority();
                    }
                }
            }
            else
            {
                if (!this.hasFired)
                {
                    this.hasFired = true;
                    Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);

                    if (base.isAuthority)
                    {
                        base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                    }
                }

                if (base.isAuthority)
                {
                    Displacement();
                    if (this.attack.Fire())
                    {
                        this.OnHitEnemyAuthority();

                    }
                }
            }
        }

        protected virtual void SetNextState()
        {
            int index = this.swingIndex;
            if (index < 4) index += 1;
            else index = 0;

            this.outer.SetNextState(new SonicMelee
            {
                swingIndex = index
            });
        }

        private void Displacement()
        {
            Vector3 vector;
            if (base.characterBody.inputBank.moveVector != Vector3.zero)
            {
                vector = base.characterBody.inputBank.moveVector * displacementDIMultiplier;
            }
            else
            {
                vector = base.characterDirection.forward;
            }
            vector = vector * meleeDisplacement;
            vector *= swingIndex == 4 ? 0.6f : 1;
            base.characterMotor.AddDisplacement(vector);
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
                if (homingAttack)
                {
                    animationEnded = true;
                    base.PlayAnimation("FullBody, Override", "BufferEmpty");
                    base.PlayAnimation("Body", "Backflip");
                }
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

            if (homingAttack&&base.isAuthority)
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
                        this.homingAttackSpeed = (base.characterBody.moveSpeed * base.characterBody.sprintingSpeedMultiplier) * 5;
                        if (this.target != null)
                        {
                            targetDirection = this.target.transform.position - base.transform.position;
                        }
                        base.characterMotor.velocity = targetDirection.normalized * this.homingAttackSpeed;
                        base.characterDirection.forward = targetDirection.normalized;
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
            else
            {
                if (this.stopwatch >= (this.duration * this.attackStartTime) && this.stopwatch <= (this.duration * this.attackEndTime))
                {
                    this.FireAttack();
                }

                if (this.stopwatch >= (this.duration - this.earlyExitTime) && base.isAuthority)
                {
                    if (base.inputBank.skill1.down)
                    {
                        if (!this.hasFired)
                        {
                            this.FireAttack();
                        }
                        this.SetNextState();
                        return;
                    }
                }


                if (base.isAuthority && base.inputBank.skill3.justPressed && base.skillLocator.utility.IsReady())
                {
                    cancelled = true;
                    base.skillLocator.utility.OnExecute();
                    return;
                }
                if (this.stopwatch >= this.duration && base.isAuthority)
                {
                    this.outer.SetNextStateToMain();
                    return;
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
            //this.attack.impactSound = this.impactSound;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return homingAttack ? InterruptPriority.PrioritySkill : InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.swingIndex = reader.ReadInt32();
        }
    }
}