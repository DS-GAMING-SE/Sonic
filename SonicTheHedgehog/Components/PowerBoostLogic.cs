using RoR2;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.Components
{
    public class PowerBoostLogic : HedgehogUtils.Boost.BoostLogic
    {  
        public bool powerBoosting = false;

        public static bool ShouldPowerBoost(CharacterBody body)
        {
            return ShouldPowerBoost(body.healthComponent);
        }

        public static bool ShouldPowerBoost(HealthComponent health)
        {
            return health.health / health.fullHealth >= 0.9f;
        }

        public void UpdatePowerBoosting()
        {
            powerBoosting = ShouldPowerBoost(body);
        }
    }
}