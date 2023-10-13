using R2API.Networking.Interfaces;
using R2API.Networking;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class ScepterBoost : Boost
    {
        private static float checksPerSecond = 8;

        private static readonly float damageRadius = 4f;
        private static readonly float superDamageRadius = 6;

        private static readonly float pushOutForceMagnitude = 500;
        private static readonly float pushUpForceMagnitude = 100;

        private float damageTimer = 0f;
        
        private SphereSearch sphereSearch = new SphereSearch();
        
        private float damage;
        private DamageInfo damageInfo;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.boosting)
            {
                this.damageTimer += Time.fixedDeltaTime;
                if (this.damageTimer > 1f / checksPerSecond)
                {
                    ScepterDamage();
                }
            }
            else
            {
                this.damageTimer = 0f;
            }
        }

        public override void ScepterReset()
        {
            ScepterDamage();
        }
        
        public override void ScepterDamage()
        {
            this.damageTimer = 0f;

            this.sphereSearch.origin = characterBody.transform.position;
            this.sphereSearch.radius = base.characterBody.HasBuff(Buffs.superSonicBuff) ? superDamageRadius : damageRadius;
            this.sphereSearch.mask = LayerIndex.entityPrecise.mask;
            this.sphereSearch.RefreshCandidates();
            this.sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
            HurtBox[] hitList = this.sphereSearch.GetHurtBoxes();

            if (hitList.Count() == 0)
            {
                return;
            }

            for (int i = 0; i < hitList.Count(); i++)
            {
                //Debug.Log("Scepter Boost hurtbox check");
                HealthComponent healthComponent = hitList[i].healthComponent;
                if (healthComponent && !boostLogic.recentlyHitHealthComponents.Contains(healthComponent))
                {
                    CalculateDamage(hitList[i]);
                    if (NetworkServer.active)
                    {
                        DealDamage(hitList[i], this.damageInfo);
                    }
                    else
                    {
                        new ScepterBoostDamage(hitList[i], this.damageInfo).Send(NetworkDestination.Server);
                    }
                    boostLogic.AddTracker(healthComponent);
                }
            }
        }
        
        private void CalculateDamage(HurtBox hurtBox)
        {
            this.damage = (StaticValues.scepterBoostDamageCoefficient * base.characterBody.moveSpeed) / StaticValues.defaultPowerBoostSpeed;
            Vector3 pushOutForce = Vector3.Normalize(base.characterMotor.velocity) * pushOutForceMagnitude * (this.damage/2f);
            Vector3 pushUpForce = Vector3.up * pushUpForceMagnitude * (this.damage / 2f);
            //Chat.AddMessage(pushOutForce + " " + pushUpForce);
            this.damageInfo = new DamageInfo
            {
                attacker = base.characterBody.gameObject,
                inflictor = base.characterBody.gameObject,
                crit = base.RollCrit(),
                damage = this.damage * base.characterBody.damage,
                position = hurtBox.transform.position,
                force = pushOutForce + pushUpForce,
                damageType = DamageType.Generic,
                damageColorIndex = DamageColorIndex.Default,
                procCoefficient = StaticValues.scepterBoostProcCoefficient
            };
        }

        public static void DealDamage(HurtBox hurtBox, DamageInfo damageInfo)
        {
            hurtBox.healthComponent.TakeDamage(damageInfo);
            GlobalEventManager.instance.OnHitEnemy(damageInfo, hurtBox.healthComponent.gameObject);
            GlobalEventManager.instance.OnHitAll(damageInfo, hurtBox.healthComponent.gameObject);
        }

        public override string GetSoundString()
        {
            return "Play_scepter_boost";
        }
    }
}