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

        public static float damageCoefficient = Modules.StaticValues.sonicBoomDamageCoefficient;
        public static float procCoefficient = 0.75f;
        public static float baseDuration = Modules.StaticValues.sonicBoomFireTime* (Modules.StaticValues.sonicBoomCount*1.23f);
        public static float baseFireTime = Modules.StaticValues.sonicBoomFireTime;
        public static float baseProjectileSpeed = 90f;
        public static float force = 80f;
        public static float recoil = 0f;
        public static float range = 100f;
        public virtual float baseMovementReduction
        {
            get { return 0.45f; }
        }
        protected static float offset = 0.4f;

        protected float duration;
        protected float fireTime;
        protected int firedCounter;
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
            base.PlayAnimation("FullBody, Override", "SonicBoom", "Slash.playbackRate", this.fireTime * Modules.StaticValues.sonicBoomCount);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        protected virtual void Fire()
        {
            this.firedCounter++;
            Util.PlaySound("Play_sonicthehedgehog_sonic_boom_fire", base.gameObject);
            if (base.isAuthority)
            {
                StartAimMode();
                base.characterBody.AddSpreadBloom(1.5f);
                if ((firedCounter==1 || this.fireTime>=0.06f))
                {
                    FireProjectileMuzzleFlash();
                }


                FireProjectile();
            }
        }

        protected virtual void FireProjectileMuzzleFlash()
        {
            EffectManager.SimpleMuzzleFlash(Modules.Assets.sonicBoomKickEffect, base.gameObject, this.muzzleString, true);
        }

        protected virtual void FireProjectile()
        {
            RoR2.Projectile.ProjectileManager.instance.FireProjectile(ProjectilePrefab(), base.GetAimRay().origin, ProjectileRotation(), base.gameObject, 
                damageCoefficient * this.damageStat, force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, baseProjectileSpeed);
        }

        protected virtual GameObject ProjectilePrefab()
        {
            return Projectiles.sonicBoomPrefab;
        }

        protected virtual Quaternion ProjectileRotation()
        {
            Quaternion direction = Util.QuaternionSafeLookRotation(base.GetAimRay().direction);
            Vector3 up = direction * Vector3.up;

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

            return Util.QuaternionSafeLookRotation(base.GetAimRay().direction, up);
        }

        protected virtual void SetNextState()
        {
            this.outer.SetNextState(new SonicBoom
            {
                easedIn = true
            });
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            movementReduction = baseMovementReduction;
            targetVelocity = (base.inputBank.moveVector.normalized*base.characterBody.moveSpeed)*movementReduction;
            targetVelocity.y = -0.5f;
            base.characterMotor.velocity = easedIn ? targetVelocity : Vector3.Lerp(base.characterMotor.velocity, targetVelocity, base.fixedAge/this.duration);
            if (base.fixedAge >= this.fireTime*(firedCounter+0.5f) && firedCounter < Modules.StaticValues.sonicBoomCount)
            {
                this.Fire();
            }
            if (base.fixedAge>=this.fireTime*Modules.StaticValues.sonicBoomCount && base.isAuthority&&base.skillLocator.secondary.stock>0&&base.inputBank.skill2.down)
            {
                SetNextState();
                base.skillLocator.secondary.DeductStock(1);
                base.characterBody.OnSkillActivated(skillLocator.secondary);
                return;
            }
            if (base.isAuthority && base.inputBank.skill3.justPressed && base.skillLocator.utility.IsReady())
            {
                base.skillLocator.utility.OnExecute();
                return;
            }
            if (base.fixedAge >= (this.fireTime * (Modules.StaticValues.sonicBoomCount)) && !exitAnimPlayed)
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
                base.PlayAnimation("Body", "SonicBoomEnd");
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