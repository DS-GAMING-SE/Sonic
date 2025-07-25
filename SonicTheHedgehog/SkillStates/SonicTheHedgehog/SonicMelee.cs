﻿using EntityStates;
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
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicMelee : BaseSkillState
    {
        public int swingIndex=0;

        protected string hitboxName;

        protected virtual float launchPushForce
        {
            get { return 250f; }
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
        protected float meleeFiringDisplacement = 0.25f;
        protected float meleeDisplacement = 0.011f;
        protected float displacementDIMultiplier = 1.5f;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString;
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab = Modules.Assets.meleeImpactEffect;
        protected NetworkSoundEventIndex impactSound = Modules.Assets.meleeHitSoundEvent.index;

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
        private HomingTracker homingTracker;
        private bool effectPlayed = false;
        private bool swingSoundPlayed = false;
        private bool bufferedHomingAttack = false;

        public SkillDefs.IMeleeSkill meleeSkillDef;

        public override void OnEnter()
        {
            base.OnEnter();
            this.flight = base.characterBody.GetComponent<ICharacterFlightParameterProvider>();
            this.homingTracker = base.characterBody.GetComponent<HomingTracker>();
            this.hasFired = false;

            meleeSkillDef = base.skillLocator.primary.skillDef as SkillDefs.IMeleeSkill;

            this.impactSound = swingIndex == 4 ? Modules.Assets.meleeFinalHitSoundEvent.index : Modules.Assets.meleeHitSoundEvent.index;
            this.swingSoundString = swingIndex == 4 ? "Play_sonicthehedgehog_swing_strong" : "Play_sonicthehedgehog_swing";
            this.hitboxName = "Sword";
            this.procCoefficient = StaticValues.meleeProcCoefficient;
            this.damageCoefficient = swingIndex == 4 ? Modules.StaticValues.finalMeleeDamageCoefficient : Modules.StaticValues.meleeDamageCoefficient;
            this.attackRecoil = swingIndex == 4 ? 2.5f : 0.6f;
            this.duration = swingIndex == 4 ? Modules.StaticValues.finalMeleeBaseSpeed / this.attackSpeedStat : Modules.StaticValues.meleeBaseSpeed / this.attackSpeedStat;
            this.earlyExitTime = swingIndex == 4 ? Modules.StaticValues.finalMeleeBaseSpeed * 0.1f / this.attackSpeedStat : Modules.StaticValues.meleeBaseSpeed * 0.1f / this.attackSpeedStat;
            this.attackStartTime = swingIndex == 4 ? 0.55f: 0.25f; //percent of duration
            this.attackEndTime = swingIndex == 4 ? 0.7f : 0.35f; //percent of duration
            this.hitStopDuration = swingIndex == 4 ? 0.15f : 0.04f;
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
            string anim;
            if (swingIndex==4)
            {
                anim = "FinalKick";
            }
            else
            {
                anim = swingIndex % 2 == 0 ? "Punch" : "Kick";
            }
            base.PlayAnimation("FullBody, Override", anim, "Slash.playbackRate", this.duration*0.8f);
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
            this.muzzleString = swingIndex == 4 ? "SwingFinal" : "SwingCenter";
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
            this.attack.forceVector = swingIndex == 4 ? (Vector3.Normalize(Vector3.up + aim) * launchPushForce) : aim * launchPushForce;

            if (base.isAuthority)
            {
                FiringDisplacement();

                if (this.attack.Fire())
                {
                    this.OnHitEnemyAuthority();

                }
            }
        }


        protected virtual void SetNextState()
        {
            int index = this.swingIndex;
            if (index < 4) index += 1;
            else index = 0;

            if (Config.KeyPressHomingAttack().Value)
            {
                if (bufferedHomingAttack)
                {
                    if (SetNextStateToMeleeSelect(index))
                    {
                        return;
                    }
                }
                else
                {
                    SonicMelee meleeType = (SonicMelee)EntityStateCatalog.InstantiateState(base.skillLocator.primary.skillDef.activationState.stateType);
                    meleeType.swingIndex = index;
                    this.outer.SetNextState(meleeType);
                    base.characterBody.OnSkillActivated(skillLocator.primary);
                    return;
                }
            }
            else
            {
                if (SetNextStateToMeleeSelect(index))
                {
                    return;
                }
            }
            this.outer.SetNextStateToMain();
        }

        protected bool SetNextStateToMeleeSelect(int index)
        {
            if (meleeSkillDef != null)
            {
                EntityState meleeState = SkillDefs.MeleeSkillDef.DecideNextState(base.skillLocator.primary, homingTracker, index);
                this.outer.SetNextState(meleeState);
                base.characterBody.OnSkillActivated(skillLocator.primary);
                return true;
            }
            return false;
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
            vector *= swingIndex == 4 ? 0.6f : 1;
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
                vector *= swingIndex == 4 ? 0.9f : 1.3f;
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
                    this.SetNextState();
                    return;
                }
            }

            if (base.isAuthority && this.stopwatch >= (this.duration/2) && base.inputBank.skill1.justPressed)
            {
                if (homingTracker && homingTracker.CanHomingAttack())
                {
                    bufferedHomingAttack = true;
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
                if (bufferedHomingAttack && homingTracker.CanHomingAttack())
                {
                    HomingAttack homingAttack = EntityStateCatalog.InstantiateState(((SkillDefs.IMeleeSkill)base.skillLocator.primary.skillDef).homingAttackState.stateType) as HomingAttack;
                    homingAttack.target = homingTracker.GetTrackingTarget();
                    this.outer.SetNextState(homingAttack);
                    base.characterBody.OnSkillActivated(skillLocator.primary);
                    return;
                }
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
            if (swingIndex == 4)
            {
                this.attack.damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launch);
            }
            else
            {
                this.attack.damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launchOnKill);
            }
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