using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperSonicMeleeEnter : SonicMeleeEnter
    {
        protected override Type meleeStateType
        {
            get { return typeof(SuperSonicMelee); }
        }

        protected override Type homingAttackStateType
        {
            get { return typeof(SuperHomingAttack); }
        }
    }
}