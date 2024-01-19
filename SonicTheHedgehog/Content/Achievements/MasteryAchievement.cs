using RoR2;
using SonicTheHedgehog.Modules.Survivors;
using System;
using UnityEngine;

namespace SonicTheHedgehog.Modules.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    /*[RegisterAchievement(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE", "SonicSkins.Mastery", null, null)]
    public class SonicMasteryAchievement : RoR2.Achievements.BasePerSurvivorClearGameMonsoonAchievement // I don't understand why this doesn't work
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }
    }*/
    [RegisterAchievement(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE", "SonicSkins.Mastery", null, null)]
    public class SonicMasteryAchievement : BaseMasteryUnlockable
    {
        public const string identifier = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE";
        public const string unlockableIdentifier = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE";

        public override string RequiredCharacterBody => "SonicTheHedgehog";

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}