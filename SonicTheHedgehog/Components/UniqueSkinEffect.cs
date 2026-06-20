using R2API;
using RoR2;
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

        private static List<string> flyingAnimationSkinTokens = new List<string>();
        private static List<string> swordAnimationSkinTokens = new List<string>();
        public static SkinEffects?[] skinEffects;

        private static Dictionary<string, SuperGrandSlamMeshReplacements> superGrandSlamMeshReplacementTokens = new Dictionary<string, SuperGrandSlamMeshReplacements>();


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
        public static void Bake()
        {
            RoR2.UI.MainMenu.MainMenuController.OnMainMenuInitialised -= UniqueSkinEffect.Bake;
            SkinDef[] skins = SkinCatalog.GetBodySkinDefs(BodyCatalog.FindBodyIndex("SonicTheHedgehog"));
            skinEffects = new SkinEffects?[skins.Length];
            bool flying = false;
            bool sword = false;
            for (int i = 0; i < skins.Length; i++)
            {
                flying = flyingAnimationSkinTokens.Contains(skins[i].nameToken);
                sword = swordAnimationSkinTokens.Contains(skins[i].nameToken);

                skinEffects[i] = new SkinEffects
                {
                    flying = flying,
                    sword = sword,
                    superGrandSlamMeshReplacement = superGrandSlamMeshReplacementTokens.TryGetValue(skins[i].nameToken, out var meshReplacement) ? meshReplacement : default
                };
            }

            flyingAnimationSkinTokens = null;
            swordAnimationSkinTokens = null;
            superGrandSlamMeshReplacementTokens = null;
        }

        public static void AddFlyingSkin(string nameToken)
        {
            flyingAnimationSkinTokens.Add(nameToken);
        }

        public static void AddFlyingSkin(List<string> nameTokens)
        {
            flyingAnimationSkinTokens.Concat(nameTokens);
        }
        public static void AddSwordSkin(string nameToken)
        {
            swordAnimationSkinTokens.Add(nameToken);
        }

        public static void AddSwordSkin(List<string> nameTokens)
        {
            swordAnimationSkinTokens.Concat(nameTokens);
        }

        public static void AddSuperGrandSlamMeshReplacement(string nameToken, Mesh mesh)
        {
            superGrandSlamMeshReplacementTokens.Add(nameToken, new SuperGrandSlamMeshReplacements { mesh = mesh });
        }
        public static void AddSuperGrandSlamMeshReplacement(string nameToken, AssetReferenceT<Mesh> meshAddress)
        {
            superGrandSlamMeshReplacementTokens.Add(nameToken, new SuperGrandSlamMeshReplacements { meshAddress = meshAddress });
        }

        public struct SuperGrandSlamMeshReplacements
        {
            public Mesh mesh;
            public AssetReferenceT<Mesh> meshAddress;
        }

        public struct SkinEffects
        {
            public bool flying;
            public bool sword;
            public SuperGrandSlamMeshReplacements superGrandSlamMeshReplacement;
        }
    }
}