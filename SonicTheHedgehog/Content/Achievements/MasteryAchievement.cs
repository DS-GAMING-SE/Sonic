using RoR2;
using RoR2.Achievements;

namespace SonicTheHedgehog.Modules.Achievements
{
    // HOW IS A MASTERY THIS DIFFICULT
    
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE", "Skins.Sonic.Alt1", null, 10)]
    public class SonicMasteryAchievement : BaseMasteryUnlockable
    {
        public const string identifier = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE";
        public const string unlockableIdentifier = "Skins.Sonic.Alt1";

        public override string RequiredCharacterBody => "SonicTheHedgehog";

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
        
    }
    /*public class SonicMasteryAchievemnt : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }
    }*/
}