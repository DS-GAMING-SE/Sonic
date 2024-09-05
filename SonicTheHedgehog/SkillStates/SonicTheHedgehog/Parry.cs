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

        public static float baseEnterAnimationPercent = 1f;
        public static float superEnterAnimationPercent = 0.8f;
        protected float enterAnimationPercent;

        protected float maxDuration;

        private bool canParry = true;
        private bool parrySuccess;

        private string muzzleString="SwingCenter";
        private Vector3 targetVelocity;

        private CapsuleCollider collider;
        private float originalHeight;
        private float originalRadius;

        public override void OnEnter()
        {
            base.OnEnter();
            this.maxDuration = baseMaxDuration;
            base.characterMotor.disableAirControlUntilCollision = false;
            base.modelLocator.normalizeToFloor = true;
            this.enterAnimationPercent = baseEnterAnimationPercent;
            base.PlayAnimation("FullBody, Override", "ParryEnter", "Slash.playbackRate", minDuration * enterAnimationPercent);
            Util.PlaySound("Play_swing", base.gameObject);
            this.collider = (CapsuleCollider)base.characterBody.mainHurtBox.collider;
            this.originalHeight = collider.height;
            this.originalRadius = collider.radius;
            this.collider.radius = this.originalRadius * 4f;
            this.collider.height = this.originalHeight * 2.5f;
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.modelLocator.normalizeToFloor = false;
            this.collider.radius = this.originalRadius;
            this.collider.height = this.originalHeight;
            base.OnExit();
        }


        public void OnTakeDamage(DamageInfo damage)
        {
            Log.Message("Hit while parrying");
            if (!parrySuccess && canParry && damage.damage>0 && !damage.damageType.damageType.HasFlag(DamageType.DoT)
                && !damage.damageType.damageType.HasFlag(DamageType.VoidDeath)
                && !damage.damageType.damageType.HasFlag(DamageType.BypassArmor)
                && !damage.damageType.damageType.HasFlag(DamageType.BypassBlock)
                && !damage.damageType.damageType.HasFlag(DamageType.OutOfBounds)
                && !damage.damageType.damageType.HasFlag(DamageType.FallDamage))
            {
                parrySuccess = true;
                canParry = false;
                Log.Message("Parry");
                SetNextState(true);
            }    
        }

        protected virtual void SetNextState(bool success)
        {
            this.outer.SetNextState(new ParryExit
            {
                parrySuccess = success
            });
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            targetVelocity = Vector3.zero;
            base.characterMotor.velocity = Vector3.Lerp(base.characterMotor.velocity, targetVelocity, base.fixedAge/baseMaxDuration);

            if (canParry && base.isAuthority && ((base.fixedAge >= minDuration && !base.inputBank.skill2.down) || (base.fixedAge >= maxDuration)))
            {
                canParry = false;
                SetNextState(false);
            }
            /*if (base.isAuthority && base.inputBank.skill3.justPressed && base.skillLocator.utility.IsReady()) // Cancel into boost
            {
                base.skillLocator.utility.OnExecute();
                return;
            }
            */
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}