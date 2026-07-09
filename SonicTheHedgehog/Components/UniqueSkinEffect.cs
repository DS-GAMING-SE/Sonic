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

        internal static List<SkinDef> flyingAnimationSkinTokens = new List<SkinDef>();
        internal static List<SkinDef> swordAnimationSkinTokens = new List<SkinDef>();
        public static SkinEffects?[] skinEffects;

        internal static Dictionary<SkinDef, AssetOrDirectReference<Mesh>> superGrandSlamMeshReplacementTokens = new Dictionary<SkinDef, AssetOrDirectReference<Mesh>>();
        internal static Dictionary<SkinDef, GameObject> boostFlashReplacementTokens = new Dictionary<SkinDef, GameObject>();
        internal static Dictionary<SkinDef, GameObject> boostAuraReplacementTokens = new Dictionary<SkinDef, GameObject>();

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
                flying = flyingAnimationSkinTokens.Contains(skins[i]);
                sword = swordAnimationSkinTokens.Contains(skins[i]);
                // this always makes a skineffects so there's no point in having it be nullable. does it need to be nullable?
                skinEffects[i] = new SkinEffects
                {
                    flying = flying,
                    sword = sword,
                    superGrandSlamMeshReplacement = superGrandSlamMeshReplacementTokens.GetValueOrDefault(skins[i]),
                    boostFlashEffect = boostFlashReplacementTokens.GetValueOrDefault(skins[i]),
                    boostAuraEffect = boostAuraReplacementTokens.GetValueOrDefault(skins[i])
                };
            }

            flyingAnimationSkinTokens = null;
            swordAnimationSkinTokens = null;
            superGrandSlamMeshReplacementTokens = null;
            boostFlashReplacementTokens = null;
            boostAuraReplacementTokens = null;
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
            flyingAnimationSkinTokens.Add(skin);
        }
        public static void AddSwordSkin(this SkinDef skin)
        {
            swordAnimationSkinTokens.Add(skin);
        }
        public static void AddSuperGrandSlamMeshReplacement(this SkinDef skin, Mesh mesh)
        {
            superGrandSlamMeshReplacementTokens.Add(skin, new AssetOrDirectReference<Mesh> { directRef = mesh });
        }
        public static void AddSuperGrandSlamMeshReplacement(this SkinDef skin, AssetReferenceT<Mesh> meshAddress)
        {
            superGrandSlamMeshReplacementTokens.Add(skin, new AssetOrDirectReference<Mesh> { loadOnAssigned = false, address = meshAddress });
        }
        public static void AddBoostFlashReplacement(this SkinDef skin, GameObject boostFlash)
        {
            boostFlashReplacementTokens.Add(skin, boostFlash);
        }
        public static void AddBoostAuraReplacement(this SkinDef skin, GameObject boostAura)
        {
            boostAuraReplacementTokens.Add(skin, boostAura);
        }
    }
}