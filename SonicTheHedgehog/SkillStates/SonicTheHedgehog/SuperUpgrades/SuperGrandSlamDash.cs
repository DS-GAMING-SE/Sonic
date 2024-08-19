using EntityStates;
using Rewired;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperGrandSlamDash : GrandSlamDash
    {  
        protected override void SetNextState()
        {
            if (base.skillLocator.special.activationState.stateType == typeof(SuperGrandSlamDash))
            {
                this.outer.SetNextState(new SuperGrandSlamSpin
                {
                    target = this.target
                });
            }
            else
            {
                this.outer.SetNextState(new GrandSlamSpin
                {
                    target = this.target
                });
            }
        }
    }
}