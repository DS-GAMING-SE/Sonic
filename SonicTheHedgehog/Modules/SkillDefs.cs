using EntityStates;
using HedgehogUtils.Boost.EntityStates;
using HedgehogUtils.Forms;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static HedgehogUtils.Forms.SkillDefs;
using static SonicTheHedgehog.Modules.SkillDefs;

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
                return base.IsReady(skillSlot) && ((RequiresTargetSkillDef.InstanceData)skillSlot.skillInstanceData).homingTracker.GetTrackingTarget(true) && skillSlot.characterBody.moveSpeed > 0;
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
            public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
            {
                return new CyloopInstanceData()
                {
                    primarySkill = skillSlot.GetComponent<SkillLocator>().primary,
                    inputBank = skillSlot.GetComponent<InputBankTest>(),
                    bodyStateMachine = EntityStateMachine.FindByCustomName(skillSlot.gameObject, "Body"),
                    homingTracker = skillSlot.GetComponent<HomingTracker>()
                };
            }
            public override bool CanExecute([NotNull] GenericSkill skillSlot)
            {
                return base.CanExecute(skillSlot) && skillSlot.characterBody.moveSpeed > 0 && ((skillSlot.characterBody.isPlayerControlled && skillSlot.characterBody.characterMotor && skillSlot.characterBody.characterMotor.velocity.magnitude >= skillSlot.characterBody.moveSpeed * SkillStates.Cyloop.Cyloop.minMoveSpeedPercent)
                    || (!skillSlot.characterBody.isPlayerControlled && ((CyloopInstanceData)skillSlot.skillInstanceData).homingTracker.GetTrackingTarget(true)));
            }
            public override void OnExecute([NotNull] GenericSkill skillSlot)
            {
                if (skillSlot.characterBody.isPlayerControlled)
                {
                    base.OnExecute(skillSlot);
                }
                else
                {
                    // rewriting base is normal, Drifter does it
                    ((CyloopInstanceData)skillSlot.skillInstanceData).bodyStateMachine.SetInterruptState(EntityStateCatalog.InstantiateState(ref quickCyloopSkillDef.activationState), quickCyloopSkillDef.interruptPriority);
                }
            }

            public override void OnFixedUpdate([NotNull] GenericSkill skillSlot, float deltaTime)
            {
                base.OnFixedUpdate(skillSlot, deltaTime);
                CyloopInstanceData instanceData = (CyloopInstanceData)skillSlot.skillInstanceData;
                if (IsReady(skillSlot) && instanceData.inputBank.skill4.down && typeof(GenericCharacterMain).IsAssignableFrom(instanceData.bodyStateMachine.state.GetType()))
                {
                    instanceData.primarySkill.SetSkillOverride(this, quickCyloopSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                }
                else
                {
                    instanceData.primarySkill.UnsetSkillOverride(this, quickCyloopSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                }
            }
            public override void OnUnassigned([NotNull] GenericSkill skillSlot)
            {
                base.OnUnassigned(skillSlot);
                ((CyloopInstanceData)skillSlot.skillInstanceData).primarySkill.UnsetSkillOverride(this, quickCyloopSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            protected class CyloopInstanceData : BaseSkillInstanceData 
            {
                public GenericSkill primarySkill;
                public InputBankTest inputBank;
                public EntityStateMachine bodyStateMachine;
                public HomingTracker homingTracker;
            }
        }
        public class RequiresFormCyloopSkillDef : CyloopSkillDef, IRequiresFormSkillDef
        {
            public FormDef requiredForm { get; set; }
        }
    }
}
