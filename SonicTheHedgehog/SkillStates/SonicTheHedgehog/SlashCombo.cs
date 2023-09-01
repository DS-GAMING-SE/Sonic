using SonicTheHedgehog.SkillStates.BaseStates;
using RoR2;
using UnityEngine;
using Rewired;

namespace SonicTheHedgehog.SkillStates
{
    public class SlashCombo : BaseMeleeAttack
    {

        public float maxHomingAttackRange;
        public float homingAttackSpeed;
        public bool homingAttack;
        public float estimatedHomingAttackTime;
        public float homingAttackOvershoot;
        public Collider target;
        public Vector3 targetDirection;
        public RaycastHit raycastHit;
        private OverlapAttack attack;

        public override void OnEnter()
        {
            if (!this.homingAttack)
            {
                this.hitboxName = "Sword";
                this.maxHomingAttackRange = base.characterBody.moveSpeed * 6;
                this.homingAttackSpeed = base.characterBody.moveSpeed * 6;
                this.homingAttackOvershoot = 0.5f;
                if (Physics.Raycast(base.GetAimRay(), out raycastHit, maxHomingAttackRange, LayerIndex.enemyBody.intVal) && raycastHit.distance > 15)
                {
                    fireHomingAttack();
                }
                else
                {
                    this.homingAttack = false;
                    base.characterMotor.velocity += base.characterDirection.forward*2;
                    base.characterMotor.velocity.y = 0;
                }

                this.damageType = DamageType.Generic;
                this.damageCoefficient = swingIndex == 5 ? Modules.StaticValues.finalMeleeDamageCoefficient : Modules.StaticValues.meleeDamageCoefficient;
                this.procCoefficient = 1f;
                this.pushForce = 50f;
                this.bonusForce = Vector3.zero;
                this.baseDuration = swingIndex == 5 ? 1.1f : 0.5f;
                this.attackStartTime = swingIndex == 5 ? 0.5f : 0.1f;
                this.attackEndTime = swingIndex == 5 ? 0.6f : 0.2f;
                this.baseEarlyExitTime = swingIndex == 5 ? 0.6f : 0.2f; ;
                this.hitStopDuration = swingIndex == 5 ? 0.1f : 0.012f;
                this.attackRecoil = swingIndex == 5 ? 1f : 0.5f;
                this.hitHopVelocity = 4f;

                this.swingSoundString = "HenrySwordSwing";
                this.hitSoundString = "";
                this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
                this.swingEffectPrefab = Modules.Assets.swordSwingEffect;
                this.hitEffectPrefab = Modules.Assets.swordHitImpactEffect;

                //this.impactSound = Modules.Assets.swordHitSoundEvent.index;

                base.OnEnter();
            }
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAttackAnimation();
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if (this.homingAttack)
            {
                this.homingAttack = false;
                base.characterMotor.velocity = Vector3.zero;
                base.SmallHop(base.characterMotor, this.hitHopVelocity*2);
                this.outer.SetNextStateToMain();
                return;
            }
        }

        protected override void SetNextState()
        {
            if (Physics.Raycast(base.GetAimRay(), out raycastHit, maxHomingAttackRange, LayerIndex.enemyBody.intVal) && raycastHit.distance > 15)
            {
                fireHomingAttack();
            }
            else
            {
                homingAttack = false;
                int index = this.swingIndex;
                if (index < 5) index += 1;
                else index = 0;

                this.outer.SetNextState(new SlashCombo
                {
                    swingIndex = index
                });
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.homingAttack)
            {
                attack.Fire();
                if (fixedAge<this.estimatedHomingAttackTime)
                {
                    if (this.target != null)
                    {
                        targetDirection = this.target.ClosestPoint(base.transform.position) - base.transform.position;
                    }
                    base.characterMotor.velocity = targetDirection.normalized * Mathf.Min(homingAttackSpeed, targetDirection.magnitude*60);
                }
                else
                {
                    base.characterMotor.velocity = Vector3.zero;
                    this.homingAttack = false;
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public void fireHomingAttack()
        {
            this.target = raycastHit.collider;
            this.homingAttack = true;
            this.targetDirection = new Vector3(0, 0, 0);
            if (target != null)
            {
                this.targetDirection = (target.ClosestPoint(base.transform.position) - base.transform.position);
            }
            this.estimatedHomingAttackTime = (targetDirection.magnitude / homingAttackSpeed) + homingAttackOvershoot;
            this.hitboxName = "Ball";
            base.PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", this.estimatedHomingAttackTime);
            this.attack = new OverlapAttack();
        }
    }
}