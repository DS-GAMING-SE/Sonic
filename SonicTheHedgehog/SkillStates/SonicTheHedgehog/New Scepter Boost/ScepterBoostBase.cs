using R2API;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SonicTheHedgehog.SkillStates
{
    public class ScepterBoostBase : HedgehogUtils.Boost.EntityStates.Boost
    {
        protected OverlapAttack attack;
        protected float attackTimer;
        protected static float attacksPerSecond = 15f;

        protected virtual string hitBoxName
        {
            get { return "Ball"; }
        }

        protected virtual float launchForce
        {
            get { return 400f; }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            PrepareOverlapAttack();
            base.characterMotor.onHitGroundAuthority += OnHitGround;
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = true;
            }
            if (airBoosting)
            {
                base.characterMotor.disableAirControlUntilCollision = false;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                attackTimer += Time.fixedDeltaTime;
                if (attackTimer > 1 / attacksPerSecond)
                {
                    attackTimer %= (1 / attacksPerSecond);
                    attack.forceVector = base.characterDirection.forward * (launchForce * 0.8f);
                    attack.isCrit = RollCrit();
                    attack.damage = ((StaticValues.scepterBoostDamageCoefficient * base.characterBody.moveSpeed) / StaticValues.defaultPowerBoostSpeed) * base.characterBody.damage;
                    attack.Fire();
                }
            }
        }

        public override void OnExit()
        {
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = false;
            }
            base.characterMotor.onHitGroundAuthority -= OnHitGround;
            base.OnExit();
        }
        public override void ProcessJump()
        {
            if (base.isAuthority && this.hasCharacterMotor && this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
            {
                base.modelAnimator.SetBool("isBall", true);
            }
            base.ProcessJump();
        }

        private void OnHitGround(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            if (base.modelAnimator.GetBool("isBall"))
            {
                base.modelAnimator.SetBool("isBall", false);
            }
        }

        public override GameObject GetAuraPrefab()
        {
            return Modules.Assets.scepterPowerBoostAuraEffect;
        }
        public override GameObject GetFlashPrefab()
        {
            return Modules.Assets.scepterPowerBoostFlashEffect;
        }

        public override string GetSoundString()
        {
            return "Play_hedgehogutils_strong_boost";
        }

        public virtual void PrepareOverlapAttack()
        {
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitBoxName);
            }

            this.attack = new OverlapAttack();
            this.attack.damageType = DamageType.Generic;
            this.attack.damageType.damageSource = DamageSource.Utility;
            this.attack.damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launch);
            this.attack.pushAwayForce = launchForce * 0.3f;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.procCoefficient = StaticValues.scepterBoostProcCoefficient;
            this.attack.hitEffectPrefab = Modules.Assets.meleeHitEffect;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.impactSound = Modules.Assets.meleeFinalHitSoundEvent.index;
            this.attack.retriggerTimeout = 0.8f;
        }
    }
}
