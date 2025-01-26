using RoR2.Skills;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SonicTheHedgehog.SkillStates;

namespace SonicTheHedgehog.Components
{
    public class ParryFollowUpTracker : MonoBehaviour
    {
        protected SkillLocator skillLocator;
        protected CharacterBody characterBody;

        public static SkillDef followUpSkillDef;

        public SkillDef parrySkillDef;
        public SkillDef setSkillOverride;
        public int stockCountBeforeOverride;

        public float removeAttackTimer;
        
        public void Start()
        {
            skillLocator = GetComponent<SkillLocator>();
            characterBody = GetComponent<CharacterBody>();
            skillLocator.secondary.onSkillChanged += OnSkillChanged;
        }

        public void FixedUpdate()
        {
            if (removeAttackTimer > 0)
            {
                removeAttackTimer -= Time.fixedDeltaTime;
                if (removeAttackTimer <= 0)
                {
                    RemoveFollowUpAttack();
                }
            }
        }
        
        public void ReadyFollowUpAttack(SkillDef parrySkillDef, SkillDef followUpSkillDef, float duration)
        {
            if (followUpSkillDef)
            {
                this.parrySkillDef = parrySkillDef;
                setSkillOverride = followUpSkillDef;
                stockCountBeforeOverride = skillLocator.secondary.stock;
                skillLocator.secondary.SetSkillOverride(this, setSkillOverride, GenericSkill.SkillOverridePriority.Contextual);
                removeAttackTimer = duration;
            }
        }

        public void RemoveFollowUpAttack()
        {
            skillLocator.secondary.UnsetSkillOverride(this, setSkillOverride, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.secondary.stock = stockCountBeforeOverride;
            stockCountBeforeOverride = 0;
            removeAttackTimer = 0;
        }

        private void OnSkillChanged(GenericSkill skill)
        {
            if (skill.skillDef != parrySkillDef && skill.skillDef != setSkillOverride)
            {
                RemoveFollowUpAttack();
            }
        }
    }
}
