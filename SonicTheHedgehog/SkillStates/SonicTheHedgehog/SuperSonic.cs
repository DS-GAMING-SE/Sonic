using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class SuperSonic : BaseState
    {
        SuperSonicComponent superSonicComponent;
        Material prevMaterial;
        GameObject superAura;
        bool superBuffApplied;
        CharacterModel model;
        public override void OnEnter()
        {
            base.OnEnter();
            superSonicComponent = base.GetComponent<SuperSonicComponent>();
            //model = base.characterBody.modelLocator.GetComponent<CharacterModel>();

            Transform transform = base.gameObject.transform;
            this.superAura = GameObject.Instantiate<GameObject>(Modules.Assets.superSonicAura, transform.position, transform.rotation);
            //this.superAura = PrefabAPI.InstantiateClone(Modules.Assets.superSonicAura, "SuperAura", true);
            this.superAura.transform.parent = transform;

            //this.model.baseRendererInfos[0].defaultMaterial = Assets.mainAssetBundle.LoadAsset<Material>("matSuperSonic");



            if (base.isAuthority && base.characterBody.healthComponent)
            {
                base.characterBody.healthComponent.HealFraction(1, new ProcChainMask());
            }
            if (base.isAuthority && base.skillLocator)
            {
                base.skillLocator.primary.SetSkillOverride(this, primarySkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                base.skillLocator.secondary.SetSkillOverride(this, shootSkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                base.skillLocator.utility.SetSkillOverride(this, rollSkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                base.skillLocator.special.SetSkillOverride(this, bombSkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                EffectManager.SimpleMuzzleFlash(Modules.Assets.superSonicTransformationEffect, base.gameObject, "MainHurtbox", true);
            }
            if (NetworkServer.active)
            {
                RoR2.Util.CleanseBody(base.characterBody, true, false, true, true, true, false);
                base.characterBody.AddTimedBuff(Modules.Buffs.superSonicBuff, Modules.StaticValues.superSonicDuration, 1);
                base.characterBody.AddTimedBuff(RoR2Content.Buffs.Immune, Modules.StaticValues.superSonicDuration, 1);
            }

        }

        public override void OnExit()
        {
            if (this.superAura)
            {
                Destroy(this.superAura);
            }
            else
            {
                Chat.AddMessage("no aura found");
            }

            //this.model.baseRendererInfos[0].defaultMaterial = this.model.gameObject.GetComponent<ModelSkinController>().skins[this.characterBody.skinIndex].rendererInfos[0].defaultMaterial;

            if (base.isAuthority && base.skillLocator)
            {
                base.skillLocator.primary.UnsetSkillOverride(this, primarySkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                base.skillLocator.secondary.UnsetSkillOverride(this, shootSkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                base.skillLocator.utility.UnsetSkillOverride(this, rollSkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                base.skillLocator.special.UnsetSkillOverride(this, bombSkillDef, GenericSkill.SkillOverridePriority.Upgrade);
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterBody.HasBuff(Modules.Buffs.superSonicBuff))
            {
                if (!superBuffApplied)
                {
                    superBuffApplied = true;
                }
            }
            else if (superBuffApplied)
            {
                if (superSonicComponent)
                {
                    superSonicComponent.superSonicState.SetNextState(new BaseSonic());
                }
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PRIMARY_SLASH_NAME",
                                                                                      SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PRIMARY_SLASH_DESCRIPTION",
                                                                                      Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMeleeIcon"),
                                                                                      new EntityStates.SerializableEntityStateType(typeof(SkillStates.SonicMelee)),
                                                                                      "Body",
                                                                                      false));

        SkillDef shootSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
        {
            skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_SONIC_BOOM_NAME",
            skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_SONIC_BOOM_NAME",
            skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_SONIC_BOOM_DESCRIPTION",
            skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSonicBoomIcon"),
            activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SonicBoom)),
            activationStateMachineName = "Body",
            baseMaxStock = 3,
            baseRechargeInterval = 5f,
            beginSkillCooldownOnSkillEnd = true,
            canceledFromSprinting = true,
            forceSprintDuringState = false,
            fullRestockOnAssign = true,
            interruptPriority = EntityStates.InterruptPriority.Skill,
            resetCooldownTimerOnUse = true,
            isCombatSkill = true,
            mustKeyPress = true,
            cancelSprintingOnActivation = true,
            rechargeStock = 99999,
            requiredStock = 1,
            stockToConsume = 1,
        });

        SkillDef rollSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
        {
            skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_UTILITY_BOOST_NAME",
            skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_UTILITY_BOOST_NAME",
            skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_UTILITY_BOOST_DESCRIPTION",
            skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBoostIcon"),
            activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Boost)),
            activationStateMachineName = "Body",
            baseMaxStock = 1,
            baseRechargeInterval = 0f,
            beginSkillCooldownOnSkillEnd = true,
            canceledFromSprinting = false,
            forceSprintDuringState = false,
            fullRestockOnAssign = true,
            interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
            resetCooldownTimerOnUse = false,
            isCombatSkill = false,
            mustKeyPress = true,
            cancelSprintingOnActivation = false,
            rechargeStock = 0,
            requiredStock = 1,
            stockToConsume = 0
        });

        SkillDef bombSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
        {
            skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SPECIAL_BOMB_NAME",
            skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SPECIAL_BOMB_NAME",
            skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SPECIAL_BOMB_DESCRIPTION",
            skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGrandSlamIcon"),
            activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.GrandSlamDash)),
            activationStateMachineName = "Body",
            baseMaxStock = 1,
            baseRechargeInterval = 12f,
            beginSkillCooldownOnSkillEnd = true,
            canceledFromSprinting = false,
            forceSprintDuringState = false,
            fullRestockOnAssign = true,
            interruptPriority = EntityStates.InterruptPriority.Skill,
            resetCooldownTimerOnUse = false,
            isCombatSkill = true,
            mustKeyPress = true,
            cancelSprintingOnActivation = true,
            rechargeStock = 1,
            requiredStock = 1,
            stockToConsume = 1
        });
    }
}