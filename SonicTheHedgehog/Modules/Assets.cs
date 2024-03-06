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

namespace SonicTheHedgehog.Modules
{
    internal static class Assets
    {
        #region Sonic's stuff
        // particle effects
        internal static GameObject swordSwingEffect;
        internal static GameObject swordHitImpactEffect;
        internal static GameObject sonicBoomKickEffect;
        internal static GameObject homingAttackTrailEffect;
        internal static GameObject sonicBoomImpactEffect;

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

        internal static GameObject grandSlamHitEffect;

        internal static GameObject bombExplosionEffect;

        // materials

        internal static Material superSonicOverlay;

        // networked hit sounds
        internal static NetworkSoundEventDef meleeHitSoundEvent;
        internal static NetworkSoundEventDef meleeFinalHitSoundEvent;
        internal static NetworkSoundEventDef homingHitSoundEvent;
        internal static NetworkSoundEventDef grandSlamHitSoundEvent;

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

            sonicBoomKickEffect = Assets.LoadEffect("SonicSonicBoomKick", true);
            homingAttackTrailEffect = Assets.LoadEffect("SonicHomingAttack", true);
            sonicBoomImpactEffect = Assets.LoadEffect("SonicSonicBoomImpact");

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
            idwAttackEffect = Assets.LoadEffect("SonicIDWAttack", true);

            superSonicBlurEffect = Assets.LoadEffect("SonicSuperBlur", true);

            powerBoostFlashEffect = Assets.LoadEffect("SonicPowerBoostFlash", true);

            scepterPowerBoostFlashEffect = Assets.LoadEffect("SonicScepterPowerBoostFlash", true);
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
            scepterSuperBoostFlashEffect = Assets.LoadEffect("SonicScepterSuperBoostFlash", true);
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


            boostFlashEffect = Assets.LoadEffect("SonicBoostFlash", true);
            superBoostFlashEffect = Assets.LoadEffect("SonicSuperBoostFlash", true);
            grandSlamHitEffect = Assets.LoadEffect("SonicGrandSlamKickHit", true);

            meleeHitSoundEvent = CreateNetworkSoundEventDef("Play_melee_hit");
            meleeFinalHitSoundEvent = CreateNetworkSoundEventDef("Play_melee_hit_final");
            homingHitSoundEvent = CreateNetworkSoundEventDef("Play_homing_impact");
            grandSlamHitSoundEvent = CreateNetworkSoundEventDef("Play_strong_impact");

            if (sonicBoomImpactEffect)
            {
                sonicBoomImpactEffect.AddComponent<SoundOnStart>().soundString = "Play_sonic_boom_explode";
            }

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

            superSonicOverlay = LegacyResourcesAPI.Load<Material>("Materials/matLunarGolemShield");
            superSonicOverlay.SetColor("_TintColor", new Color(1, 0.8f, 0.4f, 1));
            superSonicOverlay.SetColor("_EmissionColor", new Color(1, 0.8f, 0.4f, 1));
            superSonicOverlay.SetFloat("_OffsetAmount", 0.01f);
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