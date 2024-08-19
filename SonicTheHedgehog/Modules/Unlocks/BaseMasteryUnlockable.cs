using RoR2;
using RoR2.Achievements;
namespace SonicTheHedgehog.Modules
{
    public abstract class BaseMasteryUnlockable : BaseAchievement
    {
        public abstract string RequiredCharacterBody { get; }
        public abstract float RequiredDifficultyCoefficient { get; }

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            Run.onClientGameOverGlobal += OnClientGameOverGlobal;
        }
        public override void OnBodyRequirementBroken()
        {
            Run.onClientGameOverGlobal -= OnClientGameOverGlobal;
            base.OnBodyRequirementBroken();
        }
        private void OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if (runReport.gameEnding && runReport.gameEnding.isWin)
            {
                DifficultyIndex difficultyIndex = runReport.ruleBook.FindDifficulty();
                DifficultyDef runDifficulty = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());
                //checking run difficulty
                if (runDifficulty != null &&
                    ((runDifficulty.countsAsHardMode && runDifficulty.scalingValue >= RequiredDifficultyCoefficient) ||
                    //checking for eclipse
                    (difficultyIndex >= DifficultyIndex.Eclipse1 && difficultyIndex <= DifficultyIndex.Eclipse8) ||
                    //checking for modded difficulty Inferno
                    (runDifficulty.nameToken == "INFERNO_NAME")))
                {
                    Grant();
                }
            }
        }

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(RequiredCharacterBody);
        }
    }
}