using HedgehogUtils.Boost;
using RoR2;
using RoR2.HudOverlay;
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
        private OverlayController powerBoostOverlayController;

        private GameObject boostHUD;
        public new void OnDestroy()
        {
            base.OnDestroy();
            powerBoostOverlayController.onInstanceAdded -= PreparePowerBoostHUD;
        }
        protected override void BoostHUDCreated(GameObject hud)
        {
            boostHUD = hud;
            powerBoostOverlayController = HudOverlayManager.AddOverlay(base.gameObject, new OverlayCreationParams
            {
                prefab = Modules.Assets.powerBoostHud,
                childLocatorEntry = "CrosshairExtras"
            });
            powerBoostOverlayController.onInstanceAdded += PreparePowerBoostHUD;
        }
        private void PreparePowerBoostHUD(OverlayController overlayController, GameObject instance)
        {
            if (instance.TryGetComponent<PowerBoostHUD>(out var powerBoostHud))
            {
                if (boostHUD)
                {
                    powerBoostHud.Reparent(boostHUD.transform);
                }
                powerBoostHud.boostLogic = this;
            }

        }
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