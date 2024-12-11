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
    public class SuperScepterBoost : ScepterBoost
    {
        protected override bool drainBoostMeter
        {
            get { return false; }
        }

        protected override float damageRadius
        {
            get { return 6f; }
        }

        protected override void UpdatePowerBoosting()
        {
            if (!powerBoosting && Moving())
            {
                base.characterBody.MarkAllStatsDirty();
                powerBoosting = true;
                OnPowerBoostChanged();
                return;
            }
            if (powerBoosting && !Moving())
            {
                base.characterBody.MarkAllStatsDirty();
                powerBoosting = false;
                OnPowerBoostChanged();
                return;
            }
        }

        public override GameObject GetFlashPrefab(bool power)
        {
            return Modules.Assets.scepterSuperBoostFlashEffect;
        }
        public override GameObject GetAuraPrefab(bool power)
        {
            return Modules.Assets.scepterSuperBoostAuraEffect;
        }

        public override Material GetOverlayMaterial()
        {
            return LegacyResourcesAPI.Load<Material>("Materials/matStrongerBurn");
        }
    }
}