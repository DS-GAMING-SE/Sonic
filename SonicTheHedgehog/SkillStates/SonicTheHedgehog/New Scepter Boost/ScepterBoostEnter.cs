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
    public class ScepterBoostEnter : HedgehogUtils.Boost.EntityStates.BoostEnter
    {
        public override void EnterBoostIdle()
        {
            outer.SetNextState(EntityStateCatalog.InstantiateState(typeof(ScepterBoostIdle)));
        }
        public override void EnterBoost()
        {
            outer.SetNextState(EntityStateCatalog.InstantiateState(typeof(NewScepterBoost)));
        }
        public override void EnterAirBoost()
        {
            NewScepterBoost airBoost = (NewScepterBoost)EntityStateCatalog.InstantiateState(typeof(NewScepterBoost));
            airBoost.airBoosting = true;
            outer.SetNextState(airBoost);
        }
    }
}