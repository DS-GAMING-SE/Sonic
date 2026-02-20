using EntityStates.SurvivorPod;
using HedgehogUtils.Emotes;
using RoR2;
using SonicTheHedgehog.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SonicTheHedgehog.SkillStates.Pod
{
    public class Landed : SurvivorPodBaseState
    {
        public const float startup = 1.5f;
        public override void OnEnter()
        {
            base.OnEnter();
            Transform seatTransform = transform.GetChild(1);
            EffectManager.SimpleEffect(HedgehogUtils.Assets.launchWallCollisionEffect, seatTransform.position - new Vector3(0, 0.4f, 0), seatTransform.rotation, false);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && fixedAge >= startup)
            {
                base.vehicleSeat.handleVehicleExitRequestServer.AddCallback(new CallbackCheck<bool, GameObject>.CallbackDelegate(this.HandleVehicleExitRequest));
                base.survivorPodController.exitAllowed = true;
            }
        }
        private void HandleVehicleExitRequest(GameObject gameObject, ref bool? result)
        {
            base.survivorPodController.exitAllowed = false;
            this.outer.SetNextState(new Release());
            result = new bool?(true);
        }
        public override void OnExit()
        {
            base.survivorPodController.exitAllowed = false;
            base.OnExit();
        }
    }
}
