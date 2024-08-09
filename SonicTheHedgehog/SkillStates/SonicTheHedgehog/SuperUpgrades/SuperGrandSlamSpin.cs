using EntityStates;
using R2API.Networking.Interfaces;
using Rewired;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Modules;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperGrandSlamSpin : GrandSlamSpin
    {
        protected override void SetNextState()
        {
            if (base.skillLocator.special.activationState.stateType == typeof(SuperGrandSlamDash))
            {
                this.outer.SetNextState(new SuperGrandSlamFinal
                {
                    target = this.target
                });
            }
            else
            {
                this.outer.SetNextState(new GrandSlamFinal
                {
                    target = this.target
                });
            }
        }
    }
}