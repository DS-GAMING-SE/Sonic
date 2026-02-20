using EntityStates.SurvivorPod;
using HedgehogUtils.Emotes;
using RoR2;
using SonicTheHedgehog.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.Pod
{
    public class Release : SurvivorPodBaseState
    {
        public const float duration = 1.3f;
        public override void OnEnter()
        {
            base.OnEnter();
            if (!base.survivorPodController)
            {
                return;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration)
            {
                if (!base.survivorPodController)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
                if (NetworkServer.active)
                {
                    if (base.vehicleSeat && base.vehicleSeat.currentPassengerBody)
                    {
                        base.vehicleSeat.EjectPassenger(base.vehicleSeat.currentPassengerBody.gameObject);
                    }
                    else
                    {
                        this.outer.SetNextStateToMain();
                    }
                }
            }
        }
    }
}
