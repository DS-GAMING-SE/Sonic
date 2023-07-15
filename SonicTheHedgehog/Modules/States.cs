using SonicTheHedgehog.SkillStates;
using SonicTheHedgehog.SkillStates.BaseStates;
using System.Collections.Generic;
using System;

namespace SonicTheHedgehog.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(SonicEntityState));

            Modules.Content.AddEntityState(typeof(SuperSonic));
            Modules.Content.AddEntityState(typeof(BaseSonic));

            Modules.Content.AddEntityState(typeof(SonicMelee));

            Modules.Content.AddEntityState(typeof(SonicBoom));

            Modules.Content.AddEntityState(typeof(Boost));

            Modules.Content.AddEntityState(typeof(GrandSlamDash));
            Modules.Content.AddEntityState(typeof(GrandSlamSpin));
            Modules.Content.AddEntityState(typeof(GrandSlamFinal));

            Modules.Content.AddEntityState(typeof(SuperSonicTransformation));
        }
    }
}