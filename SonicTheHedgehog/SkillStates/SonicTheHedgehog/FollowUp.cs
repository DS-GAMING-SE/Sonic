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
    public class FollowUp : BaseSkillState
    {
        protected string hitboxName = "FollowUp";

        protected virtual float launchPushForce
        {
            get { return 400f; }
        }

        protected DamageType damageType = DamageType.Generic;
        protected float hitStopDuration;
        protected float attackRecoil;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab = Modules.Assets.meleeImpactEffect;
        protected NetworkSoundEventIndex impactSound = Modules.Assets.meleeFinalHitSoundEvent.index;

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
        private bool swingSoundPlayed = false;

        private ParryFollowUpTracker followUp;

        public override void OnEnter()
        {
            base.OnEnter();
            this.flight = base.characterBody.GetComponent<ICharacterFlightParameterProvider>();
            this.followUp = base.GetComponent<ParryFollowUpTracker>();
            if (followUp)
            {
                followUp.RemoveFollowUpAttack();
            }
            this.hasFired = false;
            this.swingSoundString = "Play_sonicthehedgehog_swing_strong";
            this.hitStopDuration = 0.2f;
            this.attackRecoil = 3f;

            this.duration = StaticValues.followUpDuration / base.characterBody.attackSpeed;

            StartAimMode();
            PlayAttackAnimation();

            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);

            if (!HedgehogUtils.Helpers.Flying(flight) && !base.characterMotor.isGrounded && base.isAuthority) { SmallHop(base.characterMotor, 12f); }

            PrepareOverlapAttack();

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.SmallArmorBoost);
            }
        }

        protected virtual void PlayAttackAnimation()
        {
            base.PlayAnimation("Body", "ParryFollowUp", "Slash.playbackRate", this.duration * 0.8f);
        }

        public override void OnExit()
        {
            if (!this.hasFired) this.FireAttack();
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.SmallArmorBoost);
            }
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }

        protected virtual void OnHitEnemyAuthority()
        {
            if (!effectPlayed)
            {
                effectPlayed = true;
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
                    base.AddRecoil(-0.4f * this.attackRecoil, -0.3f * this.attackRecoil, -1f * this.attackRecoil, 1f * this.attackRecoil);

                    OnFireAuthority();
                }
            }

            if (base.isAuthority)
            {
                if (this.attack.Fire())
                {
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
            }

            if (!this.inHitPause)
            {
                this.stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                if (this.animator) this.animator.SetFloat("Slash.playbackRate", 0f);
            }

            if (base.isAuthority)
            {
                base.characterMotor.velocity.y = Mathf.Max(base.characterMotor.velocity.y, -3f);
            }

            if (this.stopwatch >= (this.duration * StaticValues.followUpStartUpPercentOfDuration) && this.stopwatch <= (this.duration * StaticValues.followUpEndLagStartPercentOfDuration))
            {
                this.attack.forceVector = base.characterDirection.forward * launchPushForce;
                if (!this.hasFired)
                {
                    Util.PlayAttackSpeedSound(swingSoundString, base.gameObject, base.attackSpeedStat);
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.followUpKickEffect, base.gameObject, this.muzzleString, true);
                }
                if (base.isAuthority)
                {
                    this.FireAttack();
                }
                this.hasFired = true;
            }

            if (this.stopwatch >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
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
            this.attack.damageType.damageSource = DamageSource.Secondary;
            this.attack.damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launch);
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = StaticValues.followUpDamageCoefficient * this.damageStat;
            this.attack.procCoefficient = 1f;
            this.attack.hitEffectPrefab = this.hitEffectPrefab;
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