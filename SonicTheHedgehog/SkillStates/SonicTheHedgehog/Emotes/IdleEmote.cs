using HedgehogUtils.Emotes;
using SonicTheHedgehog.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SonicTheHedgehog.SkillStates.Emotes
{
    public class IdleEmote : BaseOverlayEmote
    {
        private Animator modelAnimator;
        public override void OnEnter()
        {
            base.OnEnter();
            modelAnimator = base.GetModelAnimator();
        }
        public override void OnExit()
        {
            base.OnExit();
        }
        public override void PlayEmoteAnimation()
        {
            PlayAnimation("Body", "IdleExtra");
        }
        public override float animationDuration => 7.5f;
        public override bool ShouldInterrupt()
        {
            return HedgehogUtils.Helpers.IsDoingSomething(characterMotor, inputBank, true, modelAnimator.GetFloatString("isSuperFloat") == 1f, false, false);
        }
    }
}
