using EntityStates;
using IL.RoR2.ConVar;
using Rewired;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperGrandSlamFinal : GrandSlamFinal
    {
        private bool projectileFired = false;
        private Vector3 superProjectilePosition;

        protected override float basePushForce
        {
            get { return 11000f; }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            startUpVelocityMax = 110f;
            startUpVelocityMin = 10f;
    }

        public override void OnExit()
        {
            base.OnExit();
            if (hasFired && !projectileFired && base.isAuthority)
            {
                FireProjectile();
            }
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if (!projectileFired)
            {
                FireProjectile();
            }
        }

        public virtual GameObject GetProjectilePrefab(string skinName)
        {
            switch (skinName)
            {
                case SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME":
                    return Projectiles.superSonicAfterimageRainPrefab;
                case SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME":
                    return Projectiles.superMetalAfterimageRainPrefab;
            }
            return Projectiles.superSonicAfterimageRainPrefab;
        }

        public virtual void FireProjectile()
        {
            if (base.isAuthority)
            {
                projectileFired = true;
                superProjectilePosition = base.characterMotor.transform.position + new Vector3(0, 2.5f, 0);
                GameObject prefab = GetProjectilePrefab(modelLocator.modelTransform.gameObject.GetComponentInChildren<ModelSkinController>().skins[base.characterBody.skinIndex].nameToken);
                RoR2.Projectile.ProjectileManager.instance.FireProjectile(prefab, superProjectilePosition, Util.QuaternionSafeLookRotation(Vector3.down), base.characterBody.gameObject, StaticValues.superGrandSlamDOTDamage * this.damageStat, 0, base.RollCrit());
            }
        }
    }
}