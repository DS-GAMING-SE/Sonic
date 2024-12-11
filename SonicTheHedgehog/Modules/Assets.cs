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
        internal static GameObject sonicBoomKickEffect;
        internal static GameObject homingAttackTrailEffect;
        internal static GameObject sonicBoomImpactEffect;
        internal static GameObject crossSlashImpactEffect;

        internal static GameObject superSonicTransformationEffect;
        internal static GameObject transformationEmeraldSwirl;

        internal static GameObject superSonicAura;
        internal static GameObject superSonicWarning;

        internal static GameObject meleeHitEffect;
        internal static GameObject meleeImpactEffect;
        internal static GameObject homingAttackLaunchEffect;
        internal static GameObject homingAttackHitEffect;

        internal static GameObject parryEffect;
        internal static GameObject parryActivateEffect;
        internal static GameObject idwAttackEffect;

        internal static GameObject superSonicBlurEffect;

        internal static GameObject powerBoostFlashEffect;
        internal static GameObject scepterPowerBoostFlashEffect;
        internal static GameObject boostFlashEffect;
        internal static GameObject scepterBoostFlashEffect;
        internal static GameObject superBoostFlashEffect;
        internal static GameObject scepterSuperBoostFlashEffect;

        internal static GameObject powerBoostAuraEffect;
        internal static GameObject superBoostAuraEffect;
        internal static GameObject scepterPowerBoostAuraEffect;
        internal static GameObject scepterSuperBoostAuraEffect;

        internal static GameObject grandSlamHitEffect;

        internal static GameObject bombExplosionEffect;

        // materials

        internal static Material superSonicOverlay;

        // networked hit sounds
        internal static NetworkSoundEventDef meleeHitSoundEvent;
        internal static NetworkSoundEventDef meleeFinalHitSoundEvent;
        internal static NetworkSoundEventDef homingHitSoundEvent;
        internal static NetworkSoundEventDef grandSlamHitSoundEvent;

        internal static LoopSoundDef superLoopSoundDef;
        internal static LoopSoundDef superGrandSlamLoopSoundDef;

        #endregion

        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        // CHANGE THIS
        private const string assetbundleName = "sonicthehedgehogassetbundle";
        //change this to your project's name if/when you've renamed it
        private const string csProjName = "SonicTheHedgehog";
        
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

        internal static void LoadAssetBundle()
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
                sonicBoomImpactEffect.AddComponent<SoundOnStart>().soundString = "Play_sonic_boom_explode";
            }
            crossSlashImpactEffect = Assets.LoadEffect("SonicCrossSlashImpact");
            if (crossSlashImpactEffect)
            {
                crossSlashImpactEffect.AddComponent<SoundOnStart>().soundString = "Play_sonic_boom_explode";
            }

            superSonicTransformationEffect = Assets.LoadEffect("SonicSuperTransformation");
            transformationEmeraldSwirl = Assets.LoadEffect("SonicChaosEmeraldSwirl");

            superSonicAura = Assets.LoadEffect("SonicSuperAura", true);
            superSonicAura.GetComponent<DestroyOnTimer>().enabled = false;
            superSonicWarning = Assets.LoadEffect("SonicSuperWarning", true);
            superSonicWarning.GetComponent<DestroyOnTimer>().enabled = false;

            meleeHitEffect = Assets.LoadEffect("SonicMeleeHit", true);
            meleeImpactEffect = Assets.LoadEffect("SonicMeleeImpact");
            homingAttackLaunchEffect = Assets.LoadEffect("SonicHomingAttackLaunch");
            homingAttackHitEffect = Assets.LoadEffect("SonicHomingAttackHit", true);

            parryEffect = Assets.LoadEffect("SonicParry", true);
            parryActivateEffect = Assets.LoadEffect("SonicParryActivate", true);
            idwAttackEffect = MaterialSwap(Assets.LoadEffect("SonicIDWAttack", true), "RoR2/Base/Croco/matCrocoSlashDistortion.mat", "Blur/Distortion");

            superSonicBlurEffect = Assets.LoadEffect("SonicSuperBlur", true);

            powerBoostFlashEffect = MaterialSwap(Assets.LoadEffect("SonicPowerBoostFlash", true), "RoR2/Base/Common/VFX/matDistortionFaded.mat", "Distortion");
            powerBoostAuraEffect = Assets.mainAssetBundle.LoadAsset<GameObject>("SonicPowerBoostAura");

            scepterPowerBoostFlashEffect = MaterialSwap(Assets.LoadEffect("SonicScepterPowerBoostFlash", true), "RoR2/Base/Common/VFX/matDistortionFaded.mat", "Distortion");
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
            scepterPowerBoostAuraEffect = Assets.mainAssetBundle.LoadAsset<GameObject>("SonicScepterPowerBoostAura");

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
            scepterSuperBoostFlashEffect = MaterialSwap(Assets.LoadEffect("SonicScepterSuperBoostFlash", true), "RoR2/Base/Common/VFX/matDistortionFaded.mat", "Distortion");
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
            scepterSuperBoostAuraEffect = Assets.mainAssetBundle.LoadAsset<GameObject>("SonicScepterSuperBoostAura");


            boostFlashEffect = Assets.LoadEffect("SonicBoostFlash", true);
            superBoostFlashEffect = MaterialSwap(Assets.LoadEffect("SonicSuperBoostFlash", true), "RoR2/Base/Common/VFX/matDistortionFaded.mat", "Distortion");
            superBoostAuraEffect = Assets.mainAssetBundle.LoadAsset<GameObject>("SonicSuperBoostAura");

            grandSlamHitEffect = Assets.LoadEffect("SonicGrandSlamKickHit", true);

            meleeHitSoundEvent = CreateNetworkSoundEventDef("Play_melee_hit");
            meleeFinalHitSoundEvent = CreateNetworkSoundEventDef("Play_melee_hit_final");
            homingHitSoundEvent = CreateNetworkSoundEventDef("Play_homing_impact");
            grandSlamHitSoundEvent = CreateNetworkSoundEventDef("Play_strong_impact");

            superLoopSoundDef = ScriptableObject.CreateInstance<LoopSoundDef>();
            superLoopSoundDef.startSoundName = "Play_super_loop";
            superLoopSoundDef.stopSoundName = "Stop_super_loop";

            superGrandSlamLoopSoundDef = ScriptableObject.CreateInstance<LoopSoundDef>();
            superGrandSlamLoopSoundDef.startSoundName = "Play_super_grand_slam_loop";
            superGrandSlamLoopSoundDef.stopSoundName = "Stop_super_grand_slam_loop";

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

            superSonicOverlay = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarGolem/matLunarGolemShield.mat").WaitForCompletion());
            superSonicOverlay.SetColor("_TintColor", new Color(1, 0.8f, 0.4f, 1));
            superSonicOverlay.SetColor("_EmissionColor", new Color(1, 0.8f, 0.4f, 1));
            superSonicOverlay.SetFloat("_OffsetAmount", 0.01f);      
        }
        // Use this to create a new boost flash prefab to be used when activating a custom boost skill
        // name: An internal name for the prefab. Doesn't really matter what this is as long as it's not the same as anything else
        // size: The size of the effect. Power Boost defaults to 1. Super Boost defaults to ---
        // alpha: How visible the effect will be. Power Boost defaults to 1.3. Super Boost defaults to 1.6
        // color1: The innermost color of the effect
        // color2: The color between the innermost color and the edge color
        // color3: The color of the edge of the boost effect
        // lightColor: The color of the light emitted
        public static GameObject CreateNewBoostFlash(string name, float size, float alpha, Color color1, Color color2, Color color3, Color lightColor)
        {
            GameObject newFlash = PrefabAPI.InstantiateClone(powerBoostFlashEffect, name);
            AddNewEffectDef(newFlash);

            ParticleSystem.MainModule main = newFlash.transform.Find("BlueCone").GetComponent<ParticleSystem>().main;
            main.startSize = new ParticleSystem.MinMaxCurve(main.startSize.constant * size);

            ParticleSystem.MainModule main2 = newFlash.transform.Find("BlueCone/BlueCone2").GetComponent<ParticleSystem>().main;
            main2.startSize = new ParticleSystem.MinMaxCurve(main2.startSize.constant * size);

            ParticleSystemRenderer renderer = newFlash.transform.Find("BlueCone").GetComponent<ParticleSystemRenderer>();
            renderer.material = CreateNewBoostMaterial(alpha, color1, color2, color3);

            ParticleSystemRenderer renderer2 = newFlash.transform.Find("BlueCone/BlueCone2").GetComponent<ParticleSystemRenderer>();
            renderer2.material = CreateNewBoostMaterial(alpha, color1, color2, color3);

            newFlash.transform.Find("BlueCone/StartFlash/Point Light").GetComponent<Light>().color = lightColor;

            return newFlash;
        }
        // Use this to create a new boost aura prefab to be used constantly while using a custom boost skill
        // name: An internal name for the prefab. Doesn't really matter what this is as long as it's not the same as anything else
        // size: The size of the effect. Power Boost defaults to 1. Super Boost defaults to 1.3
        // alpha: How visible/strong the effect will be. Power Boost defaults to 0.65. Super Boost defaults to 0.8
        // color1: The innermost color of the effect
        // color2: The color between the innermost color and the edge color
        // color3: The color of the edge of the boost effect
        // lightColor: The color of the light emitted
        public static GameObject CreateNewBoostAura(string name, float size, float alpha, Color color1, Color color2, Color color3, Color lightColor)
        {
            GameObject newAura = PrefabAPI.InstantiateClone(powerBoostAuraEffect, name);
            newAura.transform.Find("Aura").localScale *= size;
            newAura.transform.Find("Aura").GetComponent<MeshRenderer>().material = CreateNewBoostMaterial(alpha, color1, color2, color3);
            newAura.transform.Find("Point Light").GetComponent<Light>().color = lightColor;
            return newAura;
        }

        private static Material CreateNewBoostMaterial(float alpha, Color color1, Color color2, Color color3)
        {
            Material newMaterial = new Material(Assets.mainAssetBundle.LoadAsset<Material>("matPowerBoost"));
            newMaterial.SetFloat("_AlphaBoost", alpha);
            newMaterial.SetColor("_Color1", color1);
            newMaterial.SetColor("_Color2", color2);
            newMaterial.SetColor("_Color3", color3);

            return newMaterial;
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