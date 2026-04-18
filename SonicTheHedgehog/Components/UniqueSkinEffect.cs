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
        public static bool[] flyingAnimationSkins;

        private static Dictionary<string, SuperGrandSlamMeshReplacements> superGrandSlamMeshReplacementTokens = new Dictionary<string, SuperGrandSlamMeshReplacements>();
        public static SuperGrandSlamMeshReplacements[] superGrandSlamMeshReplacements;


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
            if (flyingAnimationSkins[skinIndex])
            {
                animator.SetFloat("isMetalSonic", 1);
            }
        }
        public static void Bake()
        {
            RoR2.UI.MainMenu.MainMenuController.OnMainMenuInitialised -= UniqueSkinEffect.Bake;
            SkinDef[] skins = SkinCatalog.GetBodySkinDefs(BodyCatalog.FindBodyIndex("SonicTheHedgehog"));
            flyingAnimationSkins = new bool[skins.Length];
            superGrandSlamMeshReplacements = new SuperGrandSlamMeshReplacements[skins.Length];
            for (int i = 0; i < skins.Length; i++)
            {
                flyingAnimationSkins[i] = flyingAnimationSkinTokens.Contains(skins[i].nameToken);
                if (superGrandSlamMeshReplacementTokens.TryGetValue(skins[i].nameToken, out var meshReplacement)) superGrandSlamMeshReplacements[i] = meshReplacement;
            }

            flyingAnimationSkinTokens = null;
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
    }
}