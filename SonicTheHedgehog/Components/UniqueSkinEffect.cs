using R2API;
using RoR2;
using SonicTheHedgehog.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SonicTheHedgehog.Components
{
    public class UniqueSkinEffect : MonoBehaviour
    {
        public CharacterBody characterBody;
        public Animator animator;
        public ModelSkinController skinController;

        private static List<string> flyingAnimationSkinTokens = new List<string>();
        public static bool[] flyingAnimationSkins;

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
            for (int i = 0; i < skins.Length; i++)
            {
                flyingAnimationSkins[i] = flyingAnimationSkinTokens.Contains(skins[i].nameToken);
            }

            flyingAnimationSkinTokens = null;
        }

        public static void AddFlyingSkin(string nameToken)
        {
            flyingAnimationSkinTokens.Add(nameToken);
        }

        public static void AddFlyingSkin(List<string> nameTokens)
        {
            flyingAnimationSkinTokens.Concat(nameTokens);
        }
    }
}