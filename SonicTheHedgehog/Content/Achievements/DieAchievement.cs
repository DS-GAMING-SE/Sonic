using RoR2.Achievements;
using RoR2;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SonicTheHedgehog.Modules.Achievements
{
    [RegisterAchievement(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICSAILORUNLOCKABLE", "Skin.Sonic.Sailor", null, 1)]
    public class SonicDieAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }

        public override void OnBodyRequirementMet()
        {
            GlobalEventManager.onCharacterDeathGlobal += SonicMurdered;
        }

        public override void OnBodyRequirementBroken()
        {
            GlobalEventManager.onCharacterDeathGlobal -= SonicMurdered;
        }

        private void SonicMurdered(DamageReport damageReport)
        {
            if (damageReport.victimBody == base.localUser.cachedBody)
            {
                Grant();
            }
        }
    }
}