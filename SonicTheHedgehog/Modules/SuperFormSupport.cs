using System;
using System.Collections.Generic;
using System.Text;
using HedgehogUtils.Forms.SuperForm;
using HedgehogUtils.Forms;
using SonicTheHedgehog.Modules.Survivors;

namespace SonicTheHedgehog.Modules
{
    public static class SuperFormSupport
    {
        public static void Initialize()
        {
            Forms.AddSkinForForm(SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME", 
                new RenderReplacements { material = Materials.CreateHopooMaterial("matSuperSonic"), mesh = Assets.superSonicMesh }, 
                ref SuperFormDef.superFormDef);
            Forms.AddSkinForForm(SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME",
                new RenderReplacements { material = Materials.CreateHopooMaterial("matSuperMetalSonic"), mesh = null },
                ref SuperFormDef.superFormDef);
            SuperFormDef.AddChaosEmeraldSpawningCharacter(new List<string> { "SonicTheHedgehog" });
        }
    }
}
