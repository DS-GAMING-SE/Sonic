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
        public PowerBoostLogic boostLogic;

        private ParticleSystem powerBoostParticle;
        private bool powerBoostParticlePlaying;

        public void Awake()
        {
            powerBoostParticle = GetComponent<ParticleSystem>();
        }
        public void Update()
        {
            if (boostLogic && boostLogic.boostExists)
            {
                UpdatePowerParticles();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Reparent(Transform boostHUD)
        {
            transform.SetParent(boostHUD);
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.localPosition = Vector3.zero;
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