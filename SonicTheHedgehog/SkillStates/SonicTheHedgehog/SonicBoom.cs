using EntityStates;
using RoR2;
using SonicTheHedgehog.Modules;
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
        public static float baseSuperMovementReduction=0.85f;
        public static GameObject projectilePrefab;
        private static float offset = 0.4f;

        private float duration;
        private float fireTime;
        private int firedCounter;
        private string muzzleString="SwingCenter";
        private float movementReduction;
        private Vector3 targetVelocity;
        private bool exitAnimPlayed = false;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterMotor.disableAirControlUntilCollision = false;
            this.firedCounter = 0;
            this.duration = SonicBoom.baseDuration / this.attackSpeedStat;
            this.fireTime = baseFireTime / this.attackSpeedStat;
            base.characterBody.SetAimTimer(1.5f);
            base.PlayAnimation("FullBody, Override", "SonicBoom", "Slash.playbackRate", this.fireTime*Modules.StaticValues.sonicBoomCount);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void Fire()
        {
            this.firedCounter++;
            Util.PlaySound("Play_sonic_boom_fire", base.gameObject);
            if (base.isAuthority)
            {
                StartAimMode();
                base.characterBody.AddSpreadBloom(1.5f);
                if ((firedCounter==1 || this.fireTime>=0.06f) && !base.characterBody.HasBuff(Buffs.superSonicBuff))
                {
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.sonicBoomKickEffect, base.gameObject, this.muzzleString, true);
                }

                projectilePrefab = base.characterBody.HasBuff(Modules.Buffs.superSonicBuff) ? Modules.Projectiles.superSonicBoomPrefab : Modules.Projectiles.sonicBoomPrefab;
                Quaternion direction = Util.QuaternionSafeLookRotation(base.GetAimRay().direction);
                Vector3 up = direction * Vector3.up;
                if (!base.characterBody.HasBuff(Buffs.superSonicBuff))
                {
                    if (firedCounter == 1)
                    {
                        Vector3 right = direction * Vector3.right;
                        up = Vector3.RotateTowards(up, right, offset, 1);
                    }
                    else
                    {
                        Vector3 left = direction * Vector3.left;
                        up = Vector3.RotateTowards(up, left, offset, 1);
                    }
                }
                direction= Util.QuaternionSafeLookRotation(base.GetAimRay().direction,up);


                RoR2.Projectile.ProjectileManager.instance.FireProjectile(projectilePrefab, base.GetAimRay().origin, direction, base.gameObject, (base.characterBody.HasBuff(Modules.Buffs.superSonicBuff) ? superDamageCoefficient : damageCoefficient) * this.damageStat, force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, 90f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            movementReduction = base.characterBody.HasBuff(Modules.Buffs.superSonicBuff) ? baseSuperMovementReduction : baseMovementReduction;
            targetVelocity = (base.inputBank.moveVector.normalized*base.characterBody.moveSpeed)*movementReduction;
            targetVelocity.y = -0.5f;
            base.characterMotor.velocity = easedIn ? targetVelocity : Vector3.Lerp(base.characterMotor.velocity, targetVelocity, base.fixedAge/this.duration);
            if (base.fixedAge >= this.fireTime*(firedCounter+0.5f) && firedCounter < Modules.StaticValues.sonicBoomCount)
            {
                this.Fire();
            }
            if (base.fixedAge>=this.fireTime*Modules.StaticValues.sonicBoomCount && base.isAuthority&&base.skillLocator.secondary.stock>0&&base.inputBank.skill2.down)
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
                base.skillLocator.utility.OnExecute();
                return;
            }
            if (base.fixedAge >= Modules.StaticValues.sonicBoomFireTime * (Modules.StaticValues.sonicBoomCount) && !exitAnimPlayed)
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
                base.PlayAnimation("Body", "SonicBoomEnd", "Slash.playbackRate", this.fireTime * Modules.StaticValues.sonicBoomCount);
                exitAnimPlayed = true;
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