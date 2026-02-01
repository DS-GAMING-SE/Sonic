using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SonicTheHedgehog.Modules
{
    internal static class Projectiles
    {
        internal static Material superProjectileMaterial;
        
        internal static GameObject sonicBoomPrefab;
        internal static GameObject superSonicBoomPrefab;
        internal static GameObject superMeleePunchProjectilePrefab;
        internal static GameObject superMeleeKickProjectilePrefab;
        internal static AssetReferenceT<GameObject> superMetalMeleePunchProjectileGhost;
        internal static AssetReferenceT<GameObject> superMetalMeleeKickProjectileGhost;
        internal static GameObject superSonicAfterimageRainPrefab;
        internal static GameObject superMetalAfterimageRainPrefab;

        internal static void RegisterProjectiles()
        {
            CreateProjectileMaterial();
            
            CreateSonicBoom();
            CreateSuperSonicBoom();
            CreateSuperMeleeProjectile();
            CreateSuperSonicAfterimageRain();

            AddProjectile(sonicBoomPrefab);
            AddProjectile(superSonicBoomPrefab);

            AddProjectile(superMeleePunchProjectilePrefab);
            AddProjectile(superMeleeKickProjectilePrefab);

            CreateSuperSkinMeleeProjectiles();

            AddProjectile(superSonicAfterimageRainPrefab);
            AddProjectile(superMetalAfterimageRainPrefab);
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void CreateProjectileMaterial()
        {
            superProjectileMaterial = new Material(LegacyResourcesAPI.Load<Material>("Materials/matGhostEffect"));
            //material.SetColor("_TintColor", new Color(1, 0.2f, 0f, 1));
            //material.SetColor("_Color", new Color(1, 0.2f, 0f, 1.5f));
            //material.SetColor("_EmissionColor", new Color(1, 0.2f, 0f, 2));

            superProjectileMaterial.SetTexture("_RemapTex", Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texRampSuperProjectile"));

            superProjectileMaterial.SetColor("_TintColor", new Color(1, 0.8f, 0f));

            superProjectileMaterial.SetFloat("_FresnelPower", 1);

            superProjectileMaterial.SetFloat("_AlphaBoost", 2);
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

            sonicBoomPrefab.GetComponent<ProjectileDamage>().damageType.damageSource = DamageSource.Secondary;

            ProjectileInflictTimedBuff debuff = sonicBoomPrefab.AddComponent<ProjectileInflictTimedBuff>();
            debuff.buffDef = Buffs.sonicBoomDebuff;
            debuff.duration = StaticValues.sonicBoomDebuffDuration;

            ProjectileController bombController = sonicBoomPrefab.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SonicBoomGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("SonicBoomGhost");
            bombController.startSound = "";
            bombController.allowPrediction = true;
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
            bombImpactExplosion.impactEffect = Modules.Assets.crossSlashImpactEffect;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("Play_sonic_boom_hit");
            bombImpactExplosion.timerAfterImpact = false;
            bombImpactExplosion.lifetimeAfterImpact = 0f;

            superSonicBoomPrefab.GetComponent<ProjectileDamage>().damageType.damageSource = DamageSource.Secondary;

            ProjectileInflictTimedBuff debuff = superSonicBoomPrefab.AddComponent<ProjectileInflictTimedBuff>();
            debuff.buffDef = Buffs.crossSlashDebuff;
            debuff.duration = StaticValues.sonicBoomDebuffDuration;

            ProjectileController bombController = superSonicBoomPrefab.GetComponent<ProjectileController>();
            bombController.canImpactOnTrigger = false;
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("CrossSlashGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("CrossSlashGhost");
            bombController.startSound = "";
            bombController.allowPrediction = true;
        }

        private static void CreateSuperMeleeProjectile()
        {
            superMeleePunchProjectilePrefab = CloneProjectilePrefab("FMJRamping", "SuperSonicMeleePunchProjectile");

            ProjectileDamage damage = superMeleePunchProjectilePrefab.GetComponent<ProjectileDamage>();
            ProjectileSimple simple = superMeleePunchProjectilePrefab.GetComponent<ProjectileSimple>();
            ProjectileOverlapAttack overlap = superMeleePunchProjectilePrefab.GetComponent<ProjectileOverlapAttack>();

            damage.damage = 1;
            damage.damageType.damageSource = DamageSource.Primary;

            simple.lifetime = 0.5f;
            simple.enableVelocityOverLifetime = true;
            simple.velocityOverLifetime = AnimationCurve.EaseInOut(0, 1, 0.75f, 0.2f);
            simple.lifetimeExpiredEffect = null;

            overlap.overlapProcCoefficient = StaticValues.superMeleeExtraProcCoefficient;
            overlap.impactEffect = null;
            overlap.onServerHit = null; // This removes FMJ (Commano secondary) increase damage on hit so it is just a piercing projectile

            ProjectileController bombController = superMeleePunchProjectilePrefab.GetComponent<ProjectileController>();
            bombController.procCoefficient = StaticValues.superMeleeExtraProcCoefficient;
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SuperMeleePunchGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("SuperMeleePunchGhost", superProjectileMaterial);
            bombController.startSound = "";
            bombController.allowPrediction = true;
            //CreateOtherSuperMeleeProjectiles();
            superMeleeKickProjectilePrefab = CreateOtherSuperMeleeProjectile("SuperMeleeKickGhost", "SuperSonicMeleeKickProjectile");
        }

        //I don't think the prefabName actually matters for anything
        public static GameObject CreateOtherSuperMeleeProjectile(string ghostPrefabName, string prefabName)
        {
            GameObject prefab = PrefabAPI.InstantiateClone(superMeleePunchProjectilePrefab, prefabName);
            ProjectileController controller = prefab.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostPrefabName) != null) controller.ghostPrefab = CreateGhostPrefab(ghostPrefabName, superProjectileMaterial);
            return prefab;
        }
        public static void CreateSuperSkinMeleeProjectiles()
        {
            superMetalMeleePunchProjectileGhost = new AssetReferenceT<GameObject>("b40ee4b6b2e1e7543a63e8cd55767e1c");
            AssetAsyncReferenceManager<GameObject>.LoadAsset(superMetalMeleePunchProjectileGhost).Completed += (x) => CreateGhostPrefab(x.Result, superProjectileMaterial);
            superMetalMeleeKickProjectileGhost = new AssetReferenceT<GameObject>("da412dfe504119e4f92125d057f34378");
            AssetAsyncReferenceManager<GameObject>.LoadAsset(superMetalMeleeKickProjectileGhost).Completed += (x) => CreateGhostPrefab(x.Result, superProjectileMaterial);
        }

        private static void CreateSuperSonicAfterimageRain()
        {
            superSonicAfterimageRainPrefab = PrefabAPI.InstantiateClone(Assets.mainAssetBundle.LoadAsset<GameObject>("SonicSuperAfterimageRainBase"),"SuperAfterimageRainProjectile");
            if (superSonicAfterimageRainPrefab)
            {
                Log.Message("the risk of rain is real");
            }

            NetworkIdentity network = superSonicAfterimageRainPrefab.AddComponent<NetworkIdentity>();
            network.localPlayerAuthority = true;

            superSonicAfterimageRainPrefab.AddComponent<TeamFilter>();

            ProjectileController controller = superSonicAfterimageRainPrefab.AddComponent<ProjectileController>();
            controller.cannotBeDeleted = true;
            //controller.flightSoundLoop = Assets.superGrandSlamLoopSoundDef;

            Log.Message("Afterimage Rain hitboxes");
            HitBoxGroup hitboxes = superSonicAfterimageRainPrefab.AddComponent<HitBoxGroup>();

            HitBox hitBox = superSonicAfterimageRainPrefab.transform.Find("Hitboxes/Hitbox0").gameObject.AddComponent<HitBox>();
            superSonicAfterimageRainPrefab.transform.Find("Hitboxes/Hitbox0").gameObject.layer = LayerIndex.projectile.intVal;

            HitBox hitBox1 = superSonicAfterimageRainPrefab.transform.Find("Hitboxes/Hitbox1").gameObject.AddComponent<HitBox>();
            superSonicAfterimageRainPrefab.transform.Find("Hitboxes/Hitbox1").gameObject.layer = LayerIndex.projectile.intVal;

            hitboxes.hitBoxes = new HitBox[] { hitBox, hitBox1 }; // make hitboxes on the projectile like the melees. Rex uses two boxes rotated 45 degrees to kinda get a cylinder. Rex size is about 10

            Log.Message("Afterimage Rain mat swap");
            Assets.MaterialSwap(superSonicAfterimageRainPrefab, RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Croco.matCrocoSlashDistortion_mat, "Effects/Blur");
            Assets.MaterialSwap(superSonicAfterimageRainPrefab, superProjectileMaterial, "Effects/Sonics");

            Log.Message("Afterimage Rain damage");
            ProjectileDamage damage = superSonicAfterimageRainPrefab.AddComponent<ProjectileDamage>();
            damage.damage = StaticValues.superGrandSlamDOTDamage;
            damage.damageType.damageSource = DamageSource.Special;

            ProjectileDotZone dot = superSonicAfterimageRainPrefab.AddComponent<ProjectileDotZone>();
            dot.damageCoefficient = 1;
            dot.overlapProcCoefficient = StaticValues.superGrandSlamDOTProcCoefficient;
            dot.resetFrequency = 3;
            dot.fireFrequency = 6;
            dot.lifetime = StaticValues.superGrandSlamDOTLifetime;
            dot.forceVector = Vector3.down * 50;
            dot.soundLoopStopString = Assets.superGrandSlamLoopSoundDef.stopSoundName;
            dot.soundLoopString = Assets.superGrandSlamLoopSoundDef.startSoundName;


            superMetalAfterimageRainPrefab = CreateOtherAfterimageRain(Assets.mainAssetBundle.LoadAsset<GameObject>("MetalSonicAfterimageMesh").GetComponent<MeshFilter>().sharedMesh, "SuperMetalAfterimageRainProjectile");

            /*Old Super Grand Slam, still uses Rex visuals
             * 
             * superSonicAfterimageRainPrefab = CloneProjectilePrefab("TreebotMortarRain", "SuperSonicAfterimageRainProjectile");
            //Vector3 scale = superSonicAfterimageRainPrefab.transform.localScale * 5;
            //scale.y *= 2;
            //superSonicAfterimageRainPrefab.transform.localScale = scale;

            ProjectileDotZone dot = superSonicAfterimageRainPrefab.GetComponent<ProjectileDotZone>();
            dot.overlapProcCoefficient = StaticValues.superGrandSlamDOTProcCoefficient;
            dot.resetFrequency = 3;
            dot.lifetime = StaticValues.superGrandSlamDOTLifetime;
            dot.forceVector = Vector3.down * 5;

            ProjectileDamage damage = superSonicAfterimageRainPrefab.GetComponent<ProjectileDamage>();

            damage.damage = StaticValues.superGrandSlamDOTDamage;
            damage.force = 10;

            ProjectileController rainController = superSonicAfterimageRainPrefab.GetComponent<ProjectileController>();
            rainController.cannotBeDeleted = true;
            //if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SuperMeleePunchGhost") != null) rainController.ghostPrefab = CreateGhostPrefab("SuperMeleePunchGhost");
            rainController.startSound = "";
            */
        }

        // This replaces the falling Sonics mesh with whatever mesh you want
        // The mesh should not have an armature, it should just be a normal mesh in the pose. Apply the armature modifier in blender to get rid of the armature and have just the posed mesh
        // Also please use and apply decimate modifier to keep the tri count low. These should be super low detail, they won't even have their materials. Mine are like 1/5 tris of normal
        // If you have issues with the mesh not showing up at all, try checking Read/Write Enabled in your mesh import. It worked for me, though I'm not sure why
        // I don't think prefab name actually matters
        public static GameObject CreateOtherAfterimageRain(Mesh mesh, string prefabName)
        {
            GameObject prefab = PrefabAPI.InstantiateClone(superSonicAfterimageRainPrefab, prefabName);

            prefab.transform.Find("Effects/Sonics").gameObject.GetComponent<ParticleSystemRenderer>().mesh = mesh;

            return prefab;
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
            ghostPrefab.AddComponent<VFXAttributes>().DoNotPool = true;

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }
        private static GameObject CreateGhostPrefab(GameObject ghostPrefab, Material superGhostMaterial)
        {
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            ghostPrefab.GetComponentInChildren<Renderer>().material = superGhostMaterial;

            return ghostPrefab;
        }

        private static GameObject CreateGhostPrefab(string ghostName, Material superGhostMaterial)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            ghostPrefab.GetComponentInChildren<Renderer>().material = superGhostMaterial;

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}