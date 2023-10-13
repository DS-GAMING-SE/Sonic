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
        // Experiment with changing hitbox size
        
        public static float minDuration = Modules.StaticValues.parryMinimumDuration;
        public static float baseMaxDuration = Modules.StaticValues.parryMaximumDuration;
        public static float baseSuperMaxDuration = StaticValues.superParryMaxDuration;

        public static float enterAnimationPercent = 0.4f;

        private float maxDuration;
        
        private float endLagTimer = 0;
        private float endLag;
        private float endLagFail;

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
            this.maxDuration = base.characterBody.HasBuff(Buffs.superSonicBuff) ? baseSuperMaxDuration : baseMaxDuration;
            base.characterMotor.disableAirControlUntilCollision = false;
            base.modelLocator.normalizeToFloor = true;
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
            Debug.Log("Hit while parrying");
            if (!parrySuccess && canParry && damage.damage>0 && ((damage.damageType & DamageType.DoT) != DamageType.DoT) && ((damage.damageType & DamageType.VoidDeath) != DamageType.VoidDeath) && ((damage.damageType & DamageType.BypassArmor) != DamageType.BypassArmor) && ((damage.damageType & DamageType.BypassBlock) != DamageType.BypassBlock) && ((damage.damageType & DamageType.OutOfBounds) != DamageType.OutOfBounds) && ((damage.damageType & DamageType.FallDamage) != DamageType.FallDamage))
            {
                parrySuccess = true;
                canParry = false;
                Debug.Log("Parry");
                this.outer.SetNextState(new ParryExit
                {
                    parrySuccess = true
                });

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
                this.outer.SetNextState(new ParryExit());
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