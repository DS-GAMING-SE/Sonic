using EnemiesReturns.Components;
using EnemiesReturns.Configuration;
using EntityStates;
using HedgehogUtils.Voicelines;
using R2API;
using RoR2;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
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
        protected float circumference;
        protected List<Vector3> linePointOffset;
        protected Vector3[] linePoints;
        private const float distancePerLinePoint = 0.3f;

        public CyloopVFX cyloopVFX;
        public virtual Color cyloopTrailColor { get { return SonicTheHedgehogCharacter.sonicColor2; } }
        public virtual Color cyloopTrailIntersectColor { get { return new Color(1f, 0.3f, 0.7f); } }
        public virtual float cyloopTrailSizeMultiplier { get { return 1f; } }
        public virtual Material temporaryOverlayMaterial { get { return Modules.Assets.cyloopOverlay; } }
        public virtual GameObject hitEffectPrefab { get { return Modules.Assets.cyloopHitEffect; } }
        public virtual GameObject doubleHitEffectPrefab { get { return Modules.Assets.cyloopDoubleHitEffect; } }
        public virtual float cyloopDoublePushForce { get { return 1000f; } }

        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.special.DeductStock(1);
            characterBody.OnSkillActivated(skillLocator.special);
            Util.PlaySound("Play_sonicthehedgehog_cyloop", gameObject);
            VoicelineComponent.TryPlayVoiceline(gameObject, "Play_sonicthehedgehog_voiceline_grunt_buildup_short", VoicelinePriority.PrioritySkill);
            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            if (target && target.healthComponent)
            {
                cyloopVFX = CyloopVFX.SpawnVFX(cyloopTrailSizeMultiplier, cyloopTrailColor, characterBody.coreTransform, characterBody.radius * cyloopTrailSizeMultiplier, temporaryOverlayMaterial, GetModelTransform());

                attackTime = (baseAttackTime * (1 + (target.healthComponent.body.radius * 0.2f))) / attackSpeedStat;
                circumference = 2 * Mathf.PI * (target.healthComponent.body.radius + StaticValues.quickCyloopExtraRadius);
                speed = circumference / attackTime;
                yOffset = transform.position.y - target.transform.position.y;
                startForward = target.transform.position - transform.position;
                startForward.y = 0;
                startForward = Vector3.Normalize(startForward);
                startRight = -Vector3.Cross(startForward, Vector3.up);
                linePoints = new Vector3[Mathf.FloorToInt(circumference / distancePerLinePoint) + 1];
                linePointOffset = new List<Vector3>();
                Vector3 startPoint = -startForward * (target.healthComponent.body.radius + StaticValues.quickCyloopExtraRadius);
                startPoint.y = yOffset;
                linePointOffset.Add(startPoint);
                cyloopVFX.lineRenderer.positionCount = 2;
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
        // save a "lastTargetPosition" and save orbit radius at the beginning so the move can continue if there is no target
        public override void Update()
        {
            base.Update();
            if (modelLocator.modelTransform && target)
            {
                /*Vector3 endPosition = target.transform.position - (startForward * (target.healthComponent.body.radius + StaticValues.quickCyloopExtraRadius));
                endPosition.y = yOffset;
                characterMotor.AddDisplacement((endPosition - transform.position));*/
                
                float lerp = age / attackTime;
                float lerpCircle = lerp * 2 * Mathf.PI;
                Vector3 orbitVector = -startRight * Mathf.Sin(lerpCircle + Mathf.PI);
                orbitVector += -startForward * Mathf.Cos(lerpCircle);
                orbitVector *= target.healthComponent.body.radius + StaticValues.quickCyloopExtraRadius;
                orbitVector.y = yOffset;
                modelLocator.modelTransform.SetPositionAndRotation(target.transform.position + orbitVector, Quaternion.LookRotation(Vector3.Cross(orbitVector.normalized, Vector3.up)));
                
                if (lerpCircle > (circumference / linePoints.Length) * linePointOffset.Count)
                {
                    linePointOffset.Add(orbitVector);
                    cyloopVFX.lineRenderer.positionCount = linePointOffset.Count + 1;
                }
                for (int i = 0; i < linePointOffset.Count; i++)
                {
                    linePoints[i] = target.transform.position + linePointOffset[i];
                }
                linePoints[linePointOffset.Count + 1] = target.transform.position + orbitVector;
                cyloopVFX.lineRenderer.SetPositions(linePoints);
            }
        }
        public override void OnExit()
        {
            GetModelAnimator().SetBool("isBoosting", false);
            Util.PlaySound("Stop_sonicthehedgehog_cyloop", gameObject);
            modelLocator.autoUpdateModelTransform = true;
            if (isAuthority) 
            { 
                characterMotor.velocity = startRight * Mathf.Min(speed * 0.5f, characterBody.moveSpeed);
                characterDirection.forward = startRight;
                characterDirection.moveVector = startRight;
            }
            if (base.characterBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.IgnoreFallDamage))
            {
                base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }

            cyloopVFX.SetLineColor(cyloopTrailIntersectColor);
            cyloopVFX.EndAllVFX();

            if (target)
            {
                Util.PlaySound("Play_sonicthehedgehog_cyloop_complete", gameObject);
                EffectManager.SpawnEffect(target.healthComponent && Buffs.HasCyloopDebuff(target.healthComponent.body) ?
                    doubleHitEffectPrefab : hitEffectPrefab, new EffectData
                {
                    origin = target.transform.position,
                    scale = Mathf.Min(target.healthComponent.body.radius, 3f),
                    rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up)
                }, false);
            }
            if (NetworkServer.active) characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
            if (NetworkServer.active && target && target.healthComponent)
            {
                DamageInfo damageInfo = new DamageInfo();
                PrepareAttack(ref damageInfo);
                if (Buffs.HasCyloopDebuff(target.healthComponent.body)) PrepareDoubleAttack(ref damageInfo);
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
        public virtual void PrepareDoubleAttack(ref DamageInfo damageInfo)
        {
            damageInfo.damage = StaticValues.cyloopDoubleDamageCoefficient * characterBody.damage;
            damageInfo.procCoefficient = 1.5f;
            damageInfo.damageType.RemoveModdedDamageType(DamageTypes.cyloop);
            damageInfo.damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launch);
            damageInfo.force = Vector3.down * cyloopDoublePushForce;
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
