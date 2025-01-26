using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;
using HedgehogUtils.Boost;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicBrake : HedgehogUtils.Boost.EntityStates.Brake
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = true;
            }
            if (base.GetModelAnimator().GetFloat("isSuperFloat") == 0)
            {
                Util.PlaySound("Play_sonicthehedgehog_brake", base.gameObject);
            }
            else
            {
                Util.PlaySound("Play_sonicthehedgehog_swing_low", base.gameObject);
            }
        }

        public override void OnExit()
        {
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = false;
            }
            base.OnExit();
        }
    }
}