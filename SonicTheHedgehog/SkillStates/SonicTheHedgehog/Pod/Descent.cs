using EntityStates.SurvivorPod;
using HedgehogUtils.Emotes;
using SonicTheHedgehog.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace SonicTheHedgehog.SkillStates.Pod
{
    public class Descent : SurvivorPodBaseState
    {
        public const float duration = 3f;
        public override void OnEnter()
        {
            base.OnEnter();
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
            base.OnExit();
        }
    }
}
