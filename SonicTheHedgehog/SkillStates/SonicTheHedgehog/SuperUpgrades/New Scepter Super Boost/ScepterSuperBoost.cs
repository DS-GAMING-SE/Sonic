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
    public class NewScepterSuperBoost : ScepterBoostBase
    {
        protected override BuffDef buff
        {
            get { return Buffs.superBoostBuff; }
        }

        protected override float boostMeterDrain
        {
            get { return 0; }
        }
        protected override float boostStartMeterDrain
        {
            get { return 0; }
        }

        protected override string hitBoxName
        {
            get { return "LargeBall"; }
        }

        protected override float launchForce
        {
            get { return 1000f; }
        }

        public override Material GetOverlayMaterial()
        {
            return LegacyResourcesAPI.Load<Material>("Materials/matStrongerBurn");
        }
        public override GameObject GetAuraPrefab()
        {
            return Modules.Assets.scepterSuperBoostAuraEffect;
        }
        public override GameObject GetFlashPrefab()
        {
            return Modules.Assets.scepterSuperBoostFlashEffect;
        }
    }
}