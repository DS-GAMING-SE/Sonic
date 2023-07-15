using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicBoom : BaseSkillState
    {
        public bool easedIn = false;

        private static float damageCoefficient = Modules.StaticValues.sonicBoomDamageCoefficient;
        private static float superDamageCoefficient = Modules.StaticValues.superSonicBoomDamageCoefficient;
        public static float procCoefficient = 0.75f;
        public static float baseDuration = Modules.StaticValues.sonicBoomFireTime* (Modules.StaticValues.sonicBoomCount*1.23f);
        public static float baseFireTime = Modules.StaticValues.sonicBoomFireTime;
        public static float force = 80f;
        public static float recoil = 0f;
        public static float range = 100f;
        public static float baseMovementReduction=0.3f;
        public static float baseSuperMovementReduction=0.7f;
        public static GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");
        public static GameObject projectilePrefab = Modules.Projectiles.sonicBoomPrefab;

        private float duration;
        private float fireTime;
        private int firedCounter;
        private string muzzleString="SwingCenter";
        private float movementReduction;
        private Vector3 targetVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            this.firedCounter = 0;
            this.duration = SonicBoom.baseDuration / this.attackSpeedStat;
            this.fireTime = baseFireTime / this.attackSpeedStat;
            base.characterBody.SetAimTimer(2f);
            base.PlayAnimation("FullBody, Override", "SonicBoom", "Slash.playbackRate", this.fireTime*Modules.StaticValues.sonicBoomCount);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void Fire()
        {
            if (base.isAuthority)
            {
                StartAimMode();
                this.firedCounter++;
                base.characterBody.AddSpreadBloom(1.5f);
                projectilePrefab = base.characterBody.HasBuff(Modules.Buffs.superSonicBuff) ? Modules.Projectiles.superSonicBoomPrefab : Modules.Projectiles.sonicBoomPrefab;
                EffectManager.SimpleMuzzleFlash(Modules.Assets.sonicBoomKickEffect, base.gameObject, this.muzzleString, true);
                Util.PlaySound("HenryShootPistol", base.gameObject);
                RoR2.Projectile.ProjectileManager.instance.FireProjectile(projectilePrefab, base.GetAimRay().origin, Util.QuaternionSafeLookRotation(base.GetAimRay().direction), base.gameObject, (base.characterBody.HasBuff(Modules.Buffs.superSonicBuff) ? superDamageCoefficient : damageCoefficient) * this.damageStat, force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, 90f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            movementReduction = base.characterBody.HasBuff(Modules.Buffs.superSonicBuff) ? baseSuperMovementReduction : baseMovementReduction;
            targetVelocity = (base.inputBank.moveVector.normalized*base.characterBody.moveSpeed)*movementReduction;
            targetVelocity.y = -0.5f;
            base.characterMotor.velocity = easedIn ? targetVelocity : Vector3.Lerp(base.characterMotor.velocity, targetVelocity, base.fixedAge/this.duration);
            if (base.fixedAge >= this.fireTime*(firedCounter+0.5f) && firedCounter<Modules.StaticValues.sonicBoomCount)
            {
                this.Fire();
            }
            if(base.fixedAge>=this.fireTime*Modules.StaticValues.sonicBoomCount && base.isAuthority&&base.skillLocator.secondary.stock>0&&base.inputBank.skill2.down)
            {
                this.outer.SetNextState(new SonicBoom
                {
                    easedIn=true
                });
                base.skillLocator.secondary.DeductStock(1);
                return;
            }
            if (base.isAuthority && base.inputBank.skill3.justPressed && base.skillLocator.utility.IsReady())
            {
                this.outer.SetNextState(new Boost());
                return;
            }
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}