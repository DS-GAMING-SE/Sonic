namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperGrandSlamSpin : GrandSlamSpin
    {
        protected override void SetNextState()
        {
            if (base.skillLocator.special.activationState.stateType == typeof(SuperGrandSlamDash))
            {
                this.outer.SetNextState(new SuperGrandSlamFinal
                {
                    target = this.target
                });
            }
            else
            {
                this.outer.SetNextState(new GrandSlamFinal
                {
                    target = this.target
                });
            }
        }
    }
}