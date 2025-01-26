using EntityStates;
using RoR2;
using RoR2.Skills;
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

        protected ParryFollowUpTracker followUp;
        public static SkillDef followUpSkillDef;

        public override void OnEnter()
        {
            base.OnEnter();
            this.followUp = base.GetComponent<ParryFollowUpTracker>();
            this.endLag = parrySuccess ? baseEndLag / base.characterBody.attackSpeed : baseEndLagFail / base.characterBody.attackSpeed;
            base.characterMotor.disableAirControlUntilCollision = false;
            base.modelLocator.normalizeToFloor = true;
            base.PlayAnimation("FullBody, Override", "ParryRelease", "Slash.playbackRate", endLag * endAnimationPercent);
            if (base.isAuthority)
            {
                EffectManager.SimpleMuzzleFlash(Modules.Assets.parryEffect, base.gameObject, this.muzzleString, true);
            }
            Util.PlaySound("Play_sonicthehedgehog_swing_low", base.gameObject);
            if (parrySuccess)
            {
                OnSuccessfulParry();
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
            if (NetworkServer.active)
            {
                HedgehogUtils.Boost.BoostLogic boost = base.characterBody.GetComponent<HedgehogUtils.Boost.BoostLogic>();
                if (boost && boost.boostExists)
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

        protected virtual void GiveBuffs()
        {
            base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, StaticValues.parryLingeringInvincibilityDuration, 1);
            base.characterBody.AddTimedBuff(Buffs.parryBuff, StaticValues.parryBuffDuration, 1);
        }

        protected virtual void OnParryVFX()
        {
            if (base.isAuthority)
            {
                EffectManager.SimpleMuzzleFlash(Modules.Assets.parryActivateEffect, base.gameObject, this.muzzleString, true);
            }
            Util.PlaySound("Play_sonicthehedgehog_parry", base.gameObject);
        }

        protected virtual void OnSuccessfulParry()
        {
            if (NetworkServer.active)
            {
                GiveBuffs();
            }
            OnParryVFX();
            RechargeCooldowns();
            if (followUp && base.isAuthority)
            {
                FollowUpAttack();
            }
        }

        protected virtual void FollowUpAttack()
        {
            followUp.ReadyFollowUpAttack(skillLocator.secondary.skillDef, followUpSkillDef, StaticValues.parryBuffDuration);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(parrySuccess);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            parrySuccess = reader.ReadBoolean();
        }
    }
}