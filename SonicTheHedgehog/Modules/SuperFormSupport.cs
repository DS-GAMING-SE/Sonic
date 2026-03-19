using System;
using System.Collections.Generic;
using System.Text;
using HedgehogUtils.Forms.SuperForm;
using HedgehogUtils.Forms;
using SonicTheHedgehog.Modules.Survivors;
using MonoMod.RuntimeDetour;
using EntityStates.AffixVoid;
using HedgehogUtils.Forms.SuperForm.EntityStates;
using SonicTheHedgehog.SkillStates;
using System.Reflection;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace SonicTheHedgehog.Modules
{
    public static class SuperFormSupport
    {
        public static void Initialize()
        {
            Forms.AddSkinForForm(SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME", 
                new RenderReplacements { material = Materials.CreateHopooMaterial("matSuperSonic"), mesh = Assets.superSonicMesh }, 
                ref SuperFormDef.superFormDef);
            Addressables.LoadAssetAsync<Material>(SkinAddressables.superMetalMaterial).Completed += (x) =>
            {
                Forms.AddSkinForForm(SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME",
                new RenderReplacements { material = x.Result, mesh = null },
                ref SuperFormDef.superFormDef);
            };
        }
    }
}
