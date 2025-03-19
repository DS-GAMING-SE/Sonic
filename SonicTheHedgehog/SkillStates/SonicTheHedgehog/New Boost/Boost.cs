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

namespace SonicTheHedgehog.SkillStates
{
    public class NewBoost : HedgehogUtils.Boost.EntityStates.Boost
    {
        private string jumpSoundString = "Play_sonicthehedgehog_jump";

        private const float boostChangeCooldown = 0.4f;

        private float boostChangeCooldownTimer;

        private bool boostChangeEffect;

        protected override BuffDef buff
        {
            get { return Buffs.boostBuff; }
        }

        public override void OnEnter()
        {
            base.characterMotor.onHitGroundAuthority += OnHitGround;
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

        public override void OnExit()
        {
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = false;
            }
            base.OnExit();
        }

        public override void ProcessJump()
        {
            if (base.isAuthority && this.hasCharacterMotor && this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
            {
                Util.PlaySound(jumpSoundString, base.gameObject);
                base.modelAnimator.SetBool("isBall", true);
            }
            base.ProcessJump();
        }

        private void OnHitGround(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            if (base.modelAnimator.GetBool("isBall"))
            {
                base.modelAnimator.SetBool("isBall", false);
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
                return base.GetAuraPrefab();
            }
            return null;
        }
        public override GameObject GetFlashPrefab()
        {
            if (((PowerBoostLogic)boostLogic).powerBoosting)
            {
                return base.GetFlashPrefab();
            }
            return Modules.Assets.boostFlashEffect;
        }
        public override string GetSoundString()
        {
            if (boostChangeEffect)
            {
                return "Play_sonicthehedgehog_boost_change";
            }
            else
            {
                return "Play_hedgehogutils_boost";
            }
        }
    }
}