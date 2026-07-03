using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using System.Collections.Generic;
using RoR2.UI;
using System;
using SonicTheHedgehog.Components;
using RoR2.Audio;
using UnityEngine.AddressableAssets;
using SonicTheHedgehog.SkillStates;
using SonicTheHedgehog.Modules.Survivors;
using ThreeEyedGames;
using RoR2.ContentManagement;

namespace SonicTheHedgehog.Modules
{
    public static class Assets
    {
        #region Sonic's stuff
        // meshes
        public static Mesh sonicMesh;
        public static Mesh superSonicMesh;
        public static Mesh metalSonicMesh;

        // particle effects
        public static GameObject sonicBoomKickEffect;
        public static GameObject homingAttackTrailEffect;
        public static GameObject sonicBoomImpactEffect;
        public static GameObject crossSlashImpactEffect;

        public static GameObject meleeHitEffect;
        public static GameObject meleeImpactEffect;
        public static GameObject homingAttackLaunchEffect;
        public static GameObject homingAttackHitEffect;

        public static GameObject parryEffect;
        public static GameObject parryActivateEffect;
        public static GameObject followUpKickEffect;
        public static GameObject idwAttackEffect;

        public static GameObject superSonicBlurEffect;

        public static GameObject powerBoostFlashEffect;
        public static GameObject scepterPowerBoostFlashEffect;
        public static GameObject boostFlashEffect;
        public static GameObject boostAuraEffect;
        public static GameObject scepterBoostFlashEffect;
        public static GameObject scepterBoostAuraEffect;
        public static GameObject superBoostFlashEffect;
        public static GameObject scepterSuperBoostFlashEffect;

        public static GameObject powerBoostAuraEffect;
        public static GameObject superBoostAuraEffect;
        public static GameObject scepterPowerBoostAuraEffect;
        public static GameObject scepterSuperBoostAuraEffect;

        public static GameObject grandSlamHitEffect;

        public static GameObject cyloopTrail;
        public static GameObject cyloopTrailSpawningEffect;

        // initial
        public static GameObject faceplantDecal;

        // hud
        public static GameObject powerBoostHud;

        // materials

        internal static Material cyloopOverlay;

        // networked hit sounds
        internal static NetworkSoundEventDef meleeHitSoundEvent;
        internal static NetworkSoundEventDef meleeFinalHitSoundEvent;
        internal static NetworkSoundEventDef homingHitSoundEvent;
        internal static NetworkSoundEventDef grandSlamHitSoundEvent;

        internal static LoopSoundDef superGrandSlamLoopSoundDef;

        #endregion

        // the assetbundle to load assets from
        public static AssetBundle mainAssetBundle;
        public static string AddressablesDirectory { get; private set; }

        // CHANGE THIS
        private const string assetbundleName = "sonicthehedgehogassetbundle";
        //change this to your project's name if/when you've renamed it
        private const string csProjName = "SonicTheHedgehog";
        private const string dllName = "SonicTheHedgehog.dll";

        internal static void Initialize()
        {
            if (assetbundleName == "myassetbundle")
            {
                Log.Error("AssetBundle name hasn't been changed. not loading any assets to avoid conflicts");
                return;
            }

            LoadAssetBundle();
            LoadSoundbank();
            PopulateAssets();
        }

        /*internal static void LoadAssetBundle() For embedded resources
        {
            try
            {
                if (mainAssetBundle == null)
                {
                    using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{csProjName}.{assetbundleName}"))
                    {
                        mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Failed to load assetbundle. Make sure your assetbundle name is setup correctly\n" + e);
                return;
            }
        }*/
        internal static void LoadAssetBundle()
        {
            try
            {
                if (mainAssetBundle == null)
                {
                    // This catalog shit is an enigma
                    mainAssetBundle = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace(dllName, assetbundleName));
                    AddressablesDirectory = System.IO.Path.GetDirectoryName(SonicTheHedgehogPlugin.instance.Info.Location);
                    Addressables.LoadContentCatalogAsync(System.IO.Path.Combine(AddressablesDirectory, "catalog.json")).WaitForCompletion();
                }
            }
            catch (Exception e)
            {
                Log.Error("Failed to load assetbundle. Make sure your assetbundle name is setup correctly\n" + e);
                return;
            }
        }

        internal static void LoadSoundbank()
        {
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{csProjName}.SonicBank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }

        internal static void PopulateAssets()
        {
            if (!mainAssetBundle)
            {
                Log.Error("There is no AssetBundle to load assets from.");
                return;
            }

            // feel free to delete everything in here and load in your own assets instead
            // it should work fine even if left as is- even if the assets aren't in the bundle

            //swordHitSoundEvent = CreateNetworkSoundEventDef("HenrySwordHit");

            //bombExplosionEffect = LoadEffect("BombExplosionEffect", "HenryBombExplosion");

            sonicMesh = Assets.mainAssetBundle.LoadAsset<GameObject>("SonicMesh").GetComponent<SkinnedMeshRenderer>().sharedMesh;
            superSonicMesh = Assets.mainAssetBundle.LoadAsset<GameObject>("SuperSonicMesh").GetComponent<SkinnedMeshRenderer>().sharedMesh;
            metalSonicMesh = Assets.mainAssetBundle.LoadAsset<GameObject>("MetalSonicMesh").GetComponent<SkinnedMeshRenderer>().sharedMesh;

            sonicBoomKickEffect = Assets.LoadEffect("SonicSonicBoomKick", "", true, 1f);
            homingAttackTrailEffect = Assets.LoadEffect("SonicHomingAttack", true);

            sonicBoomImpactEffect = Assets.LoadEffect("SonicSonicBoomImpact", "", false, 0.5f);
            if (sonicBoomImpactEffect)
            {
                sonicBoomImpactEffect.AddComponent<SoundOnStart>().soundString = "Play_sonicthehedgehog_sonic_boom_explode";
            }
            crossSlashImpactEffect = Assets.LoadEffect("SonicCrossSlashImpact", "", false, 0.5f);
            if (crossSlashImpactEffect)
            {
                crossSlashImpactEffect.AddComponent<SoundOnStart>().soundString = "Play_sonicthehedgehog_sonic_boom_explode";
            }

            meleeHitEffect = Assets.LoadEffect("SonicMeleeHit", "", true, 0.25f);
            meleeImpactEffect = Assets.LoadEffect("SonicMeleeImpact", "", false, 0.2f);
            homingAttackLaunchEffect = Assets.LoadEffect("SonicHomingAttackLaunch");
            homingAttackHitEffect = Assets.LoadEffect("SonicHomingAttackHit", "", true, 0.3f);

            parryEffect = Assets.LoadEffect("SonicParry", "", true, 0.15f);
            parryActivateEffect = Assets.LoadEffect("SonicParryActivate", "", true, 0.5f);
            followUpKickEffect = Assets.LoadEffect("SonicFollowUpKick", "", true, 0.35f);
            idwAttackEffect = MaterialSwap(Assets.LoadAsyncedEffect("SonicIDWAttack"), RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Croco.matCrocoSlashDistortion_mat, "Blur/Distortion");

            superSonicBlurEffect = Assets.LoadEffect("SonicSuperBlur", true);

            Material scepterElectricMat = new Material(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.matPrimeDevastatorChargeVFX2_mat).WaitForCompletion());
            scepterElectricMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTeslaCoil_png).WaitForCompletion());
            scepterElectricMat.SetColor("_TintColor", new Color(3.5f, 1f, 4f));
            mainAssetBundle.LoadAsset<GameObject>("ScepterBoostElectricEffect").GetComponent<ParticleSystemRenderer>().trailMaterial = scepterElectricMat;
            powerBoostFlashEffect = HedgehogUtils.Assets.CreateBoostFlashEffect("SonicPowerBoostFlash", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.texChefSecondaryFlameVFX1_png).WaitForCompletion(), SonicTheHedgehogCharacter.sonicColor2);
            powerBoostAuraEffect = HedgehogUtils.Assets.CreateBoostAuraEffect("SonicPowerBoostAura", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.texChefSecondaryFlameVFX1_png).WaitForCompletion(), SonicTheHedgehogCharacter.sonicColor2);

            scepterPowerBoostFlashEffect = HedgehogUtils.Assets.CreateBoostFlashEffect("SonicScepterPowerBoostFlash", new Color(0.15f, 0.15f, 1f));
            AddScepterToBoostFlash(scepterPowerBoostFlashEffect);
            if (scepterPowerBoostFlashEffect)
            {
                ShakeEmitter shakeEmitter = scepterPowerBoostFlashEffect.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.3f;
                shakeEmitter.radius = 50f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.3f,
                    frequency = 40f,
                    cycleOffset = 0f
                };
            }
            scepterPowerBoostAuraEffect = HedgehogUtils.Assets.CreateBoostAuraEffect("SonicScepterPowerBoostAura", new Color(0.15f, 0.15f, 1f));

            Color windColor = new Color(0.07f, 0.07f, 0.07f);
            scepterBoostFlashEffect = HedgehogUtils.Assets.CreateBoostFlashEffect("SonicScepterBoostFlash", null, windColor, Color.magenta, false, 0.9f);
            AddScepterToBoostFlash(scepterBoostFlashEffect);
            if (scepterBoostFlashEffect)
            {
                ShakeEmitter shakeEmitter = scepterPowerBoostFlashEffect.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.2f;
                shakeEmitter.radius = 35f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.08f,
                    frequency = 40f,
                    cycleOffset = 0f
                };
            }
            scepterBoostAuraEffect = HedgehogUtils.Assets.CreateBoostAuraEffect("SonicBoostAura", null, windColor, Color.black, Color.black, 0.9f);


            boostFlashEffect = HedgehogUtils.Assets.CreateBoostFlashEffect("SonicBoostFlash", null, windColor, Color.black, false, 0.9f);
            boostAuraEffect = HedgehogUtils.Assets.CreateBoostAuraEffect("SonicBoostAura", null, windColor, Color.black, Color.black, 0.9f);

            superBoostFlashEffect = HedgehogUtils.Assets.CreateBoostFlashEffect("SonicSuperBoostFlash", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDroneFire_png).WaitForCompletion(), Color.white, SonicTheHedgehogCharacter.superSonicColor, true, 1.25f);
            superBoostAuraEffect = HedgehogUtils.Assets.CreateBoostAuraEffect("SonicSuperBoostAura", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDroneFire_png).WaitForCompletion(), Color.white, SonicTheHedgehogCharacter.superSonicColor, new Color(1f, 0.8f, 0.6f), 1.25f);

            scepterSuperBoostFlashEffect = HedgehogUtils.Assets.CreateBoostFlashEffect("SonicScepterSuperBoostFlash", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDroneFire_png).WaitForCompletion(), Color.white, SonicTheHedgehogCharacter.superSonicColor2, true, 1.25f);
            AddScepterToBoostFlash(scepterSuperBoostFlashEffect);
            if (scepterSuperBoostFlashEffect)
            {
                ShakeEmitter shakeEmitter = scepterPowerBoostFlashEffect.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.2f;
                shakeEmitter.radius = 60f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.25f,
                    frequency = 25f,
                    cycleOffset = 0f
                };
            }
            scepterSuperBoostAuraEffect = HedgehogUtils.Assets.CreateBoostAuraEffect("SonicScepterSuperBoostAura", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDroneFire_png).WaitForCompletion(), Color.white, SonicTheHedgehogCharacter.superSonicColor2, new Color(1f, 0.4f, 0.8f), 1.25f);

            grandSlamHitEffect = Assets.LoadEffect("SonicGrandSlamKickHit", "", true, 0.7f);

            meleeHitSoundEvent = CreateNetworkSoundEventDef("Play_sonicthehedgehog_melee_hit");
            meleeFinalHitSoundEvent = CreateNetworkSoundEventDef("Play_sonicthehedgehog_melee_hit_final");
            homingHitSoundEvent = CreateNetworkSoundEventDef("Play_sonicthehedgehog_homing_impact");
            grandSlamHitSoundEvent = CreateNetworkSoundEventDef("Play_sonicthehedgehog_strong_impact");

            superGrandSlamLoopSoundDef = ScriptableObject.CreateInstance<LoopSoundDef>();
            superGrandSlamLoopSoundDef.startSoundName = "Play_sonicthehedgehog_super_grand_slam_loop";
            superGrandSlamLoopSoundDef.stopSoundName = "Stop_sonicthehedgehog_super_grand_slam_loop";

            powerBoostHud = Assets.mainAssetBundle.LoadAsset<GameObject>("PowerParticles");
            powerBoostHud.AddComponent<PowerBoostHUD>();

            faceplantDecal = PrefabAPI.CreateEmptyPrefab("SonicFaceplantDecal", false);
            faceplantDecal.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
            Decal faceplantDecalComponent = faceplantDecal.AddComponent<Decal>();
            faceplantDecalComponent.Fade = 1f;
            faceplantDecalComponent.DrawAlbedo = true;
            faceplantDecalComponent.DrawNormalAndGloss = true;
            AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_BeetleGroup.matBeetleGuardSlamDecal_mat)).Completed += (x) =>
            {
                faceplantDecalComponent.Material = x.Result;
            };

            cyloopTrail = LoadEffect("SonicCyloopTrail", "", false, -1f, false);
            Material cyberTrailMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_VoidSurvivor.matVoidSurvivorBlasterTrail_mat)).WaitForCompletion());
            cyberTrailMat.SetFloat("_InvFade", 0f);
            cyberTrailMat.SetFloat("_Boost", 1f);
            cyberTrailMat.SetFloat("_AlphaBoost", 2f);
            cyberTrailMat.SetTexture("_RampTex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2.texRampTritoneHShrine_png).WaitForCompletion());
            cyberTrailMat.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_TiledTextures.texCloudPixel2_png).WaitForCompletion());
            cyberTrailMat.SetTextureScale("_MainTex", new Vector2(0.1f, 0.4f));
            cyberTrailMat.EnableKeyword("VERTEXCOLOR");
            cyberTrailMat.SetColor("_TintColor", Color.white);
            cyloopTrail.transform.GetComponent<LineRenderer>().sharedMaterial = cyberTrailMat;

            cyloopTrailSpawningEffect = LoadEffect("SonicCyloopTrailSpawningEffect", "", false, -1f, false);
            Material cyberLinesMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.matNanoSeekerPixelTrail_1_mat)).WaitForCompletion());
            cyberLinesMat.SetTexture("_MainTex", mainAssetBundle.LoadAsset<Texture>("texCyberLines"));
            cyberLinesMat.SetFloat("_Boost", 5f);
            cyberLinesMat.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.texPrimeDevastatorTetherMask2_png).WaitForCompletion());
            cyberLinesMat.SetVector("_CutoffScroll", new Vector4(20f, 0, 0, 0));
            cyloopTrailSpawningEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberLinesMat;
            Material cyberPixelMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.matDT_ShieldPixelBurst50_mat)).WaitForCompletion());
            cyberPixelMat.DisableKeyword("DISABLEREMAP");
            cyberPixelMat.EnableKeyword("VERTEXCOLOR");
            cyberPixelMat.SetFloat("_Boost", 3f);
            cyberPixelMat.SetColor("_TintColor", Color.white);
            cyberPixelMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.texDroneTechRamp_png).WaitForCompletion());
            cyloopTrailSpawningEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberPixelMat;
            var cyloopSpawnDestroy = cyloopTrailSpawningEffect.AddComponent<DisableParticleEmissionAndDestroyOnTimer>();
            cyloopSpawnDestroy.waitDuration = 1.7f;
            cyloopSpawnDestroy.particleSystems = new List<ParticleSystem> { cyloopTrailSpawningEffect.transform.GetChild(0).GetComponent<ParticleSystem>(), cyloopTrailSpawningEffect.transform.GetChild(1).GetComponent<ParticleSystem>() };

            cyloopOverlay = new Material(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_LunarGolem.matLunarGolemShield_mat).WaitForCompletion());
            cyloopOverlay.SetColor("_TintColor", new Color(0.05f, 0.3f, 0.5f, 1));
            cyloopOverlay.SetFloat("_OffsetAmount", 0.007f);

            // REMOVE THE DUMB SOUNDONSTART COMPONENT. IT DOESN"T WORK WITH POOLED EFFECTS (it would if it was onenable but that component shouldn't exist to begin with)
        }

        public static void AddScepterToBoostFlash(GameObject boostFlashPrefab)
        {
            GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("ScepterBoostElectricEffect"), boostFlashPrefab.transform);
        }

        public static GameObject MaterialSwap(GameObject prefab, string assetPath, string pathToParticle = "")
        {
            Transform transform = prefab.transform.Find(pathToParticle);
            if (transform)
            {
                transform.GetComponent<ParticleSystemRenderer>().sharedMaterial = Addressables.LoadAssetAsync<Material>(assetPath).WaitForCompletion();
            }
            return prefab;
        }
        public static GameObject MaterialSwap(GameObject prefab, Material material, string pathToParticle = "")
        {
            Transform transform = prefab.transform.Find(pathToParticle);
            if (transform)
            {
                transform.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
            }
            return prefab;
        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName) == null) return null;

            GameObject newTracer = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            Modules.Content.AddNetworkSoundEventDef(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            if (!objectToConvert) return;

            foreach (Renderer i in objectToConvert.GetComponentsInChildren<Renderer>())
            {
                i?.material?.SetHopooMaterial();
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }


        public static GameObject LoadSurvivorModel(string modelName) {
            GameObject model = mainAssetBundle.LoadAsset<GameObject>(modelName);
            if (model == null) {
                Log.Error("Trying to load a null model- check to see if the BodyName in your code matches the prefab name of the object in Unity\nFor Example, if your prefab in unity is 'mdlHenry', then your BodyName must be 'Henry'");
                return null;
            }

            return PrefabAPI.InstantiateClone(model, model.name, false);
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null) return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadAsyncedEffect(string resourceName)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<NetworkIdentity>();

            return newEffect;
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform, float destroyOnTimer = 3, bool effectDef = true)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            if (!newEffect)
            {
                Log.Error("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
                return null;
            }

            if (destroyOnTimer > 0) newEffect.AddComponent<DestroyOnTimer>().duration = destroyOnTimer;
            if (effectDef) newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            if (effectDef) AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            Modules.Content.AddEffectDef(newEffectDef);
        }
    }
}