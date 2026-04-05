using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.Initial
{
    public class Landed : BaseState
    {
        public const float startup = 1.5f;
        public override void OnEnter()
        {
            base.OnEnter();
            EffectManager.SimpleEffect(HedgehogUtils.Assets.launchWallCollisionEffect, base.characterBody.footPosition, modelLocator.modelTransform.rotation, false);
            GameObject.Instantiate(Modules.Assets.faceplantDecal, base.characterBody.footPosition, modelLocator.modelTransform.rotation);
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
            if (base.isAuthority && fixedAge >= startup && HedgehogUtils.Helpers.IsDoingSomething(base.characterMotor, base.inputBank, true, false, false, false))
            {
                this.outer.SetNextState(new Release());
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
