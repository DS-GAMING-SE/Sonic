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
    public class NewSuperBoost : HedgehogUtils.Boost.EntityStates.Boost
    {
        private string jumpSoundString = "Play_sonicthehedgehog_jump";

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

        public override void OnEnter()
        {
            //base.characterMotor.onHitGroundAuthority += OnHitGround;
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

        /*public override void ProcessJump()
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
        }*/

        public override Material GetOverlayMaterial()
        {
            return LegacyResourcesAPI.Load<Material>("Materials/matStrongerBurn");
        }
        public override GameObject GetAuraPrefab()
        {
            return Modules.Assets.superBoostAuraEffect;
        }
        public override GameObject GetFlashPrefab()
        {
            return Modules.Assets.superBoostFlashEffect;
        }

        protected override void SetBrakeState(Vector3 endDirection)
        {
            SonicBrake brake = new SonicBrake();
            brake.endDirection = endDirection;
            outer.SetNextState(brake);
        }
    }
}