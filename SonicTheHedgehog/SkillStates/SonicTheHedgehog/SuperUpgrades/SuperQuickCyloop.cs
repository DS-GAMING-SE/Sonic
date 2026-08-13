using EntityStates;
using R2API;
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
    public class SuperQuickCyloop : Cyloop.QuickCyloop
    {
        public override Color cyloopTrailColor { get { return SonicTheHedgehogCharacter.superSonicColor; } }
        public override Color cyloopTrailIntersectColor { get { return new Color(0.2f, 0.6f, 1f); } }
        public override float cyloopTrailSizeMultiplier { get { return 2f; } }
        public override GameObject hitEffectPrefab { get { return CyloopManager.GetSuperCyloopHitVFX(); } }
        public override GameObject doubleHitEffectPrefab { get { return Modules.Assets.superCyloopDoubleHitWindEffect; } }
        public override Material temporaryOverlayMaterial { get { return null; } }
        public override float cyloopDoublePushForce { get { return 11000f; } }
        public override void PrepareAttack(ref DamageInfo damageInfo)
        {
            base.PrepareAttack(ref damageInfo);
            damageInfo.force = Vector3.up * 50f;
            damageInfo.RemoveModdedDamageType(DamageTypes.cyloop);
            damageInfo.AddModdedDamageType(DamageTypes.superCyloop);
        }
        public override void PrepareDoubleAttack(ref DamageInfo damageInfo)
        {
            base.PrepareDoubleAttack(ref damageInfo);
            damageInfo.damage = StaticValues.superCyloopDoubleDamageCoefficient * damageStat;
        }
    }
}