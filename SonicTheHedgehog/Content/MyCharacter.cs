using AncientScepter;
using BepInEx.Configuration;
using EmotesAPI;
using HarmonyLib;
using HedgehogUtils;
using HedgehogUtils.Forms;
using HedgehogUtils.Forms.SuperForm;
using HG;
using On.RoR2.UI;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using RoR2.UI;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules.Achievements;
using SonicTheHedgehog.Modules.Characters;
using SonicTheHedgehog.SkillStates;
using SonicTheHedgehog.SkillStates.Initial;
using SonicTheHedgehog.SkillStates.SuperUpgrades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static BetterUI.ProcCoefficientCatalog;
using static RoR2.TeleporterInteraction;
using static SonicTheHedgehog.Modules.SkillDefs;

namespace SonicTheHedgehog.Modules.Survivors
{
    internal class SonicTheHedgehogCharacter : SurvivorBase
    {
        //used when building your character using the prefabs you set up in unity
        //don't upload to thunderstore without changing this
        public override string prefabBodyName => "SonicTheHedgehog";

        public const string SONIC_THE_HEDGEHOG_PREFIX =
            SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_";

        public static Color sonicColor = new Color(0.29f, 0.34f, 1f);
        public static Color sonicColor2 = new Color(0, 0.7f, 1);
        public static Color superSonicColor = new Color(1f, 0.9f, 0);
        public static Color superSonicColor2 = new Color(1f, 0.6f, 0.4f);

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => SONIC_THE_HEDGEHOG_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "SonicTheHedgehog",
            bodyNameToken = SONIC_THE_HEDGEHOG_PREFIX + "NAME",
            subtitleNameToken = SONIC_THE_HEDGEHOG_PREFIX + "SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texSonicIcon"),
            bodyColor = sonicColor,

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = null,

            maxHealth = 110f,
            healthRegen = 1,
            armor = 0f,

            jumpCount = 2,

            sortPosition = 137
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
            new CustomRendererInfo
            {
                childName = "Model",
                material = Materials.CreateHopooMaterial("matSonic").Specular(0.3f),
            }
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(SonicEntityState);

        public override ItemDisplaysBase itemDisplays => null;

        //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig =>
            null; //Modules.Config.CharacterEnableConfig(bodyName);

        public static UnlockableDef masterySkinUnlockableDef;

        public static UnlockableDef grandMasterySkinUnlockableDef;
        public static UnlockableDef meridianSkinUnlockableDef;
        public static UnlockableDef decompileSkinUnlockableDef;
        public static UnlockableDef purgeSkinUnlockableDef;

        public static UnlockableDef parryUnlockableDef;
        public static UnlockableDef dieUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            bodyPrefab.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(HedgehogUtils.Miscellaneous.Death));
            prefabCharacterBody.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(Descent));
        }

        public override void InitializeUnlockables()
        {
            masterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            masterySkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texMetalSkinIcon");
            masterySkinUnlockableDef.cachedName = "Skins.Sonic.Alt1";
            masterySkinUnlockableDef.nameToken = "ACHIEVEMENT_" + SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE_NAME";
            Content.AddUnlockableDef(masterySkinUnlockableDef);

            grandMasterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            grandMasterySkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texExcaliburSkinIcon");
            grandMasterySkinUnlockableDef.cachedName = SonicGrandMasteryAchievement.unlockableIdentifier;
            grandMasterySkinUnlockableDef.nameToken = "ACHIEVEMENT_" + SonicGrandMasteryAchievement.identifier + "_NAME";
            Content.AddUnlockableDef(grandMasterySkinUnlockableDef);

            meridianSkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            meridianSkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texNoPlaceSkinIcon");
            meridianSkinUnlockableDef.cachedName = SonicMeridianEventTriggerAchievement.unlockableIdentifier;
            meridianSkinUnlockableDef.nameToken = "ACHIEVEMENT_" + SonicMeridianEventTriggerAchievement.identifier + "_NAME";
            Content.AddUnlockableDef(meridianSkinUnlockableDef);

            decompileSkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            decompileSkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texRewriteSkinIcon");
            decompileSkinUnlockableDef.cachedName = SonicDecompileAchievement.unlockableIdentifier;
            decompileSkinUnlockableDef.nameToken = "ACHIEVEMENT_" + SonicDecompileAchievement.identifier + "_NAME";
            Content.AddUnlockableDef(decompileSkinUnlockableDef);

            purgeSkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            purgeSkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texACMetalSkinIcon");
            purgeSkinUnlockableDef.cachedName = SonicPurgeAchievement.unlockableIdentifier;
            purgeSkinUnlockableDef.nameToken = "ACHIEVEMENT_" + SonicPurgeAchievement.identifier + "_NAME";
            Content.AddUnlockableDef(purgeSkinUnlockableDef);

            parryUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            parryUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texParryIcon");
            parryUnlockableDef.cachedName = "SonicSkills.Parry";
            parryUnlockableDef.nameToken = "ACHIEVEMENT_" + SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE_NAME";
            Content.AddUnlockableDef(parryUnlockableDef);

            dieUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            dieUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSailorSkinIcon");
            dieUnlockableDef.cachedName = SonicDieAchievement.unlockableIdentifier;
            dieUnlockableDef.nameToken = "ACHIEVEMENT_" + SonicDieAchievement.identifier + "_NAME";
            Content.AddUnlockableDef(dieUnlockableDef);
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

            hitboxTransform = childLocator.FindChild("ParryFollowUpHitbox");
            Modules.Prefabs.SetupHitbox(prefabCharacterModel.gameObject, hitboxTransform, "FollowUp");
        }

        public static void UnlockParryConfig(object orig, EventArgs self)
        {
            // Thanks RealerCheatUnlocks
            Log.Message("Unlock Parry Attempt");

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
            Log.Message("Unlock Mastery Attempt");

            UserProfile user = LocalUserManager.readOnlyLocalUsersList.FirstOrDefault(v => v != null)?.userProfile;
            string achievement = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE";

            if (Config.ForceUnlockMastery().Value)
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

        public static SkillDefs.MeleeSkillDef primarySkillDef;

        public static SkillDef sonicBoomSkillDef;
        public static SkillDef parrySkillDef;
        public static SkillDef followUpSkillDef;

        public static HedgehogUtils.Boost.SkillDefs.BoostSkillDef boostSkillDef;

        public static SkillDef grandSlamSkillDef;
        public static SkillDef tomPunchSkillDef;


        public static SkillDef superSonicSkillDef;

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = SonicTheHedgehogPlugin.DEVELOPER_PREFIX;

            //bodyPrefab.AddComponent<Components.PowerBoostLogic>();
            bodyPrefab.AddComponent<PowerBoostLogic>();
            bodyPrefab.AddComponent<Components.HomingTracker>();
            bodyPrefab.AddComponent<HedgehogUtils.Miscellaneous.StayOnGround>();
            bodyPrefab.AddComponent<ParryFollowUpTracker>();
            bodyPrefab.AddComponent<SuperSkillReplacer>();
            bodyPrefab.AddComponent<HedgehogUtils.Miscellaneous.MomentumPassive>();
            bodyPrefab.AddComponent<UniqueSkinEffect>();
            var sfxComponent = bodyPrefab.GetComponent<SfxLocator>();
            sfxComponent.deathSound = "Play_hedgehogutils_death";
            sfxComponent.jumpSound = "Play_hedgehogutils_jump_ball";

            if (SonicTheHedgehogPlugin.emoteAPILoaded)
            {
                EmoteSupport();
            }

            #region Primary

            //Creates a skilldef for a typical primary
            SkillDefInfo primary = new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_MELEE_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_MELEE_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_MELEE_DESCRIPTION",
                keywordTokens = new string[] { prefix + "_SONIC_THE_HEDGEHOG_BODY_HOMING_KEYWORD", HedgehogUtilsPlugin.Prefix+"LAUNCH_KEYWORD" },
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMeleeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SonicMelee)),
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
            primarySkillDef = Modules.Skills.CreateSkillDef<SkillDefs.MeleeSkillDef>(primary);

            primarySkillDef.homingAttackState = new EntityStates.SerializableEntityStateType(typeof(HomingAttack));

            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);

            #endregion

            #region Secondary Sonic Boom

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

            sonicBoomSkillDef = Modules.Skills.CreateSkillDef(sonicBoom);

            Modules.Skills.AddSecondarySkills(bodyPrefab, sonicBoomSkillDef);

            #endregion

            #region Secondary Parry

            SkillDefInfo parry = new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_PARRY_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_PARRY_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_PARRY_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texParryIcon"),
                keywordTokens = new string[] { HedgehogUtilsPlugin.Prefix + "LAUNCH_KEYWORD" },
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Parry)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 3f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            };

            parrySkillDef = Modules.Skills.CreateSkillDef(parry);
            parrySkillDef.autoHandleLuminousShot = false;

            Skills.AddSkillToFamily(bodyPrefab.GetComponent<SkillLocator>().secondary.skillFamily, parrySkillDef,
                parryUnlockableDef);

            #endregion

            #region Secondary Parry Follow Up

            SkillDefInfo followUp = new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_PARRY_FOLLOW_UP_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_PARRY_FOLLOW_UP_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_PARRY_FOLLOW_UP_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texFollowUpIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.FollowUp)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
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

            followUpSkillDef = Modules.Skills.CreateSkillDef(followUp);

            ParryExit.followUpSkillDef = followUpSkillDef;

            #endregion

            #region New Utility

            SkillDefInfo boost = new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_UTILITY_BOOST_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_UTILITY_BOOST_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_UTILITY_BOOST_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBoostIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.NewBoost)),
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
            boostSkillDef = Modules.Skills.CreateSkillDef<HedgehogUtils.Boost.SkillDefs.BoostSkillDef>(boost);
            boostSkillDef.boostIdleState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.BoostIdle));
            boostSkillDef.brakeState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SonicBrake));
            boostSkillDef.boostHUDColor = new Color(0, 0.9f, 1, 1);
            Modules.Skills.AddUtilitySkills(bodyPrefab, boostSkillDef);

            #endregion

            #region Special

            SkillDefInfo grandSlam = new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_GRAND_SLAM_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_GRAND_SLAM_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_GRAND_SLAM_DESCRIPTION",
                keywordTokens = new string[] { prefix + "_SONIC_THE_HEDGEHOG_BODY_HOMING_KEYWORD", HedgehogUtilsPlugin.Prefix + "LAUNCH_KEYWORD" },
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
            grandSlamSkillDef.isCooldownBlockedUntilManuallyReset = true;

            Modules.Skills.AddSpecialSkills(bodyPrefab, grandSlamSkillDef);

            #endregion

            SonicSkillDefs.Initialize(primarySkillDef, sonicBoomSkillDef, parrySkillDef, followUpSkillDef, boostSkillDef, grandSlamSkillDef);

            // PASSIVES

            #region Passive
            bodyPrefab.GetComponent<SkillLocator>().passiveSkill = new SkillLocator.PassiveSkill
            {
                enabled = true,
                skillNameToken = HedgehogUtils.Language.momentumPassiveNameToken,
                skillDescriptionToken = HedgehogUtils.Language.momentumPassiveDescriptionToken,
                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMomentumIcon"),
            };
            /*momentumPassiveDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = HedgehogUtils.Language.momentumPassiveNameToken,
                skillNameToken = HedgehogUtils.Language.momentumPassiveNameToken,
                skillDescriptionToken = HedgehogUtils.Language.momentumPassiveDescriptionToken,
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

            Modules.Skills.AddMiscSkills(bodyPrefab, momentumPassiveDef); */

            GenericSkill voicelinesGenericSkill = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "Voicelines", true);
            voicelinesGenericSkill.loadoutTitleToken = HedgehogUtils.Language.voicelinesTitleToken;
            SkillDef voicelinesEnable = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SonicVoicelinesEnable",
                skillNameToken = HedgehogUtils.Language.voicelinesEnableToken,
                skillDescriptionToken = SONIC_THE_HEDGEHOG_PREFIX + "VOICELINES_ENABLE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texVoicelinesEnableIcon")

            });
            SkillDef voicelinesDisable = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SonicVoicelinesDisable",
                skillNameToken = HedgehogUtils.Language.voicelinesDisableToken,
                skillDescriptionToken = "",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texVoicelinesDisableIcon")

            });
            Skills.AddSkillsToFamily(voicelinesGenericSkill.skillFamily, voicelinesEnable, voicelinesDisable);

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

            Log.Message("Making Super Sonic: Starting Stuff");

            primary.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.SuperSonicMelee));
            primary.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PRIMARY_MELEE_NAME";
            primary.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PRIMARY_MELEE_NAME";
            primary.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PRIMARY_MELEE_DESCRIPTION";
            primary.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperMeleeIcon");

            SuperSkillReplacer.melee = Modules.Skills.CreateSkillDef<SkillDefs.RequiresFormMeleeSkillDef>(primary);
            SuperSkillReplacer.melee.requiredForm = HedgehogUtils.Forms.SuperForm.SuperFormDef.superFormDef;
            SuperSkillReplacer.melee.homingAttackState = new EntityStates.SerializableEntityStateType(typeof(SuperHomingAttack));

            sonicBoom.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.SuperSonicBoom));
            sonicBoom.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_SONIC_BOOM_NAME";
            sonicBoom.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_SONIC_BOOM_NAME";
            sonicBoom.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_SONIC_BOOM_DESCRIPTION";
            sonicBoom.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texCrossSlashIcon");

            SuperSkillReplacer.sonicBoom = Modules.Skills.CreateSkillDef<HedgehogUtils.Forms.SkillDefs.RequiresFormSkillDef>(sonicBoom);
            SuperSkillReplacer.sonicBoom.requiredForm = HedgehogUtils.Forms.SuperForm.SuperFormDef.superFormDef;

            parry.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.SuperParry));
            parry.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_PARRY_NAME";
            parry.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_PARRY_NAME";
            parry.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_PARRY_DESCRIPTION";
            parry.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperParryIcon");
            parry.fullRestockOnAssign = true;
            SuperSkillReplacer.parry = Modules.Skills.CreateSkillDef<HedgehogUtils.Forms.SkillDefs.RequiresFormSkillDef>(parry);
            SuperSkillReplacer.parry.requiredForm = HedgehogUtils.Forms.SuperForm.SuperFormDef.superFormDef;

            HedgehogUtils.Forms.SkillDefs.RequiresFormSkillDef superFollowUp = HedgehogUtils.Helpers.CopySkillDef<HedgehogUtils.Forms.SkillDefs.RequiresFormSkillDef>(followUpSkillDef);
            superFollowUp.activationState = new EntityStates.SerializableEntityStateType(typeof(SuperFollowUp));
            superFollowUp.requiredForm = HedgehogUtils.Forms.SuperForm.SuperFormDef.superFormDef;
            superFollowUp.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_PARRY_FOLLOW_UP_DESCRIPTION";
            superFollowUp.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_PARRY_FOLLOW_UP_NAME";
            superFollowUp.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperFollowUpIcon");
            superFollowUp.baseRechargeInterval = 5f;
            SuperSkillReplacer.afterIDWAttack = superFollowUp;

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

            SuperParryExit.idwAttackSkillDef = Skills.CreateSkillDef<RequiresFormTargetSkillDef>(idwAttack);
            SuperParryExit.idwAttackSkillDef.requiredForm = HedgehogUtils.Forms.SuperForm.SuperFormDef.superFormDef;

            SonicSkillDefs.idwAttackSkillDef = SuperParryExit.idwAttackSkillDef;

            boost.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.NewSuperBoost));
            boost.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_UTILITY_BOOST_NAME";
            boost.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_UTILITY_BOOST_NAME";
            boost.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_UTILITY_BOOST_DESCRIPTION";
            boost.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperBoostIcon");

            SuperSkillReplacer.boost = Modules.Skills.CreateSkillDef<HedgehogUtils.Boost.SkillDefs.RequiresFormBoostSkillDef>(boost);
            SuperSkillReplacer.boost.requiredForm = HedgehogUtils.Forms.SuperForm.SuperFormDef.superFormDef;
            SuperSkillReplacer.boost.boostIdleState = new EntityStates.SerializableEntityStateType(typeof(BoostIdle));
            SuperSkillReplacer.boost.brakeState = new EntityStates.SerializableEntityStateType(typeof(SonicBrake));
            SuperSkillReplacer.boost.boostHUDColor = superSonicColor;

            grandSlam.activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperUpgrades.SuperGrandSlamDash));
            grandSlam.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SPECIAL_GRAND_SLAM_NAME";
            grandSlam.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SPECIAL_GRAND_SLAM_NAME";
            grandSlam.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SPECIAL_GRAND_SLAM_DESCRIPTION";
            grandSlam.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperGrandSlamIcon");
            SuperSkillReplacer.grandSlam = Modules.Skills.CreateSkillDef<HedgehogUtils.Forms.SkillDefs.RequiresFormSkillDef>(grandSlam);
            SuperSkillReplacer.grandSlam.requiredForm = HedgehogUtils.Forms.SuperForm.SuperFormDef.superFormDef;

            Log.Message("Making Super Sonic: All Skills");

            //NetworkStateMachine network = bodyPrefab.GetComponent<NetworkStateMachine>();
            //Helpers.Append(ref network.stateMachines, new List<EntityStateMachine> { superSonicState });
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ScepterSkill(SkillDefInfo boost)
        {
            Log.Message("Sonic Scepter skill started");
            boost.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SCEPTER_UTILITY_BOOST_NAME";
            boost.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SCEPTER_UTILITY_BOOST_NAME";
            boost.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SCEPTER_UTILITY_BOOST_DESCRIPTION";
            boost.activationState = new EntityStates.SerializableEntityStateType(typeof(NewScepterBoost));
            boost.skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texScepterBoostIcon");

            HedgehogUtils.Boost.SkillDefs.BoostSkillDef skillDef = Skills.CreateSkillDef<HedgehogUtils.Boost.SkillDefs.BoostSkillDef>(boost);
            skillDef.boostIdleState = new EntityStates.SerializableEntityStateType(typeof(BoostIdle));
            skillDef.brakeState = new EntityStates.SerializableEntityStateType(typeof(SonicBrake));
            skillDef.boostHUDColor = new Color(0, 0.9f, 1, 1);

            Log.Message("Sonic Scepter skill created? " +
                      (ItemBase<AncientScepterItem>.instance.RegisterScepterSkill(skillDef, "SonicTheHedgehog",
                          boostSkillDef)).ToString());

            boost.skillName = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SUPER_SCEPTER_UTILITY_BOOST_NAME";
            boost.skillNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SUPER_SCEPTER_UTILITY_BOOST_NAME";
            boost.skillDescriptionToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +"_SONIC_THE_HEDGEHOG_BODY_SUPER_SCEPTER_UTILITY_BOOST_DESCRIPTION";
            boost.activationState = new EntityStates.SerializableEntityStateType(typeof(NewScepterSuperBoost));
            boost.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperScepterBoostIcon");

            HedgehogUtils.Boost.SkillDefs.RequiresFormBoostSkillDef superSkillDef = Skills.CreateSkillDef<HedgehogUtils.Boost.SkillDefs.RequiresFormBoostSkillDef>(boost);
            superSkillDef.requiredForm = HedgehogUtils.Forms.SuperForm.SuperFormDef.superFormDef;
            superSkillDef.boostIdleState = new EntityStates.SerializableEntityStateType(typeof(BoostIdle));
            superSkillDef.brakeState = new EntityStates.SerializableEntityStateType(typeof(SonicBrake));
            superSkillDef.boostHUDColor = new Color(1f, 0.9f, 0, 1);

            Log.Message("Super Sonic Scepter skill created? " +
                      (ItemBase<AncientScepterItem>.instance.RegisterScepterSkill(superSkillDef, "SonicTheHedgehog",
                          SuperSkillReplacer.boost)).ToString());
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void EmoteSupport()
        {
            var skele = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SonicEmoteSupport.prefab");
            CustomEmotesAPI.ImportArmature(bodyPrefab, skele);
        }

        public override void InitializeSkins()
        {
            RoR2.UI.MainMenu.MainMenuController.OnMainMenuInitialised += UniqueSkinEffect.Bake;

            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ModelSkinController skinController2 = displayPrefab.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin

            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texDefaultSkinIcon"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            defaultSkin.meshReplacements = Modules.Skins.GetMeshReplacementsFromObject(defaultRendererinfos,
                "SonicMesh");
            //    "meshHenryGun",
            //    "meshHenry");
            /*defaultSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("MetalSonicLight"),
                    shouldActivate = false,
                }
            };*/

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);

            #region Super Form
            CharacterModel.RendererInfo[] defaultSkinSuperRenderer = ArrayUtils.Clone(defaultRendererinfos);
            defaultSkinSuperRenderer[0].defaultMaterial = Materials.CreateHopooMaterial("matSuperSonic").Specular(0.3f);
            Mesh[] defaultSkinSuperMeshes = new Mesh[]{ Assets.superSonicMesh };
            RenderReplacements defaultSkinSuper = new RenderReplacements
            {
                rendererInfo = defaultSkinSuperRenderer,
                mesh = defaultSkinSuperMeshes
            };
            Forms.AddSkinForForm(defaultSkin.nameToken,
                defaultSkinSuper,
                ref SuperFormDef.superFormDef);
            #endregion

            #endregion

            //uncomment this when you have a mastery skin

            #region MasterySkin
            SkinDefParams metalSkinDefParams = ScriptableObject.CreateInstance<SkinDefParams>();
            metalSkinDefParams.projectileGhostReplacements = new[] {
                new SkinDefParams.ProjectileGhostReplacement { projectilePrefab = Projectiles.superMeleePunchProjectilePrefab,
                    ghostReplacementAddress = Projectiles.superMetalMeleePunchProjectileGhost },
                new SkinDefParams.ProjectileGhostReplacement { projectilePrefab = Projectiles.superMeleeKickProjectilePrefab,
                    ghostReplacementAddress = Projectiles.superMetalMeleeKickProjectileGhost }};
            AssetAsyncReferenceManager<Material>.LoadAsset(SkinAddressables.metalMaterial).Completed += (x) =>
            { x.Result.SetHopooMaterial().MetalFresnel().Specular(0.4f, 4f); };

            metalSkinDefParams.rendererInfos = ArrayUtils.Clone(defaultRendererinfos);
            metalSkinDefParams.meshReplacements = Modules.Skins.GetParamMeshReplacementsFromObject(defaultRendererinfos, "MetalSonicMesh");
            // metalSkinDefParams.meshReplacements = new SkinDefParams.MeshReplacement[]
            // { new SkinDefParams.MeshReplacement { meshAddress = SkinAddressables.metalMesh, renderer = defaultRendererinfos[0].renderer }};
            metalSkinDefParams.rendererInfos[0].defaultMaterialAddress = SkinAddressables.metalMaterial;
            R2API.SkinDefParamsInfo metalSkinParamsInfo = new R2API.SkinDefParamsInfo
            {
                Name = SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME",
                NameToken = SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME",
                Icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texMetalSkinIcon"),
                UnlockableDef = masterySkinUnlockableDef,
                RootObject = prefabCharacterModel.gameObject,
                BaseSkins = new[] { defaultSkin },
                SkinDefParams = metalSkinDefParams
            };
            SkinDef metalSkin = R2API.Skins.CreateNewSkinDef(metalSkinParamsInfo);
            UniqueSkinEffect.AddFlyingSkin(SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME");
            skins.Add(metalSkin);

            #region Super Form
            AssetAsyncReferenceManager<Material>.LoadAsset(SkinAddressables.superMetalMaterial).Completed += (x) =>
            { x.Result.SetHopooMaterial().MetalFresnel().Specular(0.4f, 4f); };
            CharacterModel.RendererInfo[] masterySkinSuperRenderer = ArrayUtils.Clone(metalSkinDefParams.rendererInfos);
            masterySkinSuperRenderer[0].defaultMaterialAddress = SkinAddressables.superMetalMaterial;
            Mesh[] masterySkinSuperMeshes = new Mesh[] { metalSkinDefParams.meshReplacements[0].mesh };
            RenderReplacements masterySkinSuper = new RenderReplacements
            {
                rendererInfo = masterySkinSuperRenderer,
                mesh = masterySkinSuperMeshes
            };
            Forms.AddSkinForForm(metalSkin.nameToken,
                masterySkinSuper,
                ref SuperFormDef.superFormDef);
            UniqueSkinEffect.AddSuperGrandSlamMeshReplacement(SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME", SkinAddressables.superMetalGrandSlam);
            #endregion
            #endregion

            #region AnointedSkin EnemiesReturns
            if (SonicTheHedgehogPlugin.enemiesReturnsLoaded)
            {
                skins.Add(AnointedSkinEnemiesReturns(metalSkin));
            }
            #endregion
            skinController.skins = skins.ToArray();
            skinController2.skins = skins.ToArray();
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static SkinDef AnointedSkinEnemiesReturns(SkinDef skinToCopy)
        {
            SkinDef anointedSkin = EnemiesReturns.Enemies.Judgement.AnointedSkins.CreateAnointedSkin("SonicTheHedgehog", skinToCopy, true, Assets.mainAssetBundle.LoadAsset<Sprite>("texSpaceSuitSkinIcon"));
            anointedSkin.name = SONIC_THE_HEDGEHOG_PREFIX + "ENEMIES_RETURNS_JUDGEMENT_SKIN";
            anointedSkin.nameToken = "ENEMIES_RETURNS_JUDGEMENT_SKIN_ANOINTED_NAME";
            anointedSkin.icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSpaceSuitSkinIcon");
            return anointedSkin;
        }
    }

    public class SonicSkillDefs
    {
        public static SkillDefs.MeleeSkillDef primarySkillDef;

        public static SkillDef sonicBoomSkillDef;
        public static SkillDef parrySkillDef;
        public static SkillDef followUpSkillDef;

        public static HedgehogUtils.Boost.SkillDefs.BoostSkillDef boostSkillDef;

        public static SkillDef grandSlamSkillDef;

        public static SkillDef idwAttackSkillDef;

        public static void Initialize(SkillDefs.MeleeSkillDef primary, SkillDef sonicBoom, SkillDef parry, SkillDef followUp, HedgehogUtils.Boost.SkillDefs.BoostSkillDef boost, SkillDef grandSlam)
        {
            primarySkillDef = primary;
            sonicBoomSkillDef = sonicBoom;
            parrySkillDef = parry;
            followUpSkillDef = followUp;
            boostSkillDef = boost;
            grandSlamSkillDef = grandSlam;
        }
    }
}