using EntityStates;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class ParryExit : BaseSkillState
    {
        // Experiment with changing hitbox size

        public static float baseEndLag = Modules.StaticValues.parryEndLag;
        public static float baseEndLagFail = Modules.StaticValues.parryFailEndLag;

        public static float endAnimationPercent = 0.5f;

        private float endLag;

        public bool parrySuccess = false;

        private string muzzleString="SwingCenter";
        private Vector3 targetVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            this.endLag = parrySuccess ? baseEndLag / base.characterBody.attackSpeed : baseEndLagFail / base.characterBody.attackSpeed;
            base.characterMotor.disableAirControlUntilCollision = false;
            base.modelLocator.normalizeToFloor = true;
            base.PlayAnimation("FullBody, Override", "ParryRelease", "Slash.playbackRate", endLag * endAnimationPercent);
            if (parrySuccess)
            {
                if (NetworkServer.active)
                {
                    base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, StaticValues.parryLingeringInvincibilityDuration, 1);
                    base.characterBody.AddTimedBuff(Buffs.parryBuff, StaticValues.parryBuffDuration, 1);
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
            if (base.skillLocator.utility.activationState.stateType == typeof(Boost) && NetworkServer.active)
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

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}