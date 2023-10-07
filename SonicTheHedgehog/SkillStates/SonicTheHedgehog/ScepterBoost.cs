using EntityStates;
using R2API;
using RoR2;
using RoR2.Networking;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements.Experimental;
using static RoR2.OverlapAttack;

namespace SonicTheHedgehog.SkillStates
{
    public class ScepterBoost : Boost
    {
        // Move healthComponent tracking to boostLogic so ICD can be longer
        
        private static float checksPerSecond = 5;

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
            this.sphereSearch.radius = 3;
            this.sphereSearch.mask = LayerIndex.entityPrecise.mask;
            this.sphereSearch.RefreshCandidates();
            this.sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
            HurtBox[] hitList = this.sphereSearch.GetHurtBoxes();

            if (hitList.Count()==0)
            {
                return;
            }

            CalculateDamage();

            for (int i = 0; i < hitList.Count(); i++)
            {
                Debug.Log("Scepter Boost hurtbox check");
                HealthComponent healthComponent = hitList[i].healthComponent;
                if (healthComponent && !boostLogic.recentlyHitHealthComponents.Contains(healthComponent))
                {
                    if (NetworkServer.active)
                    {
                        DealDamage(hitList[i]);
                    }
                    boostLogic.AddHealthComponent(healthComponent);
                }
            }
        }
        
        private void CalculateDamage()
        {
            this.damage = ((StaticValues.scepterBoostDamageCoefficient * base.characterBody.moveSpeed) / StaticValues.defaultPowerBoostSpeed) * base.characterBody.damage;
            this.damageInfo = new DamageInfo
            {
                attacker = base.characterBody.gameObject,
                inflictor = base.characterBody.gameObject,
                crit = base.RollCrit(),
                damage = this.damage,
                position = base.characterBody.transform.position,
                force = Vector3.zero,
                damageType = DamageType.Generic,
                damageColorIndex = DamageColorIndex.Default,
                procCoefficient = StaticValues.scepterBoostProcCoefficient
            };
        }

        private void DealDamage(HurtBox hurtBox)
        {
            hurtBox.healthComponent.TakeDamage(this.damageInfo);
            GlobalEventManager.instance.OnHitEnemy(this.damageInfo, hurtBox.healthComponent.gameObject);
        }
    }
}