﻿using BepInEx.Configuration;
using SonicTheHedgehog.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using SonicTheHedgehog.SkillStates;
using On.RoR2.UI;
using RoR2.UI;
using SonicTheHedgehog.Components;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using static BetterUI.ProcCoefficientCatalog;
using AncientScepter;
using SonicTheHedgehog.Modules.Achievements;
using System.Linq;
using static RoR2.TeleporterInteraction;

namespace SonicTheHedgehog.Modules.Survivors
{
    internal class SonicTheHedgehogCharacter : SurvivorBase
    {
        //used when building your character using the prefabs you set up in unity
        //don't upload to thunderstore without changing this
        public override string prefabBodyName => "SonicTheHedgehog";

        public const string SONIC_THE_HEDGEHOG_PREFIX =
            SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => SONIC_THE_HEDGEHOG_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "SonicTheHedgehog",
            bodyNameToken = SONIC_THE_HEDGEHOG_PREFIX + "NAME",
            subtitleNameToken = SONIC_THE_HEDGEHOG_PREFIX + "SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texSonicIcon"),
            bodyColor = new Color(0.29f, 0.34f, 1f),

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1,
            armor = 0f,

            jumpCount = 2,
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
            new CustomRendererInfo
            {
                childName = "Model",
                material = Materials.CreateHopooMaterial("matSonic"),
            }
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(SonicEntityState);

        public override ItemDisplaysBase itemDisplays => null;

        //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig =>
            null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        private static UnlockableDef parryUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            bodyPrefab.GetComponent<CharacterDeathBehavior>().deathState =
                new EntityStates.SerializableEntityStateType(typeof(Death));
        }

        public override void InitializeUnlockables()
        {
            //Henry tutorial tells me to just uncomment something to get the mastery achievement but the uncommented stuff doesn't even compile
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>();
            masterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            masterySkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texMetalSkin");
            masterySkinUnlockableDef.cachedName = "SonicSkins.Mastery";
            masterySkinUnlockableDef.nameToken =
                "ACHIEVEMENT_" + SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE_NAME";
            Content.AddUnlockableDef(masterySkinUnlockableDef);

            // I hate achievements almost as much as I hate networking
            parryUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            parryUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texParryIcon");
            parryUnlockableDef.cachedName = "SonicSkills.Parry";
            parryUnlockableDef.nameToken =
                "ACHIEVEMENT_" + SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE_NAME";
            Content.AddUnlockableDef(parryUnlockableDef);

            /*UserProfile user = LocalUserManager.readOnlyLocalUsersList.FirstOrDefault(v => v != null)?.userProfile;
            if (!user.HasUnlockable(parryUnlockableDef) && Config.ForceUnlockParry().Value)
            {
                user.GrantUnlockable(parryUnlockableDef);
            }
            */
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();

            //example of how to create a hitbox
            Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            Modules.Prefabs.SetupHitbox(prefabCharacterModel.gameObject, hitboxTransform, "Sword");

            hitboxTransform = childLocator.FindChild("BallHitbox");
            Modules.Prefabs.SetupHitbox(prefabCharacterModel.gameObject, hitboxTransform, "Ball");

            hitboxTransform = childLocator.FindChild("LargeBallHitbox");
            Modules.Prefabs.SetupHitbox(prefabCharacterModel.gameObject, hitboxTransform, "LargeBall");

            hitboxTransform = childLocator.FindChild("StompHitbox");
            Modules.Prefabs.SetupHitbox(prefabCharacterModel.gameObject, hitboxTransform, "Stomp");
        }

        public static void UnlockParryConfig(object orig, EventArgs self)
        {
            // Thanks RealerCheatUnlocks
            Debug.Log("Unlock Parry Attempt");

            UserProfile user = LocalUserManager.readOnlyLocalUsersList.FirstOrDefault(v => v != null)?.userProfile;
            string achievement = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE";

            if (Config.ForceUnlockParry().Value)
            {
                if (!user.HasAchievement(achievement))
                {
                    user.AddAchievement(achievement, true);
                }

                if (!user.HasUnlockable(parryUnlockableDef))
                {
                    user.GrantUnlockable(parryUnlockableDef);
                }
            }
            else
            {
                if (user.HasAchievement(achievement))
                {
                    foreach (var notification in RoR2.UI.AchievementNotificationPanel.instancesList)
                        UnityEngine.Object.Destroy(notification.gameObject);
                    user.RevokeAchievement(achievement);
                }

                if (user.HasUnlockable(parryUnlockableDef))
                {
                    user.RevokeUnlockable(parryUnlockableDef);
                    user.RequestEventualSave();
                }
            }
        }

        public static void UnlockMasteryConfig(object orig, EventArgs self)
        {
            // Thanks RealerCheatUnlocks
            Debug.Log("Unlock Mastery Attempt");

            UserProfile user = LocalUserManager.readOnlyLocalUsersList.FirstOrDefault(v => v != null)?.userProfile;
            string achievement = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE";

            if (Config.ForceUnlockParry().Value)
            {
                if (!user.HasAchievement(achievement))
                {
                    user.AddAchievement(achievement, true);
                }

                if (!user.HasUnlockable(masterySkinUnlockableDef))
                {
                    user.GrantUnlockable(masterySkinUnlockableDef);
                }
            }
            else
            {
                if (user.HasAchievement(achievement))
                {
                    foreach (var notification in RoR2.UI.AchievementNotificationPanel.instancesList)
                        UnityEngine.Object.Destroy(notification.gameObject);
                    user.RevokeAchievement(achievement);
                }

                if (user.HasUnlockable(masterySkinUnlockableDef))
                {
                    user.RevokeUnlockable(masterySkinUnlockableDef);
                    user.RequestEventualSave();
                }
            }
        }

        public static SkillDef primarySkillDef;

        public static SkillDef sonicBoomSkillDef;
        public static SkillDef parrySkillDef;

        public static SkillDef boostSkillDef;

        public static SkillDef grandSlamSkillDef;


        public static SkillDef superSonicSkillDef;

        public static SkillDef momentumPassiveDef;

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = SonicTheHedgehogPlugin.DEVELOPER_PREFIX;

            bodyPrefab.AddComponent<Components.BoostLogic>();
            bodyPrefab.AddComponent<Components.MomentumPassive>();
            bodyPrefab.AddComponent<Components.HomingTracker>();

            On.RoR2.UI.HUD.Awake += CreateBoostMeterUI;

            #region Primary

            //Creates a skilldef for a typical primary
            SkillDefInfo primary = new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_MELEE_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_MELEE_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_MELEE_DESCRIPTION",
                keywordTokens = new string[] { prefix + "_SONIC_THE_HEDGEHOG_BODY_HOMING_KEYWORD" },
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMeleeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SonicMeleeEnter)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            };
            primarySkillDef = Modules.Skills.CreateSkillDef(primary);


            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);

            #endregion

            SkillDefInfo sonicBoom = new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_SONIC_BOOM_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_SONIC_BOOM_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_SONIC_BOOM_DESCRIPTION",
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
            };

            #region Secondary

            sonicBoomSkillDef = Modules.Skills.CreateSkillDef(sonicBoom);

            Modules.Skills.AddSecondarySkills(bodyPrefab, sonicBoomSkillDef);

            #endregion

            SkillDefInfo parry = new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_PARRY_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_PARRY_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_PARRY_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texParryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Parry)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 3f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            };

            #region Secondary

            parrySkillDef = Modules.Skills.CreateSkillDef(parry);

            Skills.AddSkillToFamily(bodyPrefab.GetComponent<SkillLocator>().secondary.skillFamily, parrySkillDef,
                parryUnlockableDef);

            #endregion

            #region Utility

            SkillDefInfo boost = new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_UTILITY_BOOST_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_UTILITY_BOOST_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_UTILITY_BOOST_DESCRIPTION",
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
            };
            boostSkillDef = Modules.Skills.CreateSkillDef(boost);

            Modules.Skills.AddUtilitySkills(bodyPrefab, boostSkillDef);

            #endregion

            #region Special

            SkillDefInfo grandSlam = new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_GRAND_SLAM_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_GRAND_SLAM_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_GRAND_SLAM_DESCRIPTION",
                keywordTokens = new string[] { prefix + "_SONIC_THE_HEDGEHOG_BODY_HOMING_KEYWORD" },
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
            };
            grandSlamSkillDef = Modules.Skills.CreateSkillDef(grandSlam);

            Modules.Skills.AddSpecialSkills(bodyPrefab, grandSlamSkillDef);

            #endregion

            #region Super Transformation

            superSonicSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_SUPER_TRANSFORMATION_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_SUPER_TRANSFORMATION_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_SUPER_TRANSFORMATION_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBazookaOutIcon"),
                activationState =
                    new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperSonicTransformation)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.Frozen,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 1
            });

            //Modules.Skills.AddSpecialSkills(bodyPrefab, superSonicSkillDef);

            #endregion


            // PASSIVES

            #region

            momentumPassiveDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_MOMENTUM_PASSIVE_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_MOMENTUM_PASSIVE_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_MOMENTUM_PASSIVE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMomentumIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SonicEntityState)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Modules.Skills.AddMiscSkills(bodyPrefab, momentumPassiveDef);

            #endregion

            MakeSuperSonicStuff(primary, sonicBoom, parry, boost, grandSlam);

            if (SonicTheHedgehogPlugin.ancientScepterLoaded)
            {
                ScepterSkill(boost);
            }
        }

        private void MakeSuperSonicStuff(SkillDefInfo primary, SkillDefInfo sonicBoom, SkillDefInfo parry,
            SkillDefInfo boost, SkillDefInfo grandSlam)
        {
            //EntityStateMachine superSonicState = bodyPrefab.AddComponent<EntityStateMachine>();
            //superSonicState.customName = "SonicForms";
            //superSonicState.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.BaseSonic));

            //bodyPrefab.AddComponent<Components.SuperSonicComponent>();

            Debug.Log("Making Super Sonic: Starting Stuff");

            primary.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.SuperSonicMeleeEnter));
            primary.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PRIMARY_MELEE_NAME";
            primary.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PRIMARY_MELEE_NAME";
            primary.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PRIMARY_MELEE_DESCRIPTION";
            primary.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperMeleeIcon");
            SuperSonic.melee = Modules.Skills.CreateSkillDef(primary);

            sonicBoom.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.SuperSonicBoom));
            sonicBoom.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_SONIC_BOOM_NAME";
            sonicBoom.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_SONIC_BOOM_NAME";
            sonicBoom.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_SONIC_BOOM_DESCRIPTION";
            sonicBoom.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texCrossSlashIcon");
            SuperSonic.sonicBoom = Modules.Skills.CreateSkillDef(sonicBoom);

            parry.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.SuperParry));
            parry.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_PARRY_NAME";
            parry.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_PARRY_NAME";
            parry.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_PARRY_DESCRIPTION";
            parry.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperParryIcon");
            SuperSonic.parry = Modules.Skills.CreateSkillDef(parry);

            parry.requiredStock = 9999999;
            parry.rechargeStock = 0;
            parry.fullRestockOnAssign = false;
            SuperSonic.emptyParry = Modules.Skills.CreateSkillDef(parry);

            SkillDefInfo idwAttack = new SkillDefInfo
            {
                skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_IDW_ATTACK_NAME",
                skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_IDW_ATTACK_NAME",
                skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_IDW_ATTACK_DESCRIPTION",
                keywordTokens = new string[]
                    { SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_HOMING_KEYWORD" },
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIDWAttackIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(IDWAttackSearch)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 3f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            };

            SuperSonic.idwAttack = Skills.CreateSkillDef(idwAttack);

            boost.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.SuperBoost));
            boost.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_UTILITY_BOOST_NAME";
            boost.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_UTILITY_BOOST_NAME";
            boost.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_UTILITY_BOOST_DESCRIPTION";
            boost.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperBoostIcon");
            SuperSonic.boost = Modules.Skills.CreateSkillDef(boost);

            grandSlam.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.SuperGrandSlamDash));
            grandSlam.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SPECIAL_GRAND_SLAM_NAME";
            grandSlam.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SPECIAL_GRAND_SLAM_NAME";
            grandSlam.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SPECIAL_GRAND_SLAM_DESCRIPTION";
            grandSlam.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperGrandSlamIcon");
            SuperSonic.grandSlam = Modules.Skills.CreateSkillDef(grandSlam);

            Debug.Log("Making Super Sonic: All Skills");

            //NetworkStateMachine network = bodyPrefab.GetComponent<NetworkStateMachine>();
            //Helpers.Append(ref network.stateMachines, new List<EntityStateMachine> { superSonicState });
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ScepterSkill(SkillDefInfo boost)
        {
            Debug.Log("Sonic Scepter skill started");
            boost.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SCEPTER_UTILITY_BOOST_NAME";
            boost.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SCEPTER_UTILITY_BOOST_NAME";
            boost.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SCEPTER_UTILITY_BOOST_DESCRIPTION";
            boost.activationState = new EntityStates.SerializableEntityStateType(typeof(ScepterBoost));
            boost.skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texScepterBoostIcon");
            SkillDef skillDef = Skills.CreateSkillDef(boost);
            Content.AddEntityState(typeof(ScepterBoost));
            Debug.Log("Sonic Scepter skill created? " +
                      (ItemBase<AncientScepterItem>.instance.RegisterScepterSkill(skillDef, "SonicTheHedgehog",
                          boostSkillDef)).ToString());

            boost.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SUPER_SCEPTER_UTILITY_BOOST_NAME";
            boost.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SUPER_SCEPTER_UTILITY_BOOST_NAME";
            boost.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SUPER_SCEPTER_UTILITY_BOOST_DESCRIPTION";
            boost.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.SuperScepterBoost));
            boost.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperScepterBoostIcon");
            skillDef = Skills.CreateSkillDef(boost);
            Debug.Log("Super Sonic Scepter skill created? " +
                      (ItemBase<AncientScepterItem>.instance.RegisterScepterSkill(skillDef, "SonicTheHedgehog",
                          SuperSonic.boost)).ToString());

            if (SonicTheHedgehogPlugin.betterUILoaded)
            {
                ScepterBetterUI();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ScepterBetterUI()
        {
            AddSkill(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SCEPTER_UTILITY_BOOST_NAME",
                new ProcCoefficientInfo
                {
                    name = "Boost",
                    procCoefficient = StaticValues.scepterBoostProcCoefficient
                });

            AddSkill(
                SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SCEPTER_UTILITY_BOOST_NAME",
                new ProcCoefficientInfo
                {
                    name = "Boost",
                    procCoefficient = StaticValues.scepterBoostProcCoefficient
                });
        }

        internal static void CreateBoostMeterUI(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig.Invoke(self);
            BoostHUD boostHud = self.gameObject.AddComponent<BoostHUD>();
            GameObject boostUI = UnityEngine.Object.Instantiate<GameObject>(
                Assets.mainAssetBundle.LoadAsset<GameObject>("BoostMeter2"),
                self.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas"));
            RectTransform rectTransform = boostUI.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
            rectTransform.localPosition = Vector3.zero;

            rectTransform.anchoredPosition = new Vector2(Modules.Config.BoostMeterLocationX().Value,
                Modules.Config.BoostMeterLocationY().Value);

            boostHud.boostMeter = boostUI;
            boostHud.meterBackground = boostUI.transform.Find("Background").gameObject.GetComponent<Image>();
            boostHud.meterFill = boostUI.transform.Find("Background/Fill").gameObject.GetComponent<Image>();
            boostHud.meterBackgroundOuter = boostUI.transform.Find("BackgroundOuter").gameObject.GetComponent<Image>();
            boostHud.meterFillOuter =
                boostUI.transform.Find("BackgroundOuter/FillOuter").gameObject.GetComponent<Image>();
            boostHud.meterBackgroundBackup =
                boostUI.transform.Find("BackgroundBackup").gameObject.GetComponent<RawImage>();
            boostHud.meterFillBackup = boostUI.transform.Find("BackgroundBackup/FillBackup").gameObject
                .GetComponent<RawImage>();
            boostHud.powerBoostParticle =
                boostUI.transform.Find("PowerParticles").gameObject.GetComponent<ParticleSystem>();
            boostHud.infiniteBackground = boostUI.transform.Find("InfiniteBackground").gameObject.GetComponent<Image>();
            boostHud.infiniteFill = boostUI.transform.Find("InfiniteBackground/InfiniteFill").gameObject
                .GetComponent<Image>();
        }

        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin

            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            defaultSkin.meshReplacements = Modules.Skins.GetMeshReplacementsFromObject(defaultRendererinfos,
                "SonicMesh");
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);

            #endregion

            //uncomment this when you have a mastery skin

            #region MasterySkin


            //creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMetalSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject
                ,masterySkinUnlockableDef);
                //);
            //adding the mesh replacements as above.
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.GetMeshReplacementsFromObject(defaultRendererinfos,
                "MetalSonicMesh");

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos defaultMaterials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matMetalSonic");

            //here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            /*masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("GunModel"),
                    shouldActivate = false,
                }
            };
            */
            //simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(masterySkin);
            

            #endregion

            skinController.skins = skins.ToArray();
        }
    }
}