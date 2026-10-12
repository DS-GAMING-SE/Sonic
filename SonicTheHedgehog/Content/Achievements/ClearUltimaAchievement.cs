using RoR2.Achievements;
using RoR2;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using UnityEngine;
using SonicTheHedgehog.Modules.Survivors;

namespace SonicTheHedgehog.Modules.Achievements
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, 5U, typeof(SonicClearUltimaServerAchievement))]
    public class SonicClearUltimaAchievement : BasePerSurvivorClearGameHexesAchievement
    {
        public const string identifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "CLEARULTIMAACHIEVEMENT";
        public const string unlockableIdentifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "CLEARULTIMAUNLOCKABLE";
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }
        protected class SonicClearUltimaServerAchievement : BasePerSurvivorClearUltimaAchievement.BasePerSurvivorClearUltimaServerAchievement
        {
        }
    }
}