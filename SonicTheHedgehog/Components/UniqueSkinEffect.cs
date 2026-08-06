using R2API;
using RoR2;
using RoR2.ContentManagement;
using SonicTheHedgehog.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static SonicTheHedgehog.Components.UniqueSkinEffect;

namespace SonicTheHedgehog.Components
{
    public class UniqueSkinEffect : MonoBehaviour
    {
        public CharacterBody characterBody;
        public Animator animator;
        public ModelSkinController skinController;

        internal static List<SkinDef> flyingAnimationSkins = new List<SkinDef>();
        internal static List<SkinDef> swordAnimationSkins = new List<SkinDef>(); // Could it be??? A hint at a future update???
        public static SkinEffects?[] skinEffects;

        internal static Dictionary<SkinDef, AssetOrDirectReference<Mesh>> superGrandSlamMeshReplacements = new Dictionary<SkinDef, AssetOrDirectReference<Mesh>>();
        internal static Dictionary<SkinDef, GameObject> boostFlashReplacements = new Dictionary<SkinDef, GameObject>();
        internal static Dictionary<SkinDef, GameObject> boostAuraReplacements = new Dictionary<SkinDef, GameObject>();

        private void Start()
        {
            characterBody = GetComponent<CharacterBody>();
            if (characterBody)
            {
                if (characterBody.modelLocator && characterBody.modelLocator.modelTransform)
                {
                    animator = characterBody.modelLocator.modelTransform.GetComponent<Animator>();
                    skinController = characterBody.modelLocator.modelTransform.GetComponent<ModelSkinController>();
                    skinController.onSkinApplied += UpdateSkin;
                }
            }
        }
        private void UpdateSkin(int skinIndex)
        {
            if (skinEffects[skinIndex].HasValue && skinEffects[skinIndex].Value.flying)
            {
                animator.SetFloat("isMetalSonic", 1);
            }
        }
        public static bool TryGetSkinEffects(GameObject gameObject, out SkinEffects? skinEffect)
        {
            skinEffect = null;
            if (gameObject.TryGetComponent<UniqueSkinEffect>(out var component) && component.skinController && skinEffects[component.skinController.currentSkinIndex].HasValue)
            {
                skinEffect = skinEffects[component.skinController.currentSkinIndex].Value;
                return true;
            }
            return false;
        }
        internal static void Bake()
        {
            RoR2.UI.MainMenu.MainMenuController.OnMainMenuInitialised -= UniqueSkinEffect.Bake;
            SkinDef[] skins = SkinCatalog.GetBodySkinDefs(BodyCatalog.FindBodyIndex("SonicTheHedgehog"));
            skinEffects = new SkinEffects?[skins.Length];
            bool flying = false;
            bool sword = false;
            for (int i = 0; i < skins.Length; i++)
            {
                flying = flyingAnimationSkins.Contains(skins[i]);
                sword = swordAnimationSkins.Contains(skins[i]);
                if (flying || sword || superGrandSlamMeshReplacements.ContainsKey(skins[i]) || boostFlashReplacements.ContainsKey(skins[i]) || boostAuraReplacements.ContainsKey(skins[i]))
                {
                    skinEffects[i] = new SkinEffects
                    {
                        flying = flying,
                        sword = sword,
                        superGrandSlamMeshReplacement = superGrandSlamMeshReplacements.GetValueOrDefault(skins[i]),
                        boostFlashEffect = boostFlashReplacements.GetValueOrDefault(skins[i]),
                        boostAuraEffect = boostAuraReplacements.GetValueOrDefault(skins[i])
                    };
                }
            }

            flyingAnimationSkins = null;
            swordAnimationSkins = null;
            superGrandSlamMeshReplacements = null;
            boostFlashReplacements = null;
            boostAuraReplacements = null;
        }

        public struct SkinEffects
        {
            public bool flying;
            public bool sword;
            public AssetOrDirectReference<Mesh> superGrandSlamMeshReplacement;
            public GameObject boostFlashEffect;
            public GameObject boostAuraEffect;
        }
    }
    public static class UniqueSkinEffectsExtensions
    {
        public static void AddFlyingSkin(this SkinDef skin)
        {
            flyingAnimationSkins.Add(skin);
        }
        public static void AddSwordSkin(this SkinDef skin)
        {
            swordAnimationSkins.Add(skin);
        }
        public static void AddSuperGrandSlamMeshReplacement(this SkinDef skin, Mesh mesh)
        {
            superGrandSlamMeshReplacements.Add(skin, new AssetOrDirectReference<Mesh> { directRef = mesh });
        }
        public static void AddSuperGrandSlamMeshReplacement(this SkinDef skin, AssetReferenceT<Mesh> meshAddress)
        {
            superGrandSlamMeshReplacements.Add(skin, new AssetOrDirectReference<Mesh> { loadOnAssigned = false, address = meshAddress });
        }
        public static void AddBoostFlashReplacement(this SkinDef skin, GameObject boostFlash)
        {
            boostFlashReplacements.Add(skin, boostFlash);
        }
        public static void AddBoostAuraReplacement(this SkinDef skin, GameObject boostAura)
        {
            boostAuraReplacements.Add(skin, boostAura);
        }
    }
}