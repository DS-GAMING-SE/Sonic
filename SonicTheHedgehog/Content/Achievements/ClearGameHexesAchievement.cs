using RoR2.Achievements;
using RoR2;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using UnityEngine;
using SonicTheHedgehog.Modules.Survivors;

namespace SonicTheHedgehog.Modules.Achievements
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, 5U, null)]
    public class SonicClearGameHexesAchievement : BasePerSurvivorClearGameHexesAchievement
    {
        public const string identifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "CLEARGAMEHEXESACHIEVEMENT";
        public const string unlockableIdentifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "CLEARGAMEHEXESUNLOCKABLE";
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }
    }
}