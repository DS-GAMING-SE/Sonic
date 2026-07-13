using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using SonicTheHedgehog.SkillStates.Cyloop;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperCyloop : Cyloop.Cyloop
    {
        public override float cyloopLineIntersectWidth { get { return StaticValues.superCyloopLineIntersectWidth; } }
        public override float cyloopCollisionWidth { get { return StaticValues.superCyloopCollisionWidth; } }
        public override Color cyloopTrailColor { get { return SonicTheHedgehogCharacter.superSonicColor; } }
        public override Color cyloopTrailIntersectColor { get { return new Color(0.2f, 0.6f, 1f); } }
        public override float cyloopTrailSizeMultiplier { get { return 2f; } }

        public override Material temporaryOverlayMaterial { get { return null; } }
        public override void PrepareAttack(ref OverlapAttack overlapAttack)
        {
            base.PrepareAttack(ref overlapAttack);
            overlapAttack.forceVector = Vector3.up * 50f;
        }
    }
}