using RoR2;
using SonicTheHedgehog.Modules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SonicTheHedgehog.Components
{
    public class MetalSonicAnimation : MonoBehaviour
    {
        public CharacterBody characterBody;
        public Animator animator;
        public ModelSkinController skinController;

        public static string[] skinsToHaveMetalSonicAnimations = new string[1];

        private void Start()
        {
            characterBody = GetComponent<CharacterBody>();
            if (characterBody)
            {
                if (characterBody.modelLocator && characterBody.modelLocator.modelTransform)
                {
                    animator = characterBody.modelLocator.modelTransform.GetComponent<Animator>();
                    skinController = characterBody.modelLocator.modelTransform.GetComponent<ModelSkinController>();
                    if (skinsToHaveMetalSonicAnimations.Contains(skinController.skins[characterBody.skinIndex].nameToken))
                    {
                        animator.SetFloat("isMetalSonic", 1);
                    }
                }
            }
        }

        public static void AddSkin(string nameToken)
        {
            Helpers.Append(ref skinsToHaveMetalSonicAnimations, new List<string> { nameToken });
        }

        public static void AddSkin(List<string> nameTokens)
        {
            Helpers.Append(ref skinsToHaveMetalSonicAnimations, nameTokens);
        }
    }
}