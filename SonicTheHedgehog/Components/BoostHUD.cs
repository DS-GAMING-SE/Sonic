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

namespace SonicTheHedgehog.Components
{
    public class BoostHUD : MonoBehaviour
    {
        //thanks red mist
        public GameObject boostMeter;
        public GameObject boostMeterFillInfinite;
        public Image boostMeterFill;
        public Image boostMeterBackground;
        private Color fillDefaultColor = new Color(0, 0.9f, 1, 1);
        private Color fillFadeColor = new Color(0, 0.9f, 1, 0);
        private Color fillUnavailableColor = new Color(0.8f, 0, 0, 1);
        private Color backgroundDefaultColor = new Color (0, 0, 0, 0.5f);
        private HUD hud;
        private float fadeTimer;

        private void Awake()
        {
            this.hud = base.GetComponent<HUD>();
        }
        public void Update()
        {
            if (!this.hud.targetBodyObject)
            {
                return;
            }
            BoostLogic boostLogic = this.hud.targetBodyObject.GetComponent<BoostLogic>();
            if (boostLogic)
            {
                if (this.hud.targetMaster)
                {
                    PlayerCharacterMasterController playerCharacterMasterController = this.hud.targetMaster.playerCharacterMasterController;
                }
                if (this.boostMeter)
                {
                    this.boostMeter.gameObject.SetActive(true);
                    boostMeter.GetComponent<Slider>().value = boostLogic.boostMeter;
                    if (boostLogic.boostRegen < Boost.boostMeterDrain)
                    {
                        boostMeterFillInfinite.GetComponent<Image>().enabled = false;
                        if (boostLogic.boostMeter >= boostLogic.maxBoostMeter)
                        {
                            fadeTimer += Time.fixedDeltaTime;
                            boostMeterFill.color = Color.Lerp(fillDefaultColor, fillFadeColor, fadeTimer);
                            boostMeterBackground.color = Color.Lerp(backgroundDefaultColor, new Color(0, 0, 0, 0), fadeTimer);

                        }
                        else
                        {
                            fadeTimer = 0;
                            boostMeterFill.color = boostLogic.boostAvailable ? fillDefaultColor : fillUnavailableColor;
                            boostMeterBackground.color = backgroundDefaultColor;
                        }
                    }
                    else
                    {
                        boostMeterFillInfinite.GetComponent<Image>().enabled = true;
                    }
                    return;
                }
            }
            else if (this.boostMeter)
            {
                this.boostMeter.gameObject.SetActive(false);
            }
        }
    }
}