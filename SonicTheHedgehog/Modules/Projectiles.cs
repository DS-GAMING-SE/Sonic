using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.Modules
{
    internal static class Projectiles
    {
        internal static GameObject sonicBoomPrefab;
        internal static GameObject superSonicBoomPrefab;
        internal static GameObject superMeleeProjectilePrefab;
        internal static GameObject superSonicAfterimageRainPrefab;

        internal static void RegisterProjectiles()
        {
            CreateSonicBoom();
            CreateSuperSonicBoom();
            CreateSuperMeleeProjectile();
            CreateSuperSonicAfterimageRain();

            AddProjectile(sonicBoomPrefab);
            AddProjectile(superSonicBoomPrefab);
            AddProjectile(superMeleeProjectilePrefab);
            AddProjectile(superSonicAfterimageRainPrefab);
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void CreateSonicBoom()
        {
            sonicBoomPrefab = CloneProjectilePrefab("LemurianBigFireball", "SonicBoomProjectile");
            sonicBoomPrefab.transform.localScale *= 0.5f;

            ProjectileImpactExplosion bombImpactExplosion = sonicBoomPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(bombImpactExplosion);

            bombImpactExplosion.blastRadius = 4f;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.destroyOnWorld = true;
            bombImpactExplosion.lifetime = 1;
            bombImpactExplosion.impactEffect = Modules.Assets.sonicBoomImpactEffect;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("Play_sonic_boom_hit");
            bombImpactExplosion.timerAfterImpact = false;
            bombImpactExplosion.lifetimeAfterImpact = 0f;

            ProjectileController bombController = sonicBoomPrefab.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SonicBoomGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("SonicBoomGhost");
            bombController.startSound = "";
        }

        private static void CreateSuperSonicBoom()
        {
            superSonicBoomPrefab = CloneProjectilePrefab("LemurianBigFireball", "SuperSonicBoomProjectile");
            superSonicBoomPrefab.transform.localScale *= 0.8f;

            ProjectileImpactExplosion bombImpactExplosion = superSonicBoomPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(bombImpactExplosion);

            bombImpactExplosion.blastRadius = 6f;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.destroyOnWorld = true;
            bombImpactExplosion.lifetime = 1.3f;
            bombImpactExplosion.impactEffect = Modules.Assets.sonicBoomImpactEffect;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("Play_sonic_boom_hit");
            bombImpactExplosion.timerAfterImpact = false;
            bombImpactExplosion.lifetimeAfterImpact = 0f;

            ProjectileController bombController = superSonicBoomPrefab.GetComponent<ProjectileController>();
            bombController.canImpactOnTrigger = false;
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("CrossSlashGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("CrossSlashGhost");
            bombController.startSound = "";
        }

        private static void CreateSuperMeleeProjectile()
        {
            superMeleeProjectilePrefab = CloneProjectilePrefab("FMJ", "SuperSonicMeleeProjectile");

            ProjectileDamage damage = superMeleeProjectilePrefab.GetComponent<ProjectileDamage>();
            ProjectileSimple simple = superMeleeProjectilePrefab.GetComponent<ProjectileSimple>();

            damage.damage = 1;

            simple.lifetime = 0.5f;
            simple.enableVelocityOverLifetime = true;
            simple.velocityOverLifetime = AnimationCurve.EaseInOut(0, 1, 0.75f, 0.2f);

            /*impactExplosion.blastProcCoefficient = StaticValues.superMeleeExtraProcCoefficient;
            impactExplosion.destroyOnEnemy = false;
            impactExplosion.destroyOnWorld = false;
            impactExplosion.lifetime = 0.7f;
            impactExplosion.impactOnWorld = false;
            impactExplosion.impactEffect = Modules.Assets.sonicBoomImpactEffect;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("Play_sonic_boom_hit");
            impactExplosion.timerAfterImpact = false;
            impactExplosion.lifetimeAfterImpact = 0f;
            */

            ProjectileController bombController = superMeleeProjectilePrefab.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SonicBoomGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("SonicBoomGhost");
            bombController.startSound = "";
        }

        private static void CreateSuperSonicAfterimageRain()
        {
            superSonicAfterimageRainPrefab = CloneProjectilePrefab("TreebotMortarRain", "SuperSonicAfterimageRainProjectile");
            Vector3 scale = superSonicAfterimageRainPrefab.transform.localScale * 5;
            scale.y *= 2;
            superSonicAfterimageRainPrefab.transform.localScale = scale;

            ProjectileDotZone dot = superSonicAfterimageRainPrefab.GetComponent<ProjectileDotZone>();
            dot.overlapProcCoefficient = StaticValues.superGrandSlamDOTProcCoefficient;
            dot.resetFrequency = 3;
            dot.lifetime = StaticValues.superGrandSlamDOTLifetime;
            dot.forceVector = Vector3.down;

            ProjectileDamage damage = superSonicAfterimageRainPrefab.GetComponent<ProjectileDamage>();

            damage.damage = StaticValues.superGrandSlamDOTDamage;
            damage.force = 10;

            ProjectileController rainController = superSonicAfterimageRainPrefab.GetComponent<ProjectileController>();
            rainController.cannotBeDeleted = true;
            //if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SonicBoomGhost") != null) rainController.ghostPrefab = CreateGhostPrefab("SonicBoomGhost");
            rainController.startSound = "";
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1;
            projectileImpactExplosion.blastProcCoefficient = StaticValues.sonicBoomProcCoefficient;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;
            //projectileImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("Play_sonic_boom_explode");

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}