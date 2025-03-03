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
    public class ScepterSuperBoostEnter : HedgehogUtils.Boost.EntityStates.BoostEnter
    {
        public override void EnterBoostIdle()
        {
            outer.SetNextState(EntityStateCatalog.InstantiateState(typeof(ScepterSuperBoostIdle)));
        }
        public override void EnterBoost()
        {
            outer.SetNextState(EntityStateCatalog.InstantiateState(typeof(NewScepterSuperBoost)));
        }
        public override void EnterAirBoost()
        {
            NewScepterSuperBoost airBoost = (NewScepterSuperBoost)EntityStateCatalog.InstantiateState(typeof(NewScepterSuperBoost));
            airBoost.airBoosting = true;
            outer.SetNextState(airBoost);
        }
    }
}