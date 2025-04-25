using EntityStates;
using RoR2.UI;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.SkillStates;
using BepInEx.Configuration;
using HedgehogUtils.Boost;
using static HedgehogUtils.Boost.SkillDefs;

namespace SonicTheHedgehog.Components
{
    public class PowerBoostHUD : MonoBehaviour
    {
        //thanks red mist
        private PowerBoostLogic boostLogic;

        public ParticleSystem powerBoostParticle;
        private bool powerBoostParticlePlaying;
        private HUD hud;

        private bool reparentedToBoostHUD;

        private void Awake()
        {
            this.hud = base.GetComponent<HUD>();
        }
        public void Update()
        {
            if (!this.hud.targetBodyObject)
            {
                powerBoostParticle.gameObject.SetActive(false);
                return;
            }
            if (!reparentedToBoostHUD)
            {
                Transform find = hud.transform.Find("MainContainer/MainUIArea/CrosshairCanvas/BoostMeter(Clone)");
                if (find)
                {
                    powerBoostParticle.gameObject.transform.SetParent(find);
                    reparentedToBoostHUD = true;
                    RectTransform rectTransform = powerBoostParticle.GetComponent<RectTransform>();
                    rectTransform.localScale = new Vector3(1, 1, 1);
                    rectTransform.localPosition = Vector3.zero;
                }
            }
            if (!boostLogic && this.hud.targetBodyObject.TryGetComponent<BoostLogic>(out BoostLogic logic) && logic is PowerBoostLogic)
            {
                powerBoostParticle.gameObject.SetActive(true);
                boostLogic = logic as PowerBoostLogic;
                if (boostLogic.boostExists)
                {
                    UpdatePowerParticles();
                }
            }
            else
            {
                if (boostLogic.boostExists)
                {
                    UpdatePowerParticles();
                }
            }
        }

        private void UpdatePowerParticles()
        {
            if (boostLogic.powerBoosting && !powerBoostParticlePlaying && boostLogic.boostDraining)
            {
                powerBoostParticle.Play();
                powerBoostParticlePlaying = true;
            }
            else if ((!boostLogic.powerBoosting || !boostLogic.boostDraining) && powerBoostParticlePlaying)
            {
                powerBoostParticle.Stop();
                powerBoostParticlePlaying = false;
            }
        }
    }
}