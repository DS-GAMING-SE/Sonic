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

namespace SonicTheHedgehog.Components
{
    public class BoostHUD : MonoBehaviour
    {
        //thanks red mist
        private PowerBoostLogic boostLogic;

        public GameObject boostMeter;

        public Image meterBackground;
        public Image meterFill;

        public Image meterBackgroundOuter;
        public Image meterFillOuter;

        public RawImage meterBackgroundBackup;
        public RawImage meterFillBackup;

        public Image infiniteFill;
        public Image infiniteBackground;

        public ParticleSystem powerBoostParticle;

        private Color fillDefaultColor = new Color(0, 0.9f, 1, 1);
        private Color fillFadeColor = new Color(0, 0.9f, 1, 0);
        private Color fillUnavailableColor = new Color(0.8f, 0, 0, 1);
        private Color backgroundDefaultColor = new Color (0, 0, 0, 0.5f);

        private HUD hud;
        private float fadeTimer;

        private bool powerBoostParticlePlaying=false;

        private int backupBackgroundNum;
        private int backupFillNum;

        private void Awake()
        {
            this.hud = base.GetComponent<HUD>();
            if (SonicTheHedgehogPlugin.riskOfOptionsLoaded)
            {
                ConfigEntry<float> configX = Modules.Config.BoostMeterLocationX();
                ConfigEntry<float> configY = Modules.Config.BoostMeterLocationY();
                configX.SettingChanged += (orig, self) => { this.boostMeter.GetComponent<RectTransform>().anchoredPosition = new Vector2(configX.Value, configY.Value); };
                configY.SettingChanged += (orig, self) => { this.boostMeter.GetComponent<RectTransform>().anchoredPosition = new Vector2(configX.Value, configY.Value); };
            }
        }
        public void Update()
        {
            if (!this.hud.targetBodyObject)
            {
                return;
            }
            boostLogic = this.hud.targetBodyObject.GetComponent<PowerBoostLogic>();
            if (boostLogic && boostLogic.boostExists)
            {
                if (this.hud.targetMaster)
                {
                    PlayerCharacterMasterController playerCharacterMasterController = this.hud.targetMaster.playerCharacterMasterController;
                }
                if (this.boostMeter)
                {
                    this.boostMeter.gameObject.SetActive(true);
                    UpdateMeterBackground();
                    UpdateMeterFill();
                    UpdatePowerParticles();
                    if (boostLogic.boostMeter >= boostLogic.maxBoostMeter && !boostLogic.boostDraining)
                    {
                        meterFill.fillAmount = 1;
                        fadeTimer += Time.fixedDeltaTime;
                        Color fill = Color.Lerp(fillDefaultColor, fillFadeColor, fadeTimer);
                        Color background = Color.Lerp(backgroundDefaultColor, new Color(0, 0, 0, 0), fadeTimer);
                        if (boostLogic.boostRegen < Boost.boostMeterDrain)
                        {
                            meterBackground.gameObject.SetActive(true);
                            meterFill.color = fill;
                            meterBackground.color = background;
                            infiniteBackground.gameObject.SetActive(false);
                        }
                        else
                        {
                            infiniteBackground.gameObject.SetActive(true);
                            infiniteFill.color = fill;
                            infiniteBackground.color = background;
                            meterBackground.gameObject.SetActive(false);
                        }
                        meterFillOuter.color = fill;
                        meterBackgroundOuter.color = background;
                        meterFillBackup.color = fill;
                        meterBackgroundBackup.color = background;

                    }
                    else
                    {
                        fadeTimer = 0;
                        if (boostLogic.boostRegen < Boost.boostMeterDrain)
                        {
                            meterBackground.gameObject.SetActive(true);
                            meterFill.color = boostLogic.boostAvailable ? fillDefaultColor : fillUnavailableColor;
                            meterBackground.color = backgroundDefaultColor;
                            infiniteBackground.gameObject.SetActive(false);
                        }
                        else
                        {
                            infiniteBackground.gameObject.SetActive(true);
                            infiniteFill.color = fillDefaultColor;
                            infiniteBackground.color = backgroundDefaultColor;
                            meterBackground.gameObject.SetActive(false);
                        }
                        meterFillOuter.color = boostLogic.boostAvailable ? fillDefaultColor : fillUnavailableColor;
                        meterBackgroundOuter.color = backgroundDefaultColor;
                        meterFillBackup.color = boostLogic.boostAvailable ? fillDefaultColor : fillUnavailableColor;
                        meterBackgroundBackup.color = backgroundDefaultColor;
                    }
                    return;
                }
            }
            else if (this.boostMeter)
            {
                this.boostMeter.gameObject.SetActive(false);
            }
        }

        private void UpdateMeterFill()
        {
            meterFill.fillAmount = boostLogic.predictedMeter / 100;

            meterFillOuter.fillAmount = ((boostLogic.predictedMeter - 100) % 100) / 100;
            if (boostLogic.maxBoostMeter>100 && boostLogic.maxBoostMeter%100==0 && boostLogic.predictedMeter>=boostLogic.maxBoostMeter)
            {
                meterFillOuter.fillAmount = 1;
            }

            backupFillNum = Math.Max(Mathf.CeilToInt((boostLogic.predictedMeter - 200) / 100),0);
            if (backupFillNum==0)
            {
                meterFillBackup.gameObject.SetActive(false);
            }
            else
            {
                meterFillBackup.uvRect = new Rect(meterBackgroundBackup.uvRect.x, meterBackgroundBackup.uvRect.y, backupFillNum, meterBackgroundBackup.uvRect.height);
                meterFillBackup.gameObject.SetActive(true);
                meterFillBackup.gameObject.GetComponent<RectTransform>().localScale = new Vector3((float)backupFillNum/backupBackgroundNum, 1, 1);
            }
        }

        private void UpdateMeterBackground()
        {
            meterBackground.fillAmount = boostLogic.maxBoostMeter / 100;

            meterBackgroundOuter.fillAmount = ((boostLogic.maxBoostMeter - 100) / 100) - Mathf.Max((Mathf.Floor((boostLogic.predictedMeter - 100) / 100)),0);

            backupBackgroundNum = Math.Max(Mathf.CeilToInt((boostLogic.maxBoostMeter - 200) / 100), 0);
            if (backupBackgroundNum == 0)
            {
                meterBackgroundBackup.gameObject.SetActive(false);
            }
            else
            {
                meterBackgroundBackup.uvRect = new Rect(meterBackgroundBackup.uvRect.x, meterBackgroundBackup.uvRect.y, backupBackgroundNum, meterBackgroundBackup.uvRect.height);
                meterBackgroundBackup.gameObject.SetActive(true);
                meterBackgroundBackup.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.3f * backupBackgroundNum, 0.3f, 0.3f);
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

        private void Reposition()
        {

        }
    }
}