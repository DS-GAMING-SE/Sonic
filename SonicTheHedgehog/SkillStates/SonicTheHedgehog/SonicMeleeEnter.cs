using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicMeleeEnter : BaseState
    {
        // Add persistent swing index, config option for key-press homing attack
        private HomingTracker homingTracker;
        private int swingIndex;
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                homingTracker = base.characterBody.GetComponent<HomingTracker>();
                if (homingTracker && homingTracker.CanHomingAttack())
                {
                    this.outer.SetNextState(new HomingAttack());
                }
                else
                {
                    this.outer.SetNextState(new SonicMelee());
                }
            }
        }
    }
}