using RoR2;
using RoR2.Achievements;

namespace SonicTheHedgehog.Modules.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10)]
    public class SonicGrandMasteryAchievement : BaseMasteryUnlockable
    {
        public const string identifier = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "GRANDMASTERYACHIEVEMENT";
        public const string unlockableIdentifier = "GRANDMASTERYUNLOCKABLE";

        public override string RequiredCharacterBody => "SonicTheHedgehog";

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3.5f;
        
    }
}