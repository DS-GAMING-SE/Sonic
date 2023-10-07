using EntityStates;
using R2API;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements.Experimental;

namespace SonicTheHedgehog.SkillStates
{
    public class ScepterBoost : Boost
    {
        private SphereSearch sphereSearch;
        
        private float damage;
        private DamageInfo damageInfo;

        private List<HealthComponent> recentlyHitHealthComponents = new List<HealthComponent>();
        
        private void Damage()
        {
            this.sphereSearch.origin = characterBody.transform.position;
            this.sphereSearch.radius = 3;
            this.sphereSearch.mask = LayerIndex.entityPrecise.mask;
            this.sphereSearch.RefreshCandidates();
            this.sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(teamComponent.teamIndex));
            HurtBox[] hitList = this.sphereSearch.GetHurtBoxes();
            if (hitList.Count()<=0)
            {
                return;
            }
            for (int i = 0; i < hitList.Count(); i++)
            {

            }
        }
        
        private void CalculateDamage()
        {
            this.damage = (StaticValues.scepterBoostDamageCoefficient * base.characterBody.moveSpeed) / StaticValues.defaultPowerBoostSpeed;
            this.damageInfo = new DamageInfo
            {
                attacker = base.characterBody.gameObject,
                inflictor = base.characterBody.gameObject,
                crit = base.RollCrit(),
                damage = damage * base.characterBody.damage,
                position = base.characterBody.transform.position,
                force = Vector3.zero,
                damageType = DamageType.Generic,
                damageColorIndex = DamageColorIndex.Default,
                procCoefficient = 1f
            };
        }
    }
}