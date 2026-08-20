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
using System.Linq;

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
        public static GameObject cyloopHitEffect;
        public static GameObject cyloopDoubleHitEffect;
        public static GameObject cyloopConstrictEffect;

        public static GameObject superCyloopHitWindEffect;
        public static GameObject superCyloopDoubleHitWindEffect;
        public static GameObject superCyloopConstrictWindEffect;

        public static GameObject superCyloopHitChainsEffect;
        public static GameObject superCyloopDoubleHitChainsEffect;
        public static GameObject superCyloopConstrictChainsEffect;

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
        internal static NetworkSoundEventDef cyloopCompleteSoundEvent;

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

            sonicBoomImpactEffect = Assets.LoadEffect("SonicSonicBoomImpact", "Play_sonicthehedgehog_sonic_boom_explode", false, 0.5f);
            crossSlashImpactEffect = Assets.LoadEffect("SonicCrossSlashImpact", "Play_sonicthehedgehog_sonic_boom_explode", false, 0.5f);

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

            scepterPowerBoostFlashEffect = HedgehogUtils.Assets.CreateBoostFlashEffect("SonicScepterPowerBoostFlash", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampNullifier_png).WaitForCompletion(), new Color(0.15f, 0.15f, 1f));
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
            scepterPowerBoostAuraEffect = HedgehogUtils.Assets.CreateBoostAuraEffect("SonicScepterPowerBoostAura", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampNullifier_png).WaitForCompletion(), new Color(0.15f, 0.15f, 1f));

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

            scepterSuperBoostFlashEffect = HedgehogUtils.Assets.CreateBoostFlashEffect("SonicScepterSuperBoostFlash", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampBanditSlash_png).WaitForCompletion(), Color.white, SonicTheHedgehogCharacter.superSonicColor2, true, 1.25f);
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
            scepterSuperBoostAuraEffect = HedgehogUtils.Assets.CreateBoostAuraEffect("SonicScepterSuperBoostAura", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampBanditSlash_png).WaitForCompletion(), Color.white, SonicTheHedgehogCharacter.superSonicColor2, new Color(1f, 0.15f, 0.5f), 1.25f);

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

            cyloopCompleteSoundEvent = CreateNetworkSoundEventDef("Play_sonicthehedgehog_cyloop_complete");
            cyloopTrail = LoadEffect("SonicCyloopTrail", "", false, -1f, false);
            cyloopTrail.GetComponent<VFXAttributes>().DoNotCullPool = true;
            Material cyberTrailMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_VoidSurvivor.matVoidSurvivorBlasterTrail_mat)).WaitForCompletion());
            cyberTrailMat.SetFloat("_InvFade", 0f);
            cyberTrailMat.SetFloat("_Boost", 1f);
            cyberTrailMat.SetFloat("_AlphaBoost", 2f);
            cyberTrailMat.SetTexture("_RampTex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2.texRampTritoneHShrine_png).WaitForCompletion());
            cyberTrailMat.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_TiledTextures.texCloudPixel2_png).WaitForCompletion());
            cyberTrailMat.SetTextureScale("_MainTex", new Vector2(0.1f, 0.4f));
            cyberTrailMat.EnableKeyword("VERTEXCOLOR");
            cyberTrailMat.SetColor("_TintColor", Color.white);
            cyberTrailMat.SetTexture("_Cloud2Tex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.texNanoPistolAOE_1c_png).WaitForCompletion());
            cyberTrailMat.SetTextureScale("_Cloud2Tex", new Vector2(0.15f, 2f));
            cyberTrailMat.SetVector("_CutoffScroll", new Vector4(-7, 0, -3, 5));
            cyloopTrail.transform.GetComponent<LineRenderer>().sharedMaterial = cyberTrailMat;
            var cyloopTrailFade = cyloopTrail.AddComponent<AnimateShaderAlpha>();
            cyloopTrailFade.alphaCurve = AnimationCurve.Linear(0, 1, 1, 0);
            cyloopTrailFade.initialyEnabled = false;
            cyloopTrailFade.timeMax = 0.45f;
            cyloopTrailFade.disableOnEnd = true;
            cyloopTrailFade.enabled = false;

            cyloopTrailSpawningEffect = LoadEffect("SonicCyloopTrailSpawningEffect", "", false, -1f, false);
            cyloopTrailSpawningEffect.GetComponent<VFXAttributes>().DoNotCullPool = true;
            Material cyberLinesMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.matNanoSeekerPixelTrail_1_mat)).WaitForCompletion());
            cyberLinesMat.SetTexture("_MainTex", mainAssetBundle.LoadAsset<Texture>("texCyberLines"));
            cyberLinesMat.SetFloat("_Boost", 5f);
            cyberLinesMat.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.texPrimeDevastatorTetherMask2_png).WaitForCompletion());
            cyberLinesMat.SetVector("_CutoffScroll", new Vector4(20f, 0, 0, 0));
            cyloopTrailSpawningEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberLinesMat;
            Material cyberPixelMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_RoboBallBoss.matRoboBallParticleBillboard_mat)).WaitForCompletion());
            cyberPixelMat.EnableKeyword("VERTEXCOLOR");
            cyberPixelMat.SetFloat("_Boost", 6f);
            cyberPixelMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.texDroneTechRamp_png).WaitForCompletion());
            Material cyberPixel2Mat = new Material(cyberLinesMat);
            cyberPixel2Mat.SetTexture("_MainTex", null);
            cyberPixel2Mat.SetFloat("_Boost", 3f);
            cyberPixel2Mat.SetTextureScale("_Cloud1Tex", new Vector2(0.3f, 1f));
            cyloopTrailSpawningEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberPixelMat;
            cyloopTrailSpawningEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberPixel2Mat;
            var cyloopSpawnDestroy = cyloopTrailSpawningEffect.AddComponent<DisableParticleEmissionAndDestroyOnTimer>();
            cyloopSpawnDestroy.waitDuration = 1.7f;
            cyloopSpawnDestroy.particleSystems = new List<ParticleSystem> { cyloopTrailSpawningEffect.transform.GetChild(0).GetComponent<ParticleSystem>(), cyloopTrailSpawningEffect.transform.GetChild(1).GetComponent<ParticleSystem>() };
            /*var cyloopTrailSpawningColor = cyloopTrailSpawningEffect.AddComponent<ParticleSystemColorFromEffectData>();
            cyloopTrailSpawningColor.effectComponent = cyloopTrailSpawningEffect.GetComponent<EffectComponent>();
            cyloopTrailSpawningColor.particleSystems = new ParticleSystem[] { cyloopTrailSpawningEffect.transform.GetChild(0).GetComponent<ParticleSystem>(), cyloopTrailSpawningEffect.transform.GetChild(1).GetComponent<ParticleSystem>() };*/

            cyloopOverlay = new Material(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_LunarGolem.matLunarGolemShield_mat).WaitForCompletion());
            cyloopOverlay.SetColor("_TintColor", new Color(0.05f, 0.3f, 0.5f, 1));
            cyloopOverlay.SetFloat("_OffsetAmount", 0.007f);

            Material wideGlow = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matWideGlow_mat).WaitForCompletion();
            Material glowSoft = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matGlow1Soft_mat).WaitForCompletion();
            Material distortionNoise = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Nullifier.matNullifierDeathDistortion_mat).WaitForCompletion();
            Material distortion = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matDistortionFaded_mat).WaitForCompletion();
            Material distortionInverse = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matInverseDistortion_mat).WaitForCompletion();
            Material cyberCubeSuper = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_SolusWing.matGravPulseHoloWall_mat)).WaitForCompletion());
            cyberCubeSuper.SetFloat("_Boost", 1f);
            cyberCubeSuper.SetFloat("_AlphaBoost", 0.8f);
            cyberCubeSuper.SetFloat("_AlphaBias", 0.2f);
            Material cyberCube = new Material(cyberCubeSuper);
            cyberCube.SetFloat("_Boost", 3f);
            cyberCube.SetFloat("_AlphaBoost", 0.4f);
            cyberCube.SetFloat("_AlphaBias", 0.48f);
            Material cyberWind = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Huntress.matHuntressSwingTrail_mat)).WaitForCompletion();
            cyberCube.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTeslaCoil_png).WaitForCompletion());
            Material glitch = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Items_SharedSuffering.matSharedSufferingGlitch_mat)).WaitForCompletion();
            Mesh donut2 = Addressables.LoadAssetAsync<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.mdlVFXDonut2_fbx_donut2Mesh_).WaitForCompletion();
            
            cyloopHitEffect = LoadEffect("SonicCyloopHitEffect", "", false, 0.7f, true, true);
            cyloopHitEffect.AddComponent<EffectManagerHelper>();
            cyloopHitEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = wideGlow;
            var cyloopHitDonut5 = cyloopHitEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>();
            cyloopHitDonut5.mesh = Addressables.LoadAssetAsync<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.mdlVFXDonut5_fbx_donut5Mesh_).WaitForCompletion();
            cyloopHitDonut5.sharedMaterial = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTeleportOut_mat)).WaitForCompletion());
            cyloopHitDonut5.sharedMaterial.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampHelfire_png)).WaitForCompletion());
            var cyloopHitRing = cyloopHitEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>();
            cyloopHitRing.mesh = donut2;
            cyloopHitRing.sharedMaterial = cyberWind;
            cyloopHitEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().sharedMaterial = glowSoft;
            cyloopHitEffect.transform.GetChild(4).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberCube;
            cyloopHitEffect.transform.GetChild(5).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberPixelMat;
            cyloopHitEffect.AddComponent<CyloopSoundComponent>().soundString = "Play_sonicthehedgehog_cyloop_hit";

            cyloopDoubleHitEffect = LoadEffect("SonicCyloopDoubleHitEffect", "", false, 0.45f, true, true);
            cyloopDoubleHitEffect.AddComponent<EffectManagerHelper>();
            cyloopDoubleHitEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = glowSoft;
            cyloopDoubleHitEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().trailMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.matPrimeDevastatorChargeVFX2_mat)).WaitForCompletion();
            cyloopDoubleHitEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = HedgehogUtils.Assets.darkSparkle;
            cyloopDoubleHitEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().sharedMaterial = distortionInverse;
            cyloopDoubleHitEffect.transform.GetChild(4).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberPixel2Mat;
            cyloopDoubleHitEffect.transform.GetChild(5).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Items_ShockDamageAura.matDroneShockDamageGlowBurst_mat)).WaitForCompletion();
            cyloopDoubleHitEffect.transform.GetChild(6).GetComponent<ParticleSystemRenderer>().sharedMaterial = wideGlow;
            cyloopDoubleHitEffect.AddComponent<CyloopSoundComponent>().soundString = "Play_sonicthehedgehog_cyloop_double";

            cyloopConstrictEffect = LoadEffect("SonicCyloopConstrictEffect", "", true, 0.1f, false, true);
            Material pixelFlash = new Material(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matOmniExplosion1Generic_mat).WaitForCompletion());
            pixelFlash.SetFloat("_InvFade", 2f);
            pixelFlash.SetFloat("_Boost", 4f);
            pixelFlash.SetFloat("_AlphaBoost", 2f);
            pixelFlash.SetFloat("_AlphaBias", 0.2f);
            pixelFlash.SetFloat("_DepthOffset", -2.5f);
            pixelFlash.SetInt("_ZTest", 7);
            pixelFlash.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampHelfire_png)).WaitForCompletion());
            pixelFlash.SetTexture("_Cloud1Tex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.texNanoPistolAOE_1c_png)).WaitForCompletion());
            pixelFlash.SetTexture("_Cloud2Tex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.texNanoPistolAOE_1b_png)).WaitForCompletion());
            pixelFlash.SetTextureScale("_Cloud2Tex", new Vector2(4f, 0.7f));
            pixelFlash.SetVector("_CutoffScroll", new Vector4(-200f, 50f, 100f, -250f));
            Transform cyloopConstrictMainEffect = cyloopConstrictEffect.transform.GetChild(0).transform.GetChild(0);
            cyloopConstrictMainEffect.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = pixelFlash;
            cyloopConstrictMainEffect.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Commando.matCommandoFMJRing_mat)).WaitForCompletion();
            var cyloopConstrictRing = cyloopConstrictMainEffect.GetChild(2).GetComponent<ParticleSystemRenderer>();
            cyloopConstrictRing.sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_Railgunner.matRailgunPenRing_mat)).WaitForCompletion();
            cyloopConstrictRing.mesh = donut2;
            cyloopConstrictMainEffect.GetChild(3).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberPixelMat;
            cyloopConstrictMainEffect.GetChild(4).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Items_SharedSuffering.matSharedSufferingGlitchDistortion_mat)).WaitForCompletion();
            cyloopConstrictMainEffect.GetChild(5).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Items_ShockDamageAura.matDroneShockDamageGlowBurst_mat)).WaitForCompletion();
            cyloopConstrictMainEffect.GetChild(6).GetComponent<ParticleSystemRenderer>().sharedMaterial = glitch;
            cyloopConstrictMainEffect.GetChild(7).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberPixel2Mat;

            Transform cyloopConstrictStartEffect = cyloopConstrictEffect.transform.GetChild(0).transform.GetChild(1);
            var cyloopConstrictStartLightning = cyloopConstrictStartEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
            cyloopConstrictStartLightning.sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Items_ShockDamageAura.matDroneShockDamageElectricity02_mat)).WaitForCompletion();
            cyloopConstrictStartLightning.mesh = AssetAsyncReferenceManager<Mesh>.LoadAsset(new AssetReferenceT<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3.mdlBentLightning01_fbx_mdlBentLightning01_)).WaitForCompletion();
            cyloopConstrictStartEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberPixelMat;
            cyloopConstrictStartEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = glitch;
            cyloopConstrictStartEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().sharedMaterial = HedgehogUtils.Assets.darkSparkle;
            var cyloopConstrictTempVisualEffect = cyloopConstrictEffect.AddComponent<TemporaryVisualEffect>();
            cyloopConstrictTempVisualEffect.visualTransform = cyloopConstrictEffect.transform.GetChild(0);
            cyloopConstrictTempVisualEffect.exitComponents = new MonoBehaviour[] { cyloopConstrictEffect.GetComponent<DestroyOnTimer>() };
            TempVisualEffectAPI.AddTemporaryVisualEffect(cyloopConstrictEffect, (body) => { return body.HasBuff(Buffs.cyloopDebuff) && body.hullClassification != HullClassification.BeetleQueen; });

            Material superWindMat = new Material(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_computationalexchange.matCERiver_mat).WaitForCompletion());
            superWindMat.SetFloat("_Boost", 1.5f);
            superWindMat.SetFloat("_RimStrength", 0.4f);
            superWindMat.SetColor("_TintColor", Color.white);
            superWindMat.SetTexture("_MainTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX_ParticleMasks.texAlphaGradient2Mask_png)).WaitForCompletion());
            superWindMat.SetTextureScale("_MainTex", new Vector2(1, 1));
            superWindMat.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_Common_ColorRamps.texRampConstructLaser_png)).WaitForCompletion());
            superWindMat.SetVector("_CutoffScroll", new Vector4(0, -100f, 0, -200f));
            Material superCoreMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_VoidSurvivor.matVoidSurvivorBlasterSphereAreaIndicator_mat)).WaitForCompletion());
            superCoreMat.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampLightning_png)).WaitForCompletion());
            superCyloopHitWindEffect = LoadEffect("SonicSuperCyloopHitWindEffect", "", false, 1f, true, true);
            superCyloopHitWindEffect.AddComponent<EffectManagerHelper>();
            superCyloopHitWindEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = glowSoft;
            superCyloopHitWindEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberCubeSuper;
            superCyloopHitWindEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = wideGlow;
            superCyloopHitWindEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().sharedMaterial = distortionNoise;
            superCyloopHitWindEffect.transform.GetChild(4).GetComponent<ParticleSystemRenderer>().sharedMaterial = distortionInverse;
            superCyloopHitWindEffect.transform.GetChild(5).GetComponent<ParticleSystemRenderer>().sharedMaterial = distortion;
            var superCyloopWindRing = superCyloopHitWindEffect.transform.GetChild(7).GetComponent<ParticleSystemRenderer>();
            superCyloopWindRing.sharedMaterial = cyberWind;
            superCyloopWindRing.mesh = donut2;
            var superCyloopWindCylinder = superCyloopHitWindEffect.transform.GetChild(8).GetComponent<ParticleSystemRenderer>();
            superCyloopWindCylinder.sharedMaterial = superWindMat;
            superCyloopWindCylinder.mesh = Addressables.LoadAssetAsync<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.mdlVFXDonut3_fbx_donut3Mesh_).WaitForCompletion();
            superCyloopHitWindEffect.transform.GetChild(9).GetComponent<ParticleSystemRenderer>().sharedMaterial = wideGlow;
            superCyloopHitWindEffect.transform.GetChild(10).GetComponent<ParticleSystemRenderer>().sharedMaterial = HedgehogUtils.Assets.darkSparkle;
            superCyloopHitWindEffect.transform.GetChild(11).GetComponent<ParticleSystemRenderer>().sharedMaterial = superCoreMat;
            Material superRingMat = new Material(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Huntress.matHuntressSwingTrail_mat).WaitForCompletion());
            superRingMat.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_Common_ColorRamps.texRampConstructLaser_png)).WaitForCompletion());
            superRingMat.SetFloat("_Boost", 2f);
            superCyloopHitWindEffect.AddComponent<CyloopSoundComponent>().soundString = "Play_sonicthehedgehog_cyloop_hit_wind";

            superCyloopDoubleHitWindEffect = LoadEffect("SonicSuperCyloopDoubleHitWindEffect", "", false, 0.46f, true, true);
            superCyloopDoubleHitWindEffect.AddComponent<EffectManagerHelper>();
            var superCyloopDoubleWindSphere = superCyloopDoubleHitWindEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
            superCyloopDoubleWindSphere.mesh = AssetAsyncReferenceManager<Mesh>.LoadAsset(new AssetReferenceT<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3.mdlNoiseSphere_01_fbx_GEO_NoiseSphere_)).WaitForCompletion();
            superCyloopDoubleWindSphere.sharedMaterial = new Material(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_DefectiveUnit.matDefectiveUnitDetonateSphereEnergyPers_mat).WaitForCompletion());
            superCyloopDoubleWindSphere.sharedMaterial.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_SolusAmalgamator.texRampSolusAmalgamator_png)).WaitForCompletion());
            superCyloopDoubleWindSphere.sharedMaterial.SetColor("_TintColor", Color.white);
            var superCyloopDoubleShockwave = superCyloopDoubleHitWindEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>();
            superCyloopDoubleShockwave.mesh = donut2;
            superCyloopDoubleShockwave.sharedMaterial = distortion; // different distortion mat since it's a ring?
            superCyloopDoubleHitWindEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = glowSoft;
            superCyloopDoubleHitWindEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().sharedMaterial = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Child.matChildStarGlow_mat).WaitForCompletion();
            superCyloopDoubleHitWindEffect.transform.GetChild(4).GetComponent<ParticleSystemRenderer>().sharedMaterial = wideGlow;
            var superCyloopDoubleHitRings = superCyloopDoubleHitWindEffect.transform.GetChild(6).GetComponent<ParticleSystemRenderer>();
            superCyloopDoubleHitRings.sharedMaterial = superRingMat;
            superCyloopDoubleHitRings.mesh = donut2;
            superCyloopDoubleHitWindEffect.AddComponent<CyloopSoundComponent>().soundString = "Play_sonicthehedgehog_cyloop_double";

            superCyloopConstrictWindEffect = LoadEffect("SonicSuperCyloopConstrictWindEffect", "", true, 0.1f, false, true);
            var superCyloopConstrictWindVfx = superCyloopConstrictWindEffect.transform.GetChild(0);
            superCyloopConstrictWindVfx.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberPixel2Mat;
            var superCyloopConstrictWindRings = superCyloopConstrictWindVfx.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
            superCyloopConstrictWindRings.mesh = donut2;
            superCyloopConstrictWindRings.sharedMaterial = superRingMat;
            var superCyloopConstrictWindTempVisualEffect = superCyloopConstrictWindEffect.AddComponent<TemporaryVisualEffect>();
            superCyloopConstrictWindTempVisualEffect.visualTransform = superCyloopConstrictWindVfx;
            superCyloopConstrictWindTempVisualEffect.exitComponents = new MonoBehaviour[] { superCyloopConstrictWindEffect.GetComponent<DestroyOnTimer>() };
            TempVisualEffectAPI.AddTemporaryVisualEffect(superCyloopConstrictWindEffect, (body) => { return body.HasBuff(Buffs.superCyloopDebuff)
                && (SkillStates.Cyloop.CyloopManager.superCyloopVFX == SkillStates.Cyloop.CyloopManager.SuperCyloopVFX.Wind ||
                SkillStates.Cyloop.CyloopManager.superCyloopVFX == SkillStates.Cyloop.CyloopManager.SuperCyloopVFX.Spears);/* REMOVE THIS PART*/ });

            superCyloopConstrictChainsEffect = LoadEffect("SonicSuperCyloopConstrictChainsEffect", "", true, 0.1f, false, true);
            var superCyloopConstrictChainsVfx = superCyloopConstrictChainsEffect.transform.GetChild(0);
            var superCyloopConstrictChainsStart = superCyloopConstrictChainsVfx.GetChild(0);
            var superCyloopConstrictChainsLoop = superCyloopConstrictChainsVfx.GetChild(1);
            superCyloopConstrictChainsStart.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.matLunarGazeFireLaser3_mat)).WaitForCompletion();
            superCyloopConstrictChainsStart.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTracerBrightTransparent_mat)).WaitForCompletion();
            superCyloopConstrictChainsStart.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Child.matChildStarGlow_mat).WaitForCompletion();
            superCyloopConstrictChainsStart.GetChild(3).GetComponent<ParticleSystemRenderer>().sharedMaterial = distortionInverse;
            superCyloopConstrictChainsStart.GetChild(4).GetComponent<ParticleSystemRenderer>().sharedMaterial = glowSoft;
            var superCyloopConstrictChainsStartRings = superCyloopConstrictChainsStart.GetChild(5).GetComponent<ParticleSystemRenderer>();
            superCyloopConstrictChainsStartRings.mesh = donut2;
            superCyloopConstrictChainsStartRings.sharedMaterial = superRingMat;
            superCyloopConstrictChainsStart.GetChild(6).GetComponent<ParticleSystemRenderer>().sharedMaterial = wideGlow;
            Material superCyloopChainsMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_BounceNearby.matHookTrail_mat)).WaitForCompletion());
            superCyloopChainsMat.SetFloat("_Boost", 3f);
            superCyloopChainsMat.SetFloat("_AlphaBoost", 1f);
            superCyloopChainsMat.SetFloat("_AlphaBias", 0.35f);
            superCyloopChainsMat.SetTexture("_MainTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDefault_png)).WaitForCompletion());
            superCyloopChainsMat.SetTextureScale("_MainTex", new Vector2(-1f, 1f));
            superCyloopChainsMat.SetTextureOffset("_MainTex", new Vector2(1f, 0f));
            superCyloopChainsMat.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_Common_ColorRamps.texRampConstructLaser_png)).WaitForCompletion());
            superCyloopChainsMat.SetVector("_CutoffScroll", new Vector4(30f, 0f, 0f, 0f));
            superCyloopConstrictChainsLoop.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = superCyloopChainsMat;
            var superCyloopConstrictChainsRings = superCyloopConstrictChainsLoop.GetChild(1).GetComponent<ParticleSystemRenderer>();
            superCyloopConstrictChainsRings.mesh = donut2;
            superCyloopConstrictChainsRings.sharedMaterial = superRingMat;
            var superCyloopConstrictChainsTempVisualEffect = superCyloopConstrictChainsEffect.AddComponent<TemporaryVisualEffect>();
            superCyloopConstrictChainsTempVisualEffect.visualTransform = superCyloopConstrictChainsVfx;
            var superCyloopConstrictChainsSound = superCyloopConstrictChainsEffect.AddComponent<CyloopSoundComponent>();
            superCyloopConstrictChainsSound.soundString = "Play_sonicthehedgehog_cyloop_hit_alt";
            superCyloopConstrictChainsTempVisualEffect.enterComponents = new MonoBehaviour[] { superCyloopConstrictChainsSound };
            superCyloopConstrictChainsTempVisualEffect.exitComponents = new MonoBehaviour[] { superCyloopConstrictChainsEffect.GetComponent<DestroyOnTimer>() };
            TempVisualEffectAPI.AddTemporaryVisualEffect(superCyloopConstrictChainsEffect, (body) => { return body.HasBuff(Buffs.superCyloopDebuff) && body.healthComponent && body.healthComponent.alive
                && SkillStates.Cyloop.CyloopManager.superCyloopVFX == SkillStates.Cyloop.CyloopManager.SuperCyloopVFX.Chains; });

            superCyloopHitChainsEffect = LoadEffect("SonicSuperCyloopHitChainsEffect", "", false, 0.7f, true, true);
            superCyloopHitChainsEffect.AddComponent<EffectManagerHelper>();
            superCyloopHitChainsEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = wideGlow;
            var superCyloopHitChainsDonut5 = superCyloopHitChainsEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>();
            superCyloopHitChainsDonut5.mesh = Addressables.LoadAssetAsync<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.mdlVFXDonut5_fbx_donut5Mesh_).WaitForCompletion();
            superCyloopHitChainsDonut5.sharedMaterial = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTeleportOut_mat)).WaitForCompletion());
            superCyloopHitChainsDonut5.sharedMaterial.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampRoboBall_png)).WaitForCompletion());
            var superCyloopHitChainsRing = superCyloopHitChainsEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>();
            superCyloopHitChainsRing.mesh = donut2;
            superCyloopHitChainsRing.sharedMaterial = cyberWind;
            superCyloopHitChainsEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().sharedMaterial = glowSoft;
            superCyloopHitChainsEffect.AddComponent<CyloopSoundComponent>().soundString = "Play_sonicthehedgehog_cyloop_hit";

            superCyloopDoubleHitChainsEffect = LoadEffect("SonicSuperCyloopDoubleHitChainsEffect", "", false, 0.8f, true, true);
            superCyloopDoubleHitChainsEffect.AddComponent<EffectManagerHelper>();
            superCyloopDoubleHitChainsEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = glowSoft;
            superCyloopDoubleHitChainsEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().trailMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.matPrimeDevastatorChargeVFX2_mat)).WaitForCompletion();
            superCyloopDoubleHitChainsEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = HedgehogUtils.Assets.darkSparkle;
            superCyloopDoubleHitChainsEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().sharedMaterial = distortionInverse;
            superCyloopDoubleHitChainsEffect.transform.GetChild(4).GetComponent<ParticleSystemRenderer>().sharedMaterial = cyberPixel2Mat;
            superCyloopDoubleHitChainsEffect.transform.GetChild(5).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Child.matChildStarGlow_mat)).WaitForCompletion();
            superCyloopDoubleHitChainsEffect.transform.GetChild(6).GetComponent<ParticleSystemRenderer>().sharedMaterial = wideGlow;
            Material superCyloopChainMat = new Material(superCyloopChainsMat);
            superCyloopChainMat.SetTextureOffset("_MainTex", new Vector2(1.5f, 0f));
            superCyloopChainMat.SetTextureScale("_Cloud1Tex", new Vector2(1f, 1f));
            superCyloopChainMat.SetVector("_CutoffScroll", Vector4.zero);
            superCyloopDoubleHitChainsEffect.transform.GetChild(7).GetComponent<ParticleSystemRenderer>().sharedMaterial = superCyloopChainMat;
            var superCyloopDoubleChainsShockwave = superCyloopDoubleHitChainsEffect.transform.GetChild(8).GetComponent<ParticleSystemRenderer>();
            superCyloopDoubleChainsShockwave.mesh = donut2;
            superCyloopDoubleChainsShockwave.sharedMaterial = distortion; // different distortion mat since it's a ring?

            superCyloopDoubleHitChainsEffect.AddComponent<CyloopSoundComponent>().soundString = "Play_sonicthehedgehog_cyloop_double_alt";

            SkillStates.Cyloop.CyloopManager.Initialize();
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

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform, float destroyOnTimer = 3, bool effectDef = true, bool applyScale = false)
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
            effect.applyScale = applyScale;
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