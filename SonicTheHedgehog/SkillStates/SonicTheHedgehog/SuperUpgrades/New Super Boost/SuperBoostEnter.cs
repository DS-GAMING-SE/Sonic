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
    public class SuperBoostEnter : HedgehogUtils.Boost.EntityStates.BoostEnter
    {
        public override void EnterBoostIdle()
        {
            outer.SetNextState(EntityStateCatalog.InstantiateState(typeof(SuperBoostIdle)));
        }
        public override void EnterBoost()
        {
            outer.SetNextState(EntityStateCatalog.InstantiateState(typeof(NewSuperBoost)));
        }
        public override void EnterAirBoost()
        {
            NewSuperBoost airBoost = (NewSuperBoost)EntityStateCatalog.InstantiateState(typeof(NewSuperBoost));
            airBoost.airBoosting = true;
            outer.SetNextState(airBoost);
        }
    }
}