using BepInEx.Configuration;
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

namespace SonicTheHedgehog.Modules.Survivors
{
    internal class SonicTheHedgehogCharacter : SurvivorBase
    {
        //used when building your character using the prefabs you set up in unity
        //don't upload to thunderstore without changing this
        public override string prefabBodyName => "SonicTheHedgehog";

        public const string SONIC_THE_HEDGEHOG_PREFIX = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => SONIC_THE_HEDGEHOG_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "SonicTheHedgehog",
            bodyNameToken = SONIC_THE_HEDGEHOG_PREFIX + "NAME",
            subtitleNameToken = SONIC_THE_HEDGEHOG_PREFIX + "SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texSonicIcon"),
            bodyColor = new Color(0.29f,0.34f,1f),

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
        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            bodyPrefab.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Death));
        }

        public override void InitializeUnlockables()
        {
            //uncomment this when you have a mastery skin. when you do, make sure you have an icon too
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>();
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

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = SonicTheHedgehogPlugin.DEVELOPER_PREFIX;

            bodyPrefab.AddComponent<Components.BoostLogic>();
            bodyPrefab.AddComponent<Components.MomentumPassive>();

            MakeSuperSonicStuff();

            bodyPrefab.AddComponent<Components.SuperSonicComponent>().superSonicMaterial = Materials.CreateHopooMaterial("matSuperSonic");

            On.RoR2.UI.HUD.Awake += CreateBoostMeterUI;

            #region Primary
            //Creates a skilldef for a typical primary 
            SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo(prefix + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_SLASH_NAME",
                                                                                      prefix + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_SLASH_DESCRIPTION",
                                                                                      Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMeleeIcon"),
                                                                                      new EntityStates.SerializableEntityStateType(typeof(SkillStates.SonicMelee)),
                                                                                      "Body",
                                                                                      false));


            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);
            #endregion

            #region Secondary
            SkillDef shootSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
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
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, shootSkillDef);
            #endregion

            #region Utility
            SkillDef rollSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
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
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, rollSkillDef);
            #endregion

            #region Special
            SkillDef bombSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_BOMB_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_BOMB_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_BOMB_DESCRIPTION",
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

            Modules.Skills.AddSpecialSkills(bodyPrefab, bombSkillDef);
            #endregion

            #region Special #2
            SkillDef superSonicSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_SUPER_TRANSFORMATION_NAME",
                skillNameToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_SUPER_TRANSFORMATION_NAME",
                skillDescriptionToken = prefix + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_SUPER_TRANSFORMATION_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBazookaOutIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SuperSonicTransformation)),
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
            SkillDef momentumPassiveDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
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
            
        }

        private void MakeSuperSonicStuff()
        {
            EntityStateMachine superSonicState = bodyPrefab.AddComponent<EntityStateMachine>();
            superSonicState.customName = "SonicForms";
            superSonicState.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.BaseSonic));


            bodyPrefab.AddComponent<Components.SuperSonicComponent>().superSonicMaterial = Materials.CreateHopooMaterial("matSuperSonic");

            //NetworkStateMachine network = bodyPrefab.GetComponent<NetworkStateMachine>();
            //Helpers.Append(ref network.stateMachines, new List<EntityStateMachine> { superSonicState });
        }

        internal static void CreateBoostMeterUI(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig.Invoke(self);
            BoostHUD boostHud = self.gameObject.AddComponent<BoostHUD>();
            GameObject boostUI = UnityEngine.Object.Instantiate<GameObject>(Assets.mainAssetBundle.LoadAsset<GameObject>("BoostMeter2"), self.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas"));
            RectTransform rectTransform=boostUI.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
            rectTransform.localPosition= Vector3.zero;
            rectTransform.anchoredPosition = new Vector2(90f, -50f);
            boostHud.boostMeter = boostUI;
            boostHud.meterBackground = boostUI.transform.Find("Background").gameObject.GetComponent<Image>();
            boostHud.meterFill = boostUI.transform.Find("Background/Fill").gameObject.GetComponent<Image>();
            boostHud.meterBackgroundOuter = boostUI.transform.Find("BackgroundOuter").gameObject.GetComponent<Image>();
            boostHud.meterFillOuter = boostUI.transform.Find("BackgroundOuter/FillOuter").gameObject.GetComponent<Image>();
            boostHud.meterBackgroundBackup = boostUI.transform.Find("BackgroundBackup").gameObject.GetComponent<RawImage>();
            boostHud.meterFillBackup = boostUI.transform.Find("BackgroundBackup/FillBackup").gameObject.GetComponent<RawImage>();
            boostHud.powerBoostParticle = boostUI.transform.Find("PowerParticles").gameObject.GetComponent<ParticleSystem>();
            boostHud.infiniteBackground = boostUI.transform.Find("InfiniteBackground").gameObject.GetComponent<Image>();
            boostHud.infiniteFill = boostUI.transform.Find("InfiniteBackground/InfiniteFill").gameObject.GetComponent<Image>();

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
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion
            
            //uncomment this when you have a mastery skin
            #region MasterySkin
            /*
            //creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(HenryPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                masterySkinUnlockableDef);

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                "meshHenrySwordAlt",
                null,//no gun mesh replacement. use same gun mesh
                "meshHenryAlt");

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos defaultMaterials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            masterySkin.rendererInfos[1].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            masterySkin.rendererInfos[2].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");

            //here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("GunModel"),
                    shouldActivate = false,
                }
            };
            //simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(masterySkin);
            */
            #endregion

            skinController.skins = skins.ToArray();
        }
    }
}