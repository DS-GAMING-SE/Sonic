using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicMeleeEnter : BaseState
    {
        private HomingTracker homingTracker;
        public int swingIndex = 0;

        protected virtual Type homingAttackStateType
        {
            get { return typeof(HomingAttack); }
        }
        protected virtual Type meleeStateType
        {
            get { return typeof(SonicMelee); }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                homingTracker = base.characterBody.GetComponent<HomingTracker>();
                if (homingTracker && homingTracker.CanHomingAttack())
                {
                    HomingAttack homingState = (HomingAttack)EntityStateCatalog.InstantiateState(homingAttackStateType);
                    homingState.target = homingTracker.GetTrackingTarget();
                    this.outer.SetNextState(homingState);
                }
                else
                {
                    SonicMelee meleeState = (SonicMelee)EntityStateCatalog.InstantiateState(meleeStateType);
                    meleeState.swingIndex = this.swingIndex;
                    this.outer.SetNextState(meleeState);
                }
            }
        }
    }
}