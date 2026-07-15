using EnemiesReturns.Configuration;
using EntityStates;
using R2API;
using RoR2;
using SonicTheHedgehog.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.OverlapAttack;

namespace SonicTheHedgehog.SkillStates.Cyloop
{
    public class QuickCyloop : BaseSkillState
    {
        public HurtBox target;
        public const float baseAttackTime = 0.4f;
        protected float attackTime;
        protected float speed;
        protected float yOffset;
        protected Vector3 startRight;
        protected Vector3 startForward;

        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.special.DeductStock(1);
            characterBody.OnSkillActivated(skillLocator.special);
            Util.PlaySound("Play_sonicthehedgehog_cyloop", gameObject);
            if (target && target.healthComponent)
            {
                attackTime = (baseAttackTime * (1 + (target.healthComponent.body.radius * 0.2f))) / attackSpeedStat;
                speed = (2 * Mathf.PI * target.healthComponent.body.radius + StaticValues.quickCyloopExtraRadius) / attackTime;
                yOffset = transform.position.y - target.transform.position.y;
                startForward = Vector3.Normalize(target.transform.position - transform.position);
                startRight = -Vector3.Cross(startForward, Vector3.up);
            }
            modelLocator.autoUpdateModelTransform = false;
            if (NetworkServer.active) characterBody.AddBuff(RoR2Content.Buffs.Intangible);
            PlayCrossfade("Body", "AirBoost", "Roll.playbackRate", attackTime / 2, attackTime / 3);
            GetModelAnimator().SetBool("isBoosting", true);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                characterMotor.velocity = Vector3.zero;
                if (!target || fixedAge > attackTime)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }
        public override void Update()
        {
            base.Update();
            if (modelLocator.modelTransform && target)
            {
                float lerp = (age / attackTime) * 2 * Mathf.PI;
                Vector3 orbitVector = -startRight * Mathf.Sin(lerp + Mathf.PI);
                orbitVector += -startForward * Mathf.Cos(lerp);
                orbitVector *= target.healthComponent.body.radius + StaticValues.quickCyloopExtraRadius;
                orbitVector.y = yOffset;
                modelLocator.modelTransform.SetPositionAndRotation(target.transform.position + orbitVector, Quaternion.LookRotation(Vector3.Cross(orbitVector.normalized, Vector3.up)));
            }
        }
        public override void OnExit()
        {
            GetModelAnimator().SetBool("isBoosting", false);
            Util.PlaySound("Stop_sonicthehedgehog_cyloop", gameObject);
            modelLocator.autoUpdateModelTransform = true;
            if (isAuthority) 
            { 
                characterMotor.velocity = startRight * speed * 0.5f;
                characterDirection.forward = startRight;
                characterDirection.moveVector = startRight;
            }
            if (target)
            {
                Util.PlaySound("Stop_sonicthehedgehog_complete", gameObject);
                EffectManager.SpawnEffect(Modules.Assets.cyloopHitEffect, new EffectData
                {
                    origin = target.transform.position,
                    scale = target.healthComponent.body.radius,
                    rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up)
                }, false);
            }
            if (NetworkServer.active) characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
            if (NetworkServer.active && target && target.healthComponent)
            {
                DamageInfo damageInfo = new DamageInfo();
                PrepareAttack(ref damageInfo);
                target.healthComponent.TakeDamage(damageInfo);
                GlobalEventManager.instance.OnHitEnemy(damageInfo, target.healthComponent.gameObject);
                GlobalEventManager.instance.OnHitAll(damageInfo, target.healthComponent.gameObject);
            }
            base.OnExit();
        }
        public virtual void PrepareAttack(ref DamageInfo damageInfo)
        {
            damageInfo.attacker = gameObject;
            damageInfo.inflictor = gameObject;
            damageInfo.position = target.transform.position;
            damageInfo.procCoefficient = 1f;
            damageInfo.inflictedHurtbox = target;
            damageInfo.damage = StaticValues.cyloopDamageCoefficient * characterBody.damage;
            damageInfo.damageType = DamageSource.Special;
            damageInfo.damageType.AddModdedDamageType(DamageTypes.cyloop);
            damageInfo.force = Vector3.up * 33f;
            damageInfo.physForceFlags |= PhysForceFlags.massIsOne | PhysForceFlags.resetVelocity | PhysForceFlags.respectKnockbackImmuneFlag;
            damageInfo.crit = RollCrit();
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
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
