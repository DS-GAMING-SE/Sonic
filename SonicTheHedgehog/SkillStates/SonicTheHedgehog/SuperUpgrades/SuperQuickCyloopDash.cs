using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.SkillStates.Cyloop;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperQuickCyloopDash : QuickCyloopDash
    {
        protected override void SetNextState()
        {
            this.outer.SetNextState(new SuperQuickCyloop { target = this.target });
        }
    }
}