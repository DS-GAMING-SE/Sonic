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

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperBoostIdle : BoostIdle
    {
        public override void EnterBoost()
        {
            outer.SetNextState(EntityStateCatalog.InstantiateState(typeof(NewSuperBoost)));
        }
    }
}