using R2API;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SonicTheHedgehog.SkillStates
{
    public class NewScepterBoost : ScepterBoostBase
    {
        private const float boostChangeCooldown = 0.4f;

        private float boostChangeCooldownTimer;

        private bool boostChangeEffect;

        protected override BuffDef buff
        {
            get { return Buffs.boostBuff; }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (boostChangeCooldownTimer > 0)
            {
                boostChangeCooldownTimer -= Time.fixedDeltaTime;
            }

            if (boostChangeCooldownTimer <= 0 && (((PowerBoostLogic)boostLogic).powerBoosting ^ PowerBoostLogic.ShouldPowerBoost(healthComponent)))
            {
                RemoveBoostVFX();
                boostChangeEffect = true;
                CreateBoostVFX();
            }
        }

        protected override void CreateBoostVFX()
        {
            ((PowerBoostLogic)boostLogic).UpdatePowerBoosting();
            base.CreateBoostVFX();
            boostChangeCooldownTimer = boostChangeCooldown;
            boostChangeEffect = false;
        }

        public override Material GetOverlayMaterial()
        {
            if (((PowerBoostLogic)boostLogic).powerBoosting)
            {
                return base.GetOverlayMaterial();
            }
            return null;
        }
        public override GameObject GetAuraPrefab()
        {
            if (((PowerBoostLogic)boostLogic).powerBoosting)
            {
                return Modules.Assets.scepterPowerBoostAuraEffect;
            }
            return null;
        }
        public override GameObject GetFlashPrefab()
        {
            if (((PowerBoostLogic)boostLogic).powerBoosting)
            {
                return Modules.Assets.scepterPowerBoostFlashEffect;
            }
            return Modules.Assets.scepterBoostFlashEffect;
        }
        public override string GetSoundString()
        {
            if (boostChangeEffect)
            {
                return "Play_sonicthehedgehog_boost_change";
            }
            else
            {
                return "Play_hedgehogutils_strong_boost";
            }
        }
    }
}
