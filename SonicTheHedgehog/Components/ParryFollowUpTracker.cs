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
            characterBody = GetComponent<CharacterBody>();
            skillLocator = characterBody.skillLocator;
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
            setSkillOverride = null;
            stockCountBeforeOverride = 0;
            removeAttackTimer = 0;
        }

        private void OnSkillChanged(GenericSkill skill)
        {
            if (parrySkillDef != null && setSkillOverride != null && skill.skillDef != parrySkillDef && skill.skillDef != setSkillOverride) // On skill changed rarely ever happens because priority of follow up too high
            {
                RemoveFollowUpAttack();
            }
        }
    }
}
