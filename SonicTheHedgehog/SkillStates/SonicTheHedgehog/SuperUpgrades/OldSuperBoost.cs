using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;

// OUTDATED. See New Super Boost folder for updated ones
namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class OldSuperBoost : OldBoost
    {
        protected override bool drainBoostMeter
        {
            get { return false; }
        }

        protected override BuffDef buff
        {
            get { return Buffs.superBoostBuff; }
        }

        protected override bool ShouldPowerBoost()
        {
            return Moving();
        }

        public override GameObject GetFlashPrefab(bool power)
        {
            return Modules.Assets.superBoostFlashEffect;
        }
        public override GameObject GetAuraPrefab(bool power)
        {
            return Modules.Assets.superBoostAuraEffect;
        }

        public override Material GetOverlayMaterial()
        {
            return LegacyResourcesAPI.Load<Material>("Materials/matStrongerBurn");
        }
    }
}