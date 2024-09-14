using SonicTheHedgehog.SkillStates;
using SonicTheHedgehog.SkillStates.SuperUpgrades;
using System.Collections.Generic;
using System;

namespace SonicTheHedgehog.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(InteractablePurchased));

            Modules.Content.AddEntityState(typeof(SonicEntityState));

            Modules.Content.AddEntityState(typeof(Death));

            Modules.Content.AddEntityState(typeof(SuperSonic));
            Modules.Content.AddEntityState(typeof(SonicFormBase));
            Modules.Content.AddEntityState(typeof(BaseSonic));

            Modules.Content.AddEntityState(typeof(SonicMeleeEnter));
            Modules.Content.AddEntityState(typeof(SonicMelee));
            Modules.Content.AddEntityState(typeof(HomingAttack));

            Modules.Content.AddEntityState(typeof(SuperSonicMeleeEnter));
            Modules.Content.AddEntityState(typeof(SuperSonicMelee));
            Modules.Content.AddEntityState(typeof(SuperHomingAttack));

            Modules.Content.AddEntityState(typeof(SonicBoom));

            Modules.Content.AddEntityState(typeof(SuperSonicBoom));

            Modules.Content.AddEntityState(typeof(Parry));
            Modules.Content.AddEntityState(typeof(ParryExit));

            Modules.Content.AddEntityState(typeof(SuperParry));
            Modules.Content.AddEntityState(typeof(SuperParryExit));
            Modules.Content.AddEntityState(typeof(IDWAttackSearch));
            Modules.Content.AddEntityState(typeof(IDWAttack));

            Modules.Content.AddEntityState(typeof(Boost));
            Modules.Content.AddEntityState(typeof(ScepterBoost));

            Modules.Content.AddEntityState(typeof(SuperBoost));
            Modules.Content.AddEntityState(typeof(SuperScepterBoost));

            Modules.Content.AddEntityState(typeof(GrandSlamDash));
            Modules.Content.AddEntityState(typeof(GrandSlamSpin));
            Modules.Content.AddEntityState(typeof(GrandSlamFinal));

            Modules.Content.AddEntityState(typeof(SuperGrandSlamDash));
            Modules.Content.AddEntityState(typeof(SuperGrandSlamSpin));
            Modules.Content.AddEntityState(typeof(SuperGrandSlamFinal));

            Modules.Content.AddEntityState(typeof(SuperSonicTransformation));
        }
    }
}