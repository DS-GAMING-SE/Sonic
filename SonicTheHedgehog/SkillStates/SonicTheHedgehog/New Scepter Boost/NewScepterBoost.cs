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
    public class NewScepterBoost : NewBoost
    {
        protected OverlapAttack attack;
        protected float attackTimer;
        protected static float attacksPerSecond = 10f;

        protected virtual string hitBoxName
        {
            get { return "Ball"; }
        }

        protected virtual float launchForce
        {
            get { return 250f; }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            PrepareOverlapAttack();
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
                    attack.Fire();
                }
            }
        }

        public override GameObject GetAuraPrefab()
        {
            if (((PowerBoostLogic)boostLogic).powerBoosting)
            {
                return Modules.Assets.scepterPowerBoostAuraEffect;
            }
            return null;
        }
        public override GameObject GetFlashPrefab()
        {
            if (((PowerBoostLogic)boostLogic).powerBoosting)
            {
                return Modules.Assets.scepterPowerBoostFlashEffect;
            }
            return Modules.Assets.scepterBoostFlashEffect;
        }

        public override string GetSoundString()
        {
            return "Play_hedgehogutils_strong_boost";
        }

        public void PrepareOverlapAttack()
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
            this.attack.damage = ((StaticValues.scepterBoostDamageCoefficient * base.characterBody.moveSpeed) / StaticValues.defaultPowerBoostSpeed) * base.characterBody.damage;
            this.attack.procCoefficient = StaticValues.scepterBoostProcCoefficient;
            this.attack.hitEffectPrefab = Modules.Assets.meleeHitEffect;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.impactSound = Modules.Assets.meleeFinalHitSoundEvent.index;
            this.attack.retriggerTimeout = 0.8f;
        }
    }
}
