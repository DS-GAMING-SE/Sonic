using RoR2;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using System;
using UnityEngine;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperSonicMelee : SonicMelee
    {
        protected override Type enterStateType
        {
            get { return typeof(SuperSonicMeleeEnter); }
        }

        protected override float launchPushForce
        {
            get { return 1000f; }
        }

        protected override void OnFireAuthority()
        {
            FireSuperProjectile();
        }

        private void FireSuperProjectile()
        {
            Vector3 origin = base.GetAimRay().origin;
            origin -= base.GetAimRay().direction * 8;
            Vector3 forward = base.characterDirection.forward;
            if (swingIndex % 2 == 0)
            {
                origin += Vector3.Cross(forward, Vector3.up) * 3;

            }
            else
            {
                origin -= Vector3.Cross(forward, Vector3.up) * 3;
            }

            string skinName = base.modelLocator.modelTransform.gameObject.GetComponentInChildren<ModelSkinController>().skins[base.characterBody.skinIndex].nameToken;

            RoR2.Projectile.ProjectileManager.instance.FireProjectile(GetSuperProjectile(skinName), origin, Util.QuaternionSafeLookRotation(base.GetAimRay().direction), 
                base.gameObject, this.damageCoefficient * StaticValues.superMeleeExtraDamagePercent * this.damageStat, 0, 
                Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, 120);
        }

        public virtual GameObject GetSuperProjectile(string skinName)
        {
            if (swingIndex%2 == 0 && swingIndex != 4)
            {
                switch (skinName)
                {
                    default:
                        return Projectiles.superMeleePunchProjectilePrefab;
                    case SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME":
                        return Projectiles.superMetalMeleePunchProjectilePrefab;
                }
            }
            else
            {
                switch (skinName)
                {
                    default:
                        return Projectiles.superMeleeKickProjectilePrefab;
                    case SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME":
                        return Projectiles.superMetalMeleeKickProjectilePrefab;
                }
            }
        }
    }
}