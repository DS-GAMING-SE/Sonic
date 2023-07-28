using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class Death : GenericCharacterDeath
    {
        
        private const float destroyTime=1.6f;
        public override void OnEnter()
        {
            base.OnEnter();
            cameraTargetParams.cameraPivotTransform = base.gameObject.transform;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
            }

            if (base.fixedAge >= destroyTime && NetworkServer.active)
            {
                DestroyBodyAsapServer();
            }
        }
        public override void OnExit()
        {
            base.DestroyModel();
            base.OnExit();
        }
    }
}