using EntityStates;
using RoR2;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.Initial
{
    public class Descent : BaseState
    {
        public const float duration = 3f;
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(DLC3Content.Buffs.Untargetable);
            }    
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = true;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && fixedAge >= duration)
            {
                this.outer.SetNextState(new Landed());
            }
        }
        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(DLC3Content.Buffs.Untargetable);
            }
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
            }
            base.OnExit();
        }
    }
}
