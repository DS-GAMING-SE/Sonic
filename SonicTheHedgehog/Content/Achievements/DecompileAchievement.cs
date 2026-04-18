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
    public class SonicDecompileAchievement : BasePerSurvivorDecompileAchievement
    {
        public const string identifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DECOMPILEACHIEVEMENT";
        public const string unlockableIdentifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DECOMPILEUNLOCKABLE";
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }
    }
}