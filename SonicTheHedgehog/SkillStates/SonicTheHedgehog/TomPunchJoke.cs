using EntityStates;
using R2API;
using Rewired;
using RiskOfOptions.Components.AssetResolution.Data;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class TomPunchJoke : BaseSkillState
    {
        protected string hitboxName;

        protected virtual float launchPushForce
        {
            get { return 9999f; }
        }

        protected DamageType damageType = DamageType.Generic;
        protected float damageCoefficient;
        protected float procCoefficient;
        protected float pushForce;
        protected Vector3 bonusForce;
        protected float attackStartTime;
        protected float attackEndTime;
        protected float hitStopDuration;
        protected float attackRecoil;
        protected float hitHopVelocity;
        protected bool cancelled = false;
        protected float meleeFiringDisplacement = 0.3f;
        protected float meleeDisplacement = 0.005f;
        protected float displacementDIMultiplier = 1.5f;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString;
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab = Modules.Assets.meleeImpactEffect;
        protected NetworkSoundEventIndex impactSound = Modules.Assets.grandSlamHitSoundEvent.index;

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
        private ICharacterFlightParameterProvider flight;
        private bool effectPlayed = false;
        private bool swingSoundPlayed = false;

        public override void OnEnter()
        {
            base.OnEnter();
            this.flight = base.characterBody.GetComponent<ICharacterFlightParameterProvider>();
            this.hasFired = false;
            this.swingSoundString = "Play_sonicthehedgehog_swing_strong";
            this.hitboxName = "FollowUp";
            this.procCoefficient = 10;
            this.damageCoefficient = 50;
            this.attackRecoil = 2.5f;
            this.duration =  1.1f / this.attackSpeedStat;
            this.earlyExitTime = 0.05f / this.attackSpeedStat;
            this.attackStartTime = 0.1f; //percent of duration
            this.attackEndTime = 0.2f; //percent of duration
            this.hitStopDuration = 0.1f;
            this.hitHopVelocity = Flying() ? 0 : 3 + (3 / this.attackSpeedStat);
            StartAimMode();
            PlayAttackAnimation();

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);

            PrepareOverlapAttack();
        }

        protected virtual void PlayAttackAnimation()
        {
            base.PlayAnimation("FullBody, Override", "Punch", "Slash.playbackRate", this.duration*0.45f);
        }

        public override void OnExit()
        {
            if (!this.hasFired && !this.cancelled) this.FireAttack();
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }

        protected virtual void PlaySwingEffect()
        {
            this.muzzleString = "SwingCenter";
            EffectManager.SimpleMuzzleFlash(Modules.Assets.meleeHitEffect, base.gameObject, this.muzzleString, true);
        }

        protected virtual void OnHitEnemyAuthority()
        {
            if (!effectPlayed)
            {
                //Util.PlaySound(this.hitSoundString, base.gameObject);
                PlaySwingEffect();
                effectPlayed = true;
            }
            if (!this.hasHopped && this.hitHopVelocity > 0f)
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


        private void FireAttack()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);

                if (base.isAuthority)
                {
                    base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);

                    OnFireAuthority();
                }
            }
            Vector3 aim = base.inputBank.aimDirection;
            aim.y = 0;
            aim = aim.normalized;
            this.attack.forceVector = aim * launchPushForce;

            if (base.isAuthority)
            {
                FiringDisplacement();

                if (this.attack.Fire())
                {
                    this.OnHitEnemyAuthority();

                }
            }
        }

        private void FiringDisplacement()
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
            vector *= meleeFiringDisplacement;
            vector *= 0.6f;
            if (Flying())
            {
                vector *= 2;
            }
            if (this.inHitPause)
            {
                vector *= 0.3f;
            }
            base.characterMotor.AddDisplacement(vector);
        }

        private void Displacement()
        {
            if (base.characterBody.inputBank.moveVector != Vector3.zero && !this.inHitPause)
            {
                Vector3 vector;
                vector = base.characterBody.inputBank.moveVector * base.characterBody.moveSpeed;
                vector *= meleeDisplacement;
                vector *=  0.9f;
                if (Flying())
                {
                    vector *= 1.5f;
                }
                vector *= Mathf.Lerp(1, 0.5f, Vector3.Dot(base.characterBody.inputBank.moveVector.normalized, base.characterDirection.forward) * -1);
                base.characterMotor.AddDisplacement(vector);
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
            }

            Displacement();

            if (!this.inHitPause)
            {
                this.stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                if (this.animator) this.animator.SetFloat("Swing.playbackRate", 0f);
            }

            if (this.stopwatch >= (this.duration * this.attackStartTime) && this.stopwatch <= (this.duration * this.attackEndTime))
            {
                this.FireAttack();
                if (!swingSoundPlayed)
                {
                    Util.PlayAttackSpeedSound(swingSoundString, base.gameObject, base.attackSpeedStat);
                    swingSoundPlayed = true;
                }
            }

            if (this.stopwatch >= (this.duration - this.earlyExitTime) && base.isAuthority)
            {
                if (base.inputBank.skill1.down)
                {
                    if (!this.hasFired)
                    {
                        this.FireAttack();
                    }
                    this.outer.SetNextStateToMain();
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

        private bool Flying()
        {
            return flight != null && flight.isFlying;
        }

        protected virtual void OnFireAuthority()
        {

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
            this.attack.damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launch);
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = this.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = this.procCoefficient;
            this.attack.hitEffectPrefab = this.hitEffectPrefab;
            this.attack.pushAwayForce = 0;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}