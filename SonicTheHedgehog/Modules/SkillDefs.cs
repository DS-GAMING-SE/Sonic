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
        public interface IMeleeSkill
        {
            HomingTracker homingTracker { get; set; }
            SerializableEntityStateType homingAttackState { get; set; }
        }
        public class MeleeSkillDef : SkillDef, IMeleeSkill
        {
            public HomingTracker homingTracker { get; set; }

            public SerializableEntityStateType homingAttackState { get; set; }

            public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
            {
                homingTracker = skillSlot.GetComponent<HomingTracker>();
                return null;
            }

            public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
            {
                return DecideNextState(skillSlot, homingTracker, 0);
            }
            public static EntityState DecideNextState(GenericSkill skillSlot, HomingTracker homingTracker, int swingIndex)
            {
                if (homingTracker && homingTracker.CanHomingAttack())
                {
                    EntityState entityState = EntityStateCatalog.InstantiateState(((IMeleeSkill)skillSlot.skillDef).homingAttackState.stateType);
                    ISkillState skillState = entityState as ISkillState;
                    if (skillState != null)
                    {
                        skillState.activatorSkillSlot = skillSlot;
                    }
                    if (typeof(HomingAttack).IsAssignableFrom(((IMeleeSkill)skillSlot.skillDef).homingAttackState.stateType))
                    {
                        ((HomingAttack)entityState).target = homingTracker.GetTrackingTarget();
                    }
                    return entityState;
                }
                else
                {
                    EntityState entityState = EntityStateCatalog.InstantiateState(skillSlot.activationState.stateType);
                    ISkillState skillState = entityState as ISkillState;
                    if (skillState != null)
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
        }

        public class RequiresFormMeleeSkillDef : RequiresFormSkillDef, IMeleeSkill
        {
            public HomingTracker homingTracker { get; set; }

            public SerializableEntityStateType homingAttackState { get; set; }

            public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
            {
                homingTracker = skillSlot.GetComponent<HomingTracker>();
                return base.OnAssigned(skillSlot);
            }

            public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
            {
                return MeleeSkillDef.DecideNextState(skillSlot, homingTracker, 0);
            }
        }
    }
}
