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

        public static GameObject superSonicTransformationEffect;
        public static GameObject transformationEmeraldSwirl;

        public static GameObject superSonicAura;
        public static GameObject superSonicWarning;

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
        public static GameObject scepterBoostFlashEffect;
        public static GameObject superBoostFlashEffect;
        public static GameObject scepterSuperBoostFlashEffect;

        public static GameObject powerBoostAuraEffect;
        public static GameObject superBoostAuraEffect;
        public static GameObject scepterPowerBoostAuraEffect;
        public static GameObject scepterSuperBoostAuraEffect;

        public static GameObject grandSlamHitEffect;

        // hud
        public static GameObject powerBoostHud;

        // materials

        internal static Material superSonicOverlay;

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

            sonicBoomKickEffect = Assets.LoadEffect("SonicSonicBoomKick", true);
            homingAttackTrailEffect = Assets.LoadEffect("SonicHomingAttack", true);

            sonicBoomImpactEffect = Assets.LoadEffect("SonicSonicBoomImpact");
            if (sonicBoomImpactEffect)
            {
                sonicBoomImpactEffect.AddComponent<SoundOnStart>().soundString = "Play_sonicthehedgehog_sonic_boom_explode";
            }
            crossSlashImpactEffect = Assets.LoadEffect("SonicCrossSlashImpact");
            if (crossSlashImpactEffect)
            {
                crossSlashImpactEffect.AddComponent<SoundOnStart>().soundString = "Play_sonicthehedgehog_sonic_boom_explode";
            }

            superSonicTransformationEffect = Assets.LoadEffect("SonicSuperTransformation");
            transformationEmeraldSwirl = Assets.LoadEffect("SonicChaosEmeraldSwirl");

            superSonicAura = Assets.LoadAsyncedEffect("SonicSuperAura");

            superSonicWarning = Assets.LoadAsyncedEffect("SonicSuperWarning");

            meleeHitEffect = Assets.LoadEffect("SonicMeleeHit", true);
            meleeImpactEffect = Assets.LoadEffect("SonicMeleeImpact");
            homingAttackLaunchEffect = Assets.LoadEffect("SonicHomingAttackLaunch");
            homingAttackHitEffect = Assets.LoadEffect("SonicHomingAttackHit", true);

            parryEffect = Assets.LoadEffect("SonicParry", true);
            parryActivateEffect = Assets.LoadEffect("SonicParryActivate", true);
            followUpKickEffect = Assets.LoadEffect("SonicFollowUpKick", true);
            idwAttackEffect = MaterialSwap(Assets.LoadAsyncedEffect("SonicIDWAttack"), RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Croco.matCrocoSlashDistortion_mat, "Blur/Distortion");

            superSonicBlurEffect = Assets.LoadEffect("SonicSuperBlur", true);

            powerBoostFlashEffect = HedgehogUtils.Assets.CreateNewBoostFlash("SonicPowerBoostFlash", 1, 1.3f,
                new Color(1, 1, 1), new Color(0.1098039f, 0.772549f, 1), new Color(0.05098039f, 0.4469049f, 1), new Color(0.1098039f, 0.772549f, 1));
                
                //MaterialSwap(Assets.LoadEffect("SonicPowerBoostFlash", true), "RoR2/Base/Common/VFX/matDistortionFaded.mat", "Distortion");


            powerBoostAuraEffect = HedgehogUtils.Assets.CreateNewBoostAura("SonicPowerBoostAura", 1, 0.65f,
                new Color(1, 1, 1), new Color(0.1098039f, 0.772549f, 1), new Color(0.05098039f, 0.4469049f, 1), new Color(0.1098039f, 0.772549f, 1));
            // ScepterBoostElectricEffect ScepterSuperBoostElectricEffect

            scepterPowerBoostFlashEffect = HedgehogUtils.Assets.CreateNewBoostFlash("SonicScepterPowerBoostFlash", 1, 1.3f,
                new Color(1, 1, 1), new Color(0.1933962f, 0.6118863f, 1), new Color(0.3363763f, 0.240566f, 1), new Color(0.1933962f, 0.6118863f, 1));

            AddScepterToBoostFlash(scepterPowerBoostFlashEffect);

            //MaterialSwap(Assets.LoadEffect("SonicScepterPowerBoostFlash", true), "RoR2/Base/Common/VFX/matDistortionFaded.mat", "Distortion");
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
            scepterPowerBoostAuraEffect = HedgehogUtils.Assets.CreateNewBoostAura("SonicScepterPowerBoostAura", 1, 0.65f,
                new Color(1, 1, 1), new Color(0.1933962f, 0.6118863f, 1), new Color(0.3363763f, 0.240566f, 1), new Color(0.1933962f, 0.6118863f, 1));

            scepterBoostFlashEffect = Assets.LoadEffect("SonicScepterBoostFlash", true);
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


            boostFlashEffect = Assets.LoadEffect("SonicBoostFlash", true);

            superBoostFlashEffect = HedgehogUtils.Assets.CreateNewBoostFlash("SonicSuperBoostFlash", 1.3f, 1.6f,
                new Color(1, 1, 1), new Color(1, 0.9702323f, 0.08f), new Color(1f, 0.6267163f, 0), new Color(1, 0.9702323f, 0.08f));
            superBoostAuraEffect = HedgehogUtils.Assets.CreateNewBoostAura("SonicSuperBoostAura", 1.3f, 0.8f,
                new Color(1, 1, 1), new Color(1, 0.9702323f, 0.08f), new Color(1f, 0.6267163f, 0), new Color(1, 0.9702323f, 0.08f));

            scepterSuperBoostFlashEffect = HedgehogUtils.Assets.CreateNewBoostFlash("SonicScepterSuperBoostFlash", 1.3f, 1.6f,
                new Color(1, 1, 1), new Color(1, 0.9843137f, 0.3160377f), new Color(1f, 0.4103774f, 0.6793281f), new Color(1, 0.9843137f, 0.3160377f));

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
            scepterSuperBoostAuraEffect = HedgehogUtils.Assets.CreateNewBoostAura("SonicScepterSuperBoostAura", 1.3f, 0.8f,
                new Color(1, 1, 1), new Color(1, 0.9843137f, 0.3160377f), new Color(1f, 0.4103774f, 0.6793281f), new Color(1, 0.9843137f, 0.3160377f));

            grandSlamHitEffect = Assets.LoadEffect("SonicGrandSlamKickHit", true);

            meleeHitSoundEvent = CreateNetworkSoundEventDef("Play_sonicthehedgehog_melee_hit");
            meleeFinalHitSoundEvent = CreateNetworkSoundEventDef("Play_sonicthehedgehog_melee_hit_final");
            homingHitSoundEvent = CreateNetworkSoundEventDef("Play_sonicthehedgehog_homing_impact");
            grandSlamHitSoundEvent = CreateNetworkSoundEventDef("Play_sonicthehedgehog_strong_impact");

            superGrandSlamLoopSoundDef = ScriptableObject.CreateInstance<LoopSoundDef>();
            superGrandSlamLoopSoundDef.startSoundName = "Play_sonicthehedgehog_super_grand_slam_loop";
            superGrandSlamLoopSoundDef.stopSoundName = "Stop_sonicthehedgehog_super_grand_slam_loop";

            if (superSonicTransformationEffect)
            {
                ShakeEmitter shakeEmitter = superSonicTransformationEffect.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.7f;
                shakeEmitter.radius = 200f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.7f,
                    frequency = 40f,
                    cycleOffset = 0f
                };
            }

            superSonicOverlay = new Material(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_LunarGolem.matLunarGolemShield_mat).WaitForCompletion());
            superSonicOverlay.SetColor("_TintColor", new Color(1, 0.8f, 0.4f, 1));
            superSonicOverlay.SetColor("_EmissionColor", new Color(1, 0.8f, 0.4f, 1));
            superSonicOverlay.SetFloat("_OffsetAmount", 0.01f);

            powerBoostHud = Assets.mainAssetBundle.LoadAsset<GameObject>("PowerParticles");
            powerBoostHud.AddComponent<PowerBoostHUD>();

            /*
             * New pod needs ------------------------------------------------
             *  NetworkIdentity
             *  EntityStateMachine Main
             *      initialStateType = whatever new state handles this
             *  NetworkEntityStateMachine
             *      Add main state machine
             *  SurvivorPodController
             *      cameraBone on another object off to the side
             *      camera following target is baked into the falling animation. Since the pod itself has no animator, probably automate the camera somehow?
             *  VehicleSeat
             *      passengerState = new SerializableEntityStateType(typeof(GenericCharacterPod));
             *      seatPosition = transform;
             *      handleExitTeleport = false
             *      exitVelocityFraction = 0f
             *      isSurvivorPod = true
             *      exitVehicleContextString = "Get up" lang token or something
             *      shouldProximityHighlight = false
             *      Should delete itself after the player exits?
             *  BuffPassengerWhileSeated 
             *      RoR2Content.Buffs.HiddenInvincibility (Might have to load asset since it's done early?)
             *  Permanent crater decal?
             */
        }

        public static void AddScepterToBoostFlash(GameObject boostFlashPrefab)
        {
            GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("ScepterBoostElectricEffect"), boostFlashPrefab.transform.Find("BlueCone"));
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

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            if (!newEffect)
            {
                Log.Error("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
                return null;
            }

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

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