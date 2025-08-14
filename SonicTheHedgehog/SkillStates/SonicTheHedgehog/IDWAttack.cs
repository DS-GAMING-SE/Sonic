using EntityStates;
using Rewired;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using HedgehogUtils.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using HedgehogUtils.Forms.SuperForm;

namespace SonicTheHedgehog.SkillStates
{
    public class IDWAttack : BaseSkillState
    {  
        protected static DamageType damageType = DamageType.Stun1s | DamageType.AOE;
        protected static float damageCoefficient = Modules.StaticValues.idwAttackDamageCoefficient;
        protected static float procCoefficient = StaticValues.idwAttackProcCoefficient;
        protected static float baseAttackDuration = 1.6f;
        protected static float increasePerAdditionalStock = 0.20f;
        protected static float range = 25;
        protected static int baseAttackCount = 8;
        protected static float pushForce = 400f;
        protected static float baseEndLag = 1.6f;
        protected float endLag;

        public HurtBox target = null;
        protected Vector3 targetPosition = Vector3.zero;

        public float attackDuration;
        public float duration;
        private bool hasFired;
        protected bool inHitPause;
        protected float stopwatch;
        protected Animator animator;
        private int maxAttackCount;
        private int attackCount;
        private bool invisible;

        private Transform modelTransform;
        private CharacterModel characterModel;

        private GameObject idwAttackEffect;

        protected BlastAttack blastAttack;
        private BlastAttack.HitPoint[] hit;

        public override void OnEnter()
        {
            base.OnEnter();
            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
            }
            this.hasFired = false;
            this.endLag = baseEndLag;
            if (base.isAuthority)
            {
                base.characterMotor.Motor.ForceUnground();
            }
            this.attackDuration = baseAttackDuration * (1 + (increasePerAdditionalStock * (base.skillLocator.secondary.maxStock-1)));
            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);
            base.characterMotor.disableAirControlUntilCollision = false;
            Util.PlaySound("Play_sonicthehedgehog_idw", base.gameObject);
            idwAttackEffect = UnityEngine.GameObject.Instantiate<GameObject>(Modules.Assets.idwAttackEffect);

            EffectManager.SimpleMuzzleFlash(Modules.Assets.superSonicBlurEffect, base.gameObject, "BlurForward", true);
            hasFired = true;
            if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }
            invisible = true;
            this.maxAttackCount = (int)Math.Ceiling((base.characterBody.attackSpeed * baseAttackCount) * (1 + (increasePerAdditionalStock * (base.skillLocator.secondary.maxStock - 1))));
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.Intangible);
            }

            base.skillLocator.secondary.onSkillChanged += OnSkillChanged;
        }

        public override void OnExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (invisible)
            {
                if (this.characterModel)
                {
                    this.characterModel.invisibilityCount--;
                }
                if (NetworkServer.active)
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
                }
            }
            if (idwAttackEffect)
            {
                UnityEngine.GameObject.Destroy(idwAttackEffect);
            }
            base.OnExit();

            this.animator.SetBool("attacking", false);

            base.skillLocator.secondary.onSkillChanged -= OnSkillChanged;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.stopwatch += Time.fixedDeltaTime;

            if (this.fixedAge <= attackDuration) // attack
            {
                if (this.fixedAge > (attackDuration / this.maxAttackCount * (this.attackCount + 1)))
                {
                    if (this.target != null)
                    {
                        this.targetPosition = this.target.transform.position;
                    }
                    /*else if (this.hit.Length > 0) Why does this not workkkkkkk aauhjahujfdhdjuhg. It's supposed to retarget if the thing you're targetting disappears but uhhh I guess not
                    {
                        Debug.Log("IDW Searching for target");
                        this.target = this.hit[0].hurtBox;
                        float distance = Vector3.Distance(this.hit[0].hurtBox.transform.position, this.targetPosition);
                        Debug.Log("Almost foreach");
                        foreach (BlastAttack.HitPoint hitPoint in this.hit)
                        {
                            Debug.Log("foreach");
                            if (Vector3.Distance(hitPoint.hurtBox.transform.position, this.targetPosition) <= distance)
                            {
                                target = hitPoint.hurtBox;
                                distance = Vector3.Distance(hitPoint.hurtBox.transform.position, this.targetPosition);
                            }
                        }
                        this.targetPosition = this.target.transform.position;
                    }
                    */
                    idwAttackEffect.transform.position = this.targetPosition;
                    Util.PlaySound("Play_sonicthehedgehog_idw_hit", base.gameObject);
                    if (base.isAuthority)
                    {
                        PrepareBlastAttack();
                        FireBlastAttack();
                    }
                    attackCount++;
                    return;
                }
            }

            if (this.fixedAge > attackDuration) //end lag start
            {
                if (invisible)
                {
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.superSonicBlurEffect, base.gameObject, "BlurSide", true);
                    idwAttackEffect.GetComponentInChildren<ParticleSystem>().Stop();
                    if (this.characterModel)
                    {
                        this.characterModel.invisibilityCount--;
                    }
                    invisible = false;
                    if (NetworkServer.active)
                    {
                        base.characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
                    }
                    Util.PlaySound("Play_sonicthehedgehog_idw_end", base.gameObject);
                    this.endLag = baseEndLag / base.characterBody.attackSpeed;
                    base.PlayAnimation("FullBody, Override", "IDWEnd", "Slash.playbackRate", this.endLag);
                    base.characterDirection.forward = base.characterDirection.forward * -1;
                    base.characterDirection.moveVector = base.characterDirection.moveVector * -1;
                    return;
                }
            }

            if (base.isAuthority && (this.fixedAge > attackDuration + this.endLag)) //end
            {
                base.characterMotor.velocity = Vector3.zero;
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public virtual void PrepareBlastAttack()
        {
            blastAttack = new BlastAttack();
            blastAttack.radius = range;
            blastAttack.procCoefficient = procCoefficient;
            blastAttack.position = targetPosition;
            blastAttack.attacker = base.gameObject;
            blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
            blastAttack.baseDamage = base.characterBody.damage * damageCoefficient;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.damageType = damageType;
            blastAttack.damageType.damageSource = DamageSource.Secondary;
            blastAttack.baseForce = -pushForce;
            blastAttack.teamIndex = base.teamComponent.teamIndex;
            blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
        }

        protected virtual void FireBlastAttack()
        {
            this.hit = blastAttack.Fire().hitPoints;
        }
        public virtual void OnSkillChanged(GenericSkill skill)
        {
            if (!typeof(IDWAttackSearch).IsAssignableFrom(skill.activationState.stateType) && skill.skillDef != SuperSkillReplacer.afterIDWAttack)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(HurtBoxReference.FromHurtBox(target));
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            target = reader.ReadHurtBoxReference().ResolveHurtBox();
        }
    }
}