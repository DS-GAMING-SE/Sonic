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
    public class SuperHomingAttack : HomingAttack
    {

        protected override float launchPushForce
        {
            get { return 400f; }
        }
        protected override float baseHomingAttackEndLag
        {
            get { return 0.1f; }
        }
    }
}