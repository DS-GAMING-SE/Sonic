using EntityStates;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class Parry : BaseSkillState
    {
        public static float minDuration = Modules.StaticValues.parryMinimumDuration;
        public static float baseMaxDuration = Modules.StaticValues.parryMaximumDuration;
        public static float baseSuperMaxDuration = StaticValues.superParryMaxDuration;

        public static float baseEndLag = Modules.StaticValues.parryEndLag;
        public static float baseEndLagFail = Modules.StaticValues.parryFailEndLag;

        public static float enterAnimationPercent = 0.4f;
        public static float endAnimationPercent = 0.4f;

        private float maxDuration;
        
        private float endLagTimer = 0;
        private float endLag;
        private float endLagFail;

        private bool canParry = true;
        private bool parrySuccess;

        private string muzzleString="SwingCenter";
        private Vector3 targetVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            this.maxDuration = base.characterBody.HasBuff(Buffs.superSonicBuff) ? baseSuperMaxDuration : baseMaxDuration;
            this.endLag = baseEndLag / base.characterBody.attackSpeed;
            this.endLagFail = baseEndLagFail / base.characterBody.attackSpeed;
            base.characterMotor.disableAirControlUntilCollision = false;
            base.modelLocator.normalizeToFloor = true;
            base.PlayAnimation("FullBody, Override", "ParryEnter", "Slash.playbackRate", minDuration * enterAnimationPercent);
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }

        public override void OnExit()
        {
            if (base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility) && !parrySuccess && NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.modelLocator.normalizeToFloor = false;
            base.OnExit();
        }


        public void OnTakeDamage(DamageInfo damage)
        {
            Debug.Log("Hit while parrying");
            if (!parrySuccess && canParry && damage.damage>0 && ((damage.damageType & DamageType.DoT) != DamageType.DoT) && ((damage.damageType & DamageType.VoidDeath) != DamageType.VoidDeath) && ((damage.damageType & DamageType.BypassArmor) != DamageType.BypassArmor) && ((damage.damageType & DamageType.BypassBlock) != DamageType.BypassBlock) && ((damage.damageType & DamageType.OutOfBounds) != DamageType.OutOfBounds) && ((damage.damageType & DamageType.FallDamage) != DamageType.FallDamage))
            {
                parrySuccess = true;
                canParry = false;
                if (NetworkServer.active)
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                    base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, StaticValues.parryLingeringInvincibilityDuration, 1);
                    base.characterBody.AddTimedBuff(RoR2Content.Buffs.WarCryBuff, StaticValues.parryBuffDuration, 1);
                }
                Util.PlaySound("Play_parry", base.gameObject);
                this.endLag = baseEndLag / base.characterBody.attackSpeed;
                base.PlayAnimation("FullBody, Override", "ParryRelease", "Slash.playbackRate", endLag * endAnimationPercent);
                RechargeCooldowns();
                Debug.Log("Parry");

            }    
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            targetVelocity = Vector3.zero;
            base.characterMotor.velocity = Vector3.Lerp(base.characterMotor.velocity, targetVelocity, base.fixedAge/baseMaxDuration);

            if (canParry && base.isAuthority && ((base.fixedAge >= minDuration && !base.inputBank.skill2.down) || (base.fixedAge >= maxDuration)))
            {
                canParry = false;
                if (NetworkServer.active)
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
                this.endLagFail = baseEndLagFail / base.characterBody.attackSpeed;
                base.PlayAnimation("FullBody, Override", "ParryRelease", "Slash.playbackRate", endLagFail*endAnimationPercent);
            }
            /*if (base.isAuthority && base.inputBank.skill3.justPressed && base.skillLocator.utility.IsReady()) // Cancel into boost
            {
                base.skillLocator.utility.OnExecute();
                return;
            }
            */
            if (!canParry)
            {
                endLagTimer += Time.fixedDeltaTime;
                if (parrySuccess)
                {
                    if (endLagTimer >= endLag)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
                else
                {
                    if (endLagTimer >= endLagFail)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
            }
        }

        private void RechargeCooldowns()
        {
            base.skillLocator.primary.RunRecharge(StaticValues.parryCooldownReduction);
            BoostLogic boostLogic = GetComponent<BoostLogic>();
            if (base.skillLocator.utility.activationState.stateType == typeof(Boost) && boostLogic)
            {
                if (NetworkServer.active)
                {
                    boostLogic.AddBoost(StaticValues.parryBoostRecharge);
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