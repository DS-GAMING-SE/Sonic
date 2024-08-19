﻿using EntityStates;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class ParryExit : BaseSkillState
    {
        public static float baseEndLag = Modules.StaticValues.parryEndLag;
        public static float baseEndLagFail = Modules.StaticValues.parryFailEndLag;
        public static float superParryRange = 50;

        public static float endAnimationPercent = 0.5f;

        private float endLag;

        public bool parrySuccess = false;

        private string muzzleString= "BallHitbox";
        private Vector3 targetVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            this.endLag = parrySuccess ? baseEndLag / base.characterBody.attackSpeed : baseEndLagFail / base.characterBody.attackSpeed;
            base.characterMotor.disableAirControlUntilCollision = false;
            base.modelLocator.normalizeToFloor = true;
            base.PlayAnimation("FullBody, Override", "ParryRelease", "Slash.playbackRate", endLag * endAnimationPercent);
            if (base.isAuthority)
            {
                EffectManager.SimpleMuzzleFlash(Assets.parryEffect, base.gameObject, this.muzzleString, true);
            }
            Util.PlaySound("Play_swing_low", base.gameObject);
            if (parrySuccess)
            {
                OnSuccessfulParry();
                if (NetworkServer.active)
                {
                    base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, StaticValues.parryLingeringInvincibilityDuration, 1);
                    base.characterBody.AddTimedBuff(Buffs.parryBuff, StaticValues.parryBuffDuration, 1);
                }
                if (base.isAuthority)
                {
                    EffectManager.SimpleMuzzleFlash(Assets.parryActivateEffect, base.gameObject, this.muzzleString, true);
                }
                Util.PlaySound("Play_parry", base.gameObject);
                RechargeCooldowns();
            }
        }

        public override void OnExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.modelLocator.normalizeToFloor = false;
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            targetVelocity = Vector3.zero;
            base.characterMotor.velocity = targetVelocity;

            if (fixedAge >= endLag)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void RechargeCooldowns()
        {
            base.skillLocator.primary.RunRecharge(StaticValues.parryCooldownReduction);
            if (typeof(Boost).IsAssignableFrom(base.skillLocator.utility.activationState.stateType) && NetworkServer.active)
            {
                BoostLogic boost = base.characterBody.GetComponent<BoostLogic>();
                if (boost)
                {
                    boost.AddBoost(StaticValues.parryBoostRecharge);
                }
            }
            else
            {
                base.skillLocator.utility.RunRecharge(StaticValues.parryCooldownReduction);
            }
            base.skillLocator.special.RunRecharge(StaticValues.parryCooldownReduction);
        }

        protected virtual void OnSuccessfulParry()
        {

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void SuperParryBlast()
        {
            SphereSearch sphereSearch = new SphereSearch();
            sphereSearch.origin = base.characterBody.transform.position;
            sphereSearch.radius = superParryRange;
            sphereSearch.mask = LayerIndex.entityPrecise.mask;
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(base.teamComponent.teamIndex));
            sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            HurtBox[] hurtBoxes = sphereSearch.GetHurtBoxes();
            foreach (HurtBox hurtBox in hurtBoxes)
            {
                CharacterBody characterBody = hurtBox.healthComponent.body;
                if (characterBody)
                {
                    characterBody.AddTimedBuff(Buffs.superParryDebuff, StaticValues.superParryDebuffDuration);
                }
            }
        }
    }
}