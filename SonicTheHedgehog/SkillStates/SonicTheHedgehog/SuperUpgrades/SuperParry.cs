using EntityStates;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperParry : Parry
    {
        public static float baseSuperMaxDuration = StaticValues.superParryMaxDuration;

        public override void OnEnter()
        {
            base.OnEnter();
            this.maxDuration = baseSuperMaxDuration;
            this.enterAnimationPercent = superEnterAnimationPercent;
        }

        protected override void SetNextState(bool success)
        {
            if (skillLocator.secondary.activationState.stateType == typeof(SuperParry))
            {
                this.outer.SetNextState(new SuperParryExit
                {
                    parrySuccess = success
                });
            }
            else
            {
                this.outer.SetNextState(new ParryExit
                {
                    parrySuccess = success
                });
            }
        }
    }
}