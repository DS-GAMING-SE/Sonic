using RoR2.Achievements;
using RoR2;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using UnityEngine;
using SonicTheHedgehog.Modules.Survivors;

namespace SonicTheHedgehog.Modules.Achievements
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, 5U, typeof(SonicClearChampRushServerAchievement))]
    public class SonicClearChampRushAchievement : BasePerSurvivorClearGameHexesAchievement
    {
        public const string identifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "CLEARCHAMPRUSHACHIEVEMENT";
        public const string unlockableIdentifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "CLEARCHAMPRUSHUNLOCKABLE";
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }
        protected class SonicClearChampRushServerAchievement : BasePerSurvivorClearChampRushAchievement.BasePerSurvivorClearChampRushServerAchievement
        {
        }
    }
}