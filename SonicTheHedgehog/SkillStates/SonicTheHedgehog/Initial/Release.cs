using EntityStates;

namespace SonicTheHedgehog.SkillStates.Initial
{
    public class Release : BaseState
    {
        public const float duration = 1.3f;
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = true;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
            }
            base.OnExit();
        }
    }
}
