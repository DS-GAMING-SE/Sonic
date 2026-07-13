using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using EntityStates;
using UnityEngine;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.SkillStates;
using HedgehogUtils.Forms;
using static HedgehogUtils.Forms.SkillDefs;
using HedgehogUtils.Boost.EntityStates;

namespace SonicTheHedgehog.Modules
{
    public class SkillDefs
    {
        public class MeleeSkillDef : SkillDef
        {
            public SerializableEntityStateType homingAttackState { get; set; }

            public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
            {
                return new MeleeSkillDef.InstanceData
                {
                    homingTracker = skillSlot.GetComponent<HomingTracker>()
                };
            }

            public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
            {
                return DecideNextState(skillSlot, ((MeleeSkillDef.InstanceData)skillSlot.skillInstanceData).homingTracker, 0);
            }
            public static EntityState DecideNextState(GenericSkill skillSlot, HomingTracker homingTracker, int swingIndex)
            {
                if (homingTracker && homingTracker.CanHomingAttack())
                {
                    EntityState entityState = EntityStateCatalog.InstantiateState(((MeleeSkillDef)skillSlot.skillDef).homingAttackState.stateType);
                    ISkillState skillState = entityState as ISkillState;
                    if (skillState != null)
                    {
                        skillState.activatorSkillSlot = skillSlot;
                    }
                    if (typeof(HomingAttack).IsAssignableFrom(((MeleeSkillDef)skillSlot.skillDef).homingAttackState.stateType))
                    {
                        ((HomingAttack)entityState).target = homingTracker.GetTrackingTarget();
                    }
                    return entityState;
                }
                else
                {
                    EntityState entityState = EntityStateCatalog.InstantiateState(skillSlot.activationState.stateType);
                    if (entityState is ISkillState skillState)
                    {
                        skillState.activatorSkillSlot = skillSlot;
                    }
                    if (typeof(SonicMelee).IsAssignableFrom(skillSlot.activationState.stateType))
                    {
                        ((SonicMelee)entityState).swingIndex = swingIndex;
                    }
                    return entityState;
                }
            }

            protected class InstanceData : BaseSkillInstanceData
            {
                public HomingTracker homingTracker;
            }
        }

        public class RequiresFormMeleeSkillDef : MeleeSkillDef, IRequiresFormSkillDef
        {
            public FormDef requiredForm { get; set; }
        }
        public class RequiresTargetSkillDef : SkillDef
        {
            public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
            {
                return new InstanceData
                {
                    homingTracker = skillSlot.GetComponent<HomingTracker>()
                };
            }
            public override bool IsReady([NotNull] GenericSkill skillSlot)
            {
                return base.IsReady(skillSlot) && ((RequiresTargetSkillDef.InstanceData)skillSlot.skillInstanceData).homingTracker.GetTrackingTarget(true);
            }
            protected class InstanceData : BaseSkillInstanceData
            {
                public HomingTracker homingTracker;
            }
        }

        public class RequiresFormTargetSkillDef : RequiresTargetSkillDef, IRequiresFormSkillDef
        {
            public FormDef requiredForm { get; set; }
        }

        public class CyloopSkillDef : SkillDef
        {
            public SkillDef quickCyloopSkillDef { get; set; }
            public override bool CanExecute([NotNull] GenericSkill skillSlot)
            {
                return base.CanExecute(skillSlot) && skillSlot.characterBody.characterMotor && skillSlot.characterBody.characterMotor.velocity.magnitude >= skillSlot.characterBody.moveSpeed * SkillStates.Cyloop.Cyloop.minMoveSpeedPercent;
            }
        }
        public class RequiresFormCyloopSkillDef : CyloopSkillDef, IRequiresFormSkillDef
        {
            public FormDef requiredForm { get; set; }
        }
    }
}
