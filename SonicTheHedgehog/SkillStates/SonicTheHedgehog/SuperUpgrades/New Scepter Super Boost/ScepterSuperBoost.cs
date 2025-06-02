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
    public class NewScepterSuperBoost : NewScepterBoost
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
            get { return 400f; }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = true;
            }
            if (airBoosting)
            {
                base.characterMotor.disableAirControlUntilCollision = false;
            }
        }

        public override void OnExit()
        {
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = false;
            }
            base.OnExit();
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

        protected override void SetBrakeState(Vector3 endDirection)
        {
            SonicBrake brake = new SonicBrake();
            brake.endDirection = endDirection;
            outer.SetNextState(brake);
        }
    }
}