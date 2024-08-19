using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperBoost : Boost
    {
        protected override bool drainBoostMeter
        {
            get { return false; }
        }

        protected override bool ShouldPowerBoost()
        {
            return Moving();
        }

        public override GameObject GetEffectPrefab(bool power)
        {
            return Assets.superBoostFlashEffect;
        }

        public override Material GetOverlayMaterial()
        {
            return LegacyResourcesAPI.Load<Material>("Materials/matStrongerBurn");
        }
    }
}