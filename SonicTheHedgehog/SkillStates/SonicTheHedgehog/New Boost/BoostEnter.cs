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
    public class BoostEnter : HedgehogUtils.Boost.EntityStates.BoostEnter
    {
        public override void EnterBoostIdle()
        {
            outer.SetNextState(EntityStateCatalog.InstantiateState(typeof(BoostIdle)));
        }
        public override void EnterBoost()
        {
            outer.SetNextState(EntityStateCatalog.InstantiateState(typeof(NewBoost)));
        }
        public override void EnterAirBoost()
        {
            NewBoost airBoost = (NewBoost)EntityStateCatalog.InstantiateState(typeof(NewBoost));
            airBoost.airBoosting = true;
            outer.SetNextState(airBoost);
        }
    }
}