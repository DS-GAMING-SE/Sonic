using EntityStates;
using RoR2;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperSonicBoom : SonicBoom
    {
        private static float superDamageCoefficient = Modules.StaticValues.superSonicBoomDamageCoefficient;
        public static float baseSuperProjectileSpeed = 135f;

        public override float baseMovementReduction
        {
            get { return 0.85f; }
        }

        protected override void FireProjectile()
        {
            RoR2.Projectile.ProjectileManager.instance.FireProjectile(ProjectilePrefab(), base.GetAimRay().origin, ProjectileRotation(), base.gameObject,
                superDamageCoefficient * this.damageStat, force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, baseSuperProjectileSpeed);
        }

        protected override void FireProjectileMuzzleFlash()
        {

        }

        protected override GameObject ProjectilePrefab()
        {
            return Projectiles.superSonicBoomPrefab;
        }

        protected override Quaternion ProjectileRotation()
        {
            Quaternion direction = Util.QuaternionSafeLookRotation(base.GetAimRay().direction);
            Vector3 up = direction * Vector3.up;

            return Util.QuaternionSafeLookRotation(base.GetAimRay().direction, up);
        }

        protected override void SetNextState()
        {
            if (skillLocator.secondary.activationState.stateType == typeof(SuperSonicBoom))
            {
                this.outer.SetNextState(new SuperSonicBoom
                {
                    easedIn = true
                });
            }
            else
            {
                this.outer.SetNextState(new SonicBoom
                {
                    easedIn = true
                });
            }
        }
    }
}