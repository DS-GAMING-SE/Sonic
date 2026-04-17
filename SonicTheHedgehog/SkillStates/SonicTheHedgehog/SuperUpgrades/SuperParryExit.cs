using EntityStates;
using RoR2;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.SuperUpgrades
{
    public class SuperParryExit : ParryExit
    {
        public static SkillDefs.RequiresFormTargetSkillDef idwAttackSkillDef;
        public static float superParryRange = 50;
        protected override void OnSuccessfulParry()
        {
            base.OnSuccessfulParry();
            if (NetworkServer.active)
            {
                SuperParryBlast();
            }
        }

        protected override void FollowUpAttack()
        {
            followUp.ReadyFollowUpAttack(skillLocator.secondary.skillDef, idwAttackSkillDef, 9999f);
        }

        private void SuperParryBlast()
        {
            SphereSearch sphereSearch = new SphereSearch();
            sphereSearch.origin = base.characterBody.transform.position;
            sphereSearch.radius = superParryRange;
            sphereSearch.mask = LayerIndex.entityPrecise.mask;
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(base.teamComponent.teamIndex));
            sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            HurtBox[] hurtBoxes = sphereSearch.GetHurtBoxes();
            foreach (HurtBox hurtBox in hurtBoxes)
            {
                CharacterBody characterBody = hurtBox.healthComponent.body;
                if (characterBody)
                {
                    characterBody.AddTimedBuff(Buffs.superParryDebuff, StaticValues.superParryDebuffDuration);
                }
            }
        }
    }
}