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
            SerializableEntityStateType homingAttackState { get; set; }
        }
        public class MeleeSkillDef : SkillDef, IMeleeSkill
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

            protected class InstanceData : BaseSkillInstanceData
            {
                public HomingTracker homingTracker;
            }
        }

        public class RequiresFormMeleeSkillDef : RequiresFormSkillDef, IMeleeSkill
        {
            public SerializableEntityStateType homingAttackState { get; set; }

            public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
            {
                MeleeInstanceData instanceData = new MeleeInstanceData
                {
                    homingTracker = skillSlot.GetComponent<HomingTracker>()
                };
                instanceData.formComponent = ((InstanceData)base.OnAssigned(skillSlot)).formComponent;
                return instanceData;
            }

            public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
            {
                return MeleeSkillDef.DecideNextState(skillSlot, ((MeleeInstanceData)skillSlot.skillInstanceData).homingTracker, 0);
            }

            protected class MeleeInstanceData : RequiresFormSkillDef.InstanceData
            {
                public HomingTracker homingTracker;
            }
        }

        public static T CopyMeleeSkillDef<T>(MeleeSkillDef originDef) where T : SkillDef, IMeleeSkill
        {
            SerializableEntityStateType homing = originDef.homingAttackState;
            T meleeDef = HedgehogUtils.Helpers.CopySkillDef<T>(originDef);
            meleeDef.homingAttackState = homing;
            return meleeDef;
        }
        public static T CopyMeleeSkillDef<T>(RequiresFormMeleeSkillDef originDef) where T : SkillDef, IMeleeSkill
        {
            SerializableEntityStateType homing = originDef.homingAttackState;
            T meleeDef = HedgehogUtils.Helpers.CopySkillDef<T>(originDef);
            meleeDef.homingAttackState = homing;
            return meleeDef;
        }
    }
}
