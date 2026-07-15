using SonicTheHedgehog.SkillStates;
using SonicTheHedgehog.SkillStates.SuperUpgrades;
using SonicTheHedgehog.SkillStates.Emotes;
using SonicTheHedgehog.SkillStates.Initial;
using System.Collections.Generic;
using System;
using SonicTheHedgehog.SkillStates.Cyloop;

namespace SonicTheHedgehog.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(SonicEntityState));

            Modules.Content.AddEntityState(typeof(Descent));
            Modules.Content.AddEntityState(typeof(Landed));
            Modules.Content.AddEntityState(typeof(Release));

            Modules.Content.AddEntityState(typeof(IdleEmote));

            Modules.Content.AddEntityState(typeof(SonicMelee));
            Modules.Content.AddEntityState(typeof(HomingAttack));

            Modules.Content.AddEntityState(typeof(SuperSonicMelee));
            Modules.Content.AddEntityState(typeof(SuperHomingAttack));

            Modules.Content.AddEntityState(typeof(SonicBoom));

            Modules.Content.AddEntityState(typeof(SuperSonicBoom));

            Modules.Content.AddEntityState(typeof(Parry));
            Modules.Content.AddEntityState(typeof(ParryExit));
            Modules.Content.AddEntityState(typeof(FollowUp));

            Modules.Content.AddEntityState(typeof(SuperParry));
            Modules.Content.AddEntityState(typeof(SuperParryExit));
            Modules.Content.AddEntityState(typeof(IDWAttackSearch));
            Modules.Content.AddEntityState(typeof(IDWAttack));
            Modules.Content.AddEntityState(typeof(SuperFollowUp));

            Modules.Content.AddEntityState(typeof(NewBoost));
            Modules.Content.AddEntityState(typeof(BoostIdle));
            Modules.Content.AddEntityState(typeof(SonicBrake));

            Modules.Content.AddEntityState(typeof(NewScepterBoost));

            Modules.Content.AddEntityState(typeof(NewSuperBoost));

            Modules.Content.AddEntityState(typeof(NewScepterSuperBoost));

            Modules.Content.AddEntityState(typeof(GrandSlamDash));
            Modules.Content.AddEntityState(typeof(GrandSlamSpin));
            Modules.Content.AddEntityState(typeof(GrandSlamFinal));

            Modules.Content.AddEntityState(typeof(SuperGrandSlamDash));
            Modules.Content.AddEntityState(typeof(SuperGrandSlamSpin));
            Modules.Content.AddEntityState(typeof(SuperGrandSlamFinal));

            Modules.Content.AddEntityState(typeof(Cyloop));
            Modules.Content.AddEntityState(typeof(QuickCyloopDash));
            Modules.Content.AddEntityState(typeof(QuickCyloop));

            Modules.Content.AddEntityState(typeof(SuperCyloop));
        }
    }
}