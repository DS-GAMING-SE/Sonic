using HedgehogUtils.Emotes;
using SonicTheHedgehog.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace SonicTheHedgehog.SkillStates.Emotes
{
    public class IdleEmote : BaseBodyEmote
    {
        private HomingTracker homingTracker;
        public override void OnEnter()
        {
            base.OnEnter();
            homingTracker = GetComponent<HomingTracker>();
            if (homingTracker)
            {
                homingTracker.visible = true;
            }
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = true;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
            }
            if (homingTracker)
            {
                homingTracker.visible = false;
            }
        }
        public override void PlayEmoteAnimation()
        {
            PlayAnimation("Body", "IdleExtra");
        }
        public override float animationDuration => 7.5f;
    }
}
