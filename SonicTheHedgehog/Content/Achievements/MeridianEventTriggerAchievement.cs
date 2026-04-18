using RoR2.Achievements;
using RoR2;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using UnityEngine;
using SonicTheHedgehog.Modules.Survivors;

namespace SonicTheHedgehog.Modules.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 5U, typeof(SonicMeridianEventTriggerServerAchievement))]
    public class SonicMeridianEventTriggerAchievement : BaseAchievement
    {
        public const string identifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MERIDIANACHIEVEMENT";
        public const string unlockableIdentifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MERIDIANUNLOCKABLE";

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            base.SetServerTracked(true);
        }

        public override void OnBodyRequirementBroken()
        {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }

        // Token: 0x020014A2 RID: 5282
        private class SonicMeridianEventTriggerServerAchievement : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                EntityStates.FalseSonBoss.SkyJumpDeathState.falseSonDeathEvent += this.OnMeridianEventTriggerActivated;
            }

            public override void OnUninstall()
            {
                base.OnUninstall();
                EntityStates.FalseSonBoss.SkyJumpDeathState.falseSonDeathEvent -= this.OnMeridianEventTriggerActivated;
            }

            private void OnMeridianEventTriggerActivated()
            {
                base.Grant();
            }
        }
    }
}