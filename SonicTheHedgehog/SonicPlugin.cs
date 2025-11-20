using BepInEx;
using SonicTheHedgehog.Modules.Survivors;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using SonicTheHedgehog.Modules;
using IL.RoR2.UI;
using UnityEngine;
using EmotesAPI;
using System.Runtime.CompilerServices;
using R2API;
using SonicTheHedgehog.Components;

using static BetterUI.ProcCoefficientCatalog;
using static BetterUI.Buffs;
using RiskOfOptions;

using SonicTheHedgehog.SkillStates;
using RiskOfOptions.Options;
using EntityStates;
using System.Security.Claims;
using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using R2API.Networking;
using System;
using HarmonyLib;
using LookingGlass.LookingGlassLanguage;
using LookingGlass.BuffDescriptions;
using LookingGlass.ItemStatsNameSpace;
using LoadingScreenFix;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace SonicTheHedgehog
{
    //[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.content_management", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.networking", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.unlockable", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.items", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.addressables", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.skins", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.sound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.director", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.damagetype", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(HedgehogUtils.HedgehogUtilsPlugin.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]

    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    //[BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(LookingGlass.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Nebby1999.LoadingScreenFix", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]

    public class SonicTheHedgehogPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.ds_gaming.SonicTheHedgehog";
        public const string MODNAME = "SonicTheHedgehog";
        public const string MODVERSION = "4.0.4";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "DS_GAMING";

        public static SonicTheHedgehogPlugin instance;
        public static bool emoteAPILoaded = false;
        //public static bool betterUILoaded = false;
        public static bool lookingGlassLoaded = false;
        public static bool riskOfOptionsLoaded = false;
        public static bool ancientScepterLoaded = false;
        public static bool loadingScreenFixLoaded = false;


        private void Awake()
        {
            instance = this;

            Log.Init(Logger);

            emoteAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
            Log.Message("Emote API exists? " + emoteAPILoaded);

            /*betterUILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.xoxfaby.BetterUI");
            Log.Message("Better UI exists? " + betterUILoaded);*/

            lookingGlassLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID);
            Log.Message("Looking Glass exists? " + lookingGlassLoaded);

            riskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            Log.Message("Risk of Options exists? " + riskOfOptionsLoaded);

            ancientScepterLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            Log.Message("Ancient Scepter exists? " + ancientScepterLoaded);

            loadingScreenFixLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Nebby1999.LoadingScreenFix");
            Log.Message("Loading Screen Fix exists? " + loadingScreenFixLoaded);

            Modules.Assets.Initialize(); // load assets and read config
            if (loadingScreenFixLoaded)
            {
                SonicLoadingScreenSprite();
            }
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules
            Modules.SuperFormSupport.Initialize();
            Modules.DamageTypes.Initialize();

            NetworkingAPI.RegisterMessageType<SonicParryHit>();
            //NetworkingAPI.RegisterMessageType<ScepterBoostDamage>();

            // survivor initialization
            new SonicTheHedgehogCharacter().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            /*if (betterUILoaded)
            {
                BetterUISetup();
            }*/

            if (riskOfOptionsLoaded)
            {
                RiskOfOptionsSetup();
            }

            //On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { }; //Magic multiplayer line, COMMENT OUT BEFORE RELEASE

            Hook();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            //On.RoR2.CharacterBody.RecalculateStats += WhereIsRecalcStatAPIAcceleration;

            On.RoR2.HealthComponent.TakeDamage += TakeDamage;

            On.RoR2.CharacterBody.OnBuffFirstStackGained += AddGrandSlamJuggleFloat;

            RecalculateStatsAPI.GetStatCoefficients += SonicRecalculateStats;

            On.RoR2.UserProfile.OnLogin += ConfigUnlocks;
            if (emoteAPILoaded)
            {
                EmoteSkeleton();
            }

            if (lookingGlassLoaded)
            {
                RoR2Application.onLoad += LookingGlassSetup;
            }

        }

        private void EmoteSkeleton()
        {
            //CustomEmotesAPI.CreateNameTokenSpritePair(DEVELOPER_PREFIX+"_SONIC_THE_HEDGEHOG_BODY_NAME", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSonicEmoteIcon"));
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "SonicTheHedgehog")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SonicEmoteSupport.prefab");
                        CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1f;
                    }
                }
            };
        }

        private static void SonicLoadingScreenSprite()
        {
            Sprite[] sprites = Modules.Assets.mainAssetBundle.LoadAssetWithSubAssets<Sprite>("SonicLoadingScreen"); // SonicLoadingScreen is a single png sprite sheet that is split into the 4 frames in Unity
            int[] durations = { 1, 1, 1, 1 };
            SimpleSpriteAnimation spriteAnimation = LoadingScreenFix.SimpleSpriteAnimationGenerator.CreateSpriteAnimation(sprites, durations, 8f);
            LoadingScreenFix.LoadingScreenFix.AddSpriteAnimation(spriteAnimation);
            Log.Message("Added Sonic loading screen sprite");
        }
        #region BetterUI
        /*private static void BetterUISetup()
        {
            ProcCoefficientInfo melee = new ProcCoefficientInfo
            {
                name = "Melee / Homing Attack",
                procCoefficient = StaticValues.meleeProcCoefficient
            };
            ProcCoefficientInfo superMeleeGhost = new ProcCoefficientInfo
            {
                name = "Projectile",
                procCoefficient = StaticValues.superMeleeExtraProcCoefficient
            };
            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_MELEE_NAME", melee);

            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PRIMARY_MELEE_NAME", new List<ProcCoefficientInfo>
            {
                melee, superMeleeGhost
            });

            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_SONIC_BOOM_NAME", new ProcCoefficientInfo
            {
                name = "Sonic Boom",
                procCoefficient = StaticValues.sonicBoomProcCoefficient
            });
            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_SONIC_BOOM_NAME", new ProcCoefficientInfo
            {
                name = "Sonic Boom",
                procCoefficient = StaticValues.sonicBoomProcCoefficient
            });
            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SECONDARY_IDW_ATTACK_NAME", new ProcCoefficientInfo
            {
                name = "Attack",
                procCoefficient = StaticValues.idwAttackProcCoefficient
            });

            ProcCoefficientInfo spin = new ProcCoefficientInfo
            {
                name = "Repeated Attack",
                procCoefficient = StaticValues.grandSlamSpinProcCoefficient
            };
            ProcCoefficientInfo kick = new ProcCoefficientInfo
            {
                name = "Final Attack",
                procCoefficient = StaticValues.grandSlamFinalProcCoefficient
            };
            ProcCoefficientInfo superGrandSlamAfterimage = new ProcCoefficientInfo
            {
                name = "Afterimages",
                procCoefficient = StaticValues.superGrandSlamDOTProcCoefficient
            };
            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_GRAND_SLAM_NAME", new List<ProcCoefficientInfo>
            {
                spin,kick
            });
            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_SPECIAL_GRAND_SLAM_NAME", new List<ProcCoefficientInfo>
            {
                spin,kick,superGrandSlamAfterimage
            });

            RegisterBuffInfo(Buffs.boostBuff, "Sonic Boost", $"+{StaticValues.boostArmor} Armor. If health is above 90%, +{StaticValues.powerBoostListedSpeedCoefficient*100}% movement speed. Otherwise, +{StaticValues.boostListedSpeedCoefficient*100}% movement speed");
            RegisterBuffInfo(Buffs.ballBuff, "Sonic Ball", $"+{StaticValues.ballArmor} Armor.");
            RegisterBuffInfo(Buffs.superSonicBuff, "Super Sonic", $"Upgrades all of your skills. +{100f * StaticValues.superSonicBaseDamage}% Damage. +{100f * StaticValues.superSonicAttackSpeed}% Attack speed. +{100f * StaticValues.superSonicMovementSpeed}% Base movement speed. Complete invincibility and flight.");
            RegisterBuffInfo(Buffs.parryBuff, "Sonic Parry", $"+{StaticValues.parryAttackSpeedBuff*100}% Attack speed. +{StaticValues.parryMovementSpeedBuff*100}% Movement speed.");
            RegisterBuffInfo(Buffs.superParryDebuff, "Super Sonic Parry Debuff", $"-{StaticValues.superParryArmorDebuff * 100} Armor. -{(1/StaticValues.superParryAttackSpeedDebuff) * 100}% Attack speed. -{(1 / StaticValues.superParryMovementSpeedDebuff) * 100}% Movement speed.");
        }*/
        #endregion
        public static void LookingGlassSetup()
        {
            if (Language.languagesByName.TryGetValue("en", out Language language))
            {
                RegisterLookingGlassBuff(language, Buffs.boostBuff, "Sonic Boost", $"Gain <style=cIsUtility>+{StaticValues.boostArmor} armor</style>. If <style=cIsDamage>health</style> is above <style=cIsDamage>90%</style>, gain <style=cIsUtility>+{StaticValues.powerBoostListedSpeedCoefficient * 100}% movement speed</style>. Otherwise, gain <style=cIsUtility>+{StaticValues.boostListedSpeedCoefficient * 100}% movement speed</style>.");
                RegisterLookingGlassBuff(language, Buffs.superBoostBuff, "Super Sonic Boost", $"Gain <style=cIsUtility>+{StaticValues.superBoostListedSpeedCoefficient * 100}% movement speed</style>.");
                RegisterLookingGlassBuff(language, Buffs.ballBuff, "Sonic Ball", $"Gain <style=cIsUtility>+{StaticValues.ballArmor} armor</style>.");
                RegisterLookingGlassBuff(language, Buffs.parryBuff, "Sonic Parry", $"Gain <style=cIsUtility>+{StaticValues.parryAttackSpeedBuff * 100}% attack speed</style> and <style=cIsUtility>+{StaticValues.parryMovementSpeedBuff * 100}% movement speed</style>.");
                RegisterLookingGlassBuff(language, Buffs.superParryDebuff, "Super Sonic Parry Debuff", $"Reduces <style=cIsUtility>armor</style> by {StaticValues.superParryArmorDebuff}, reduces <style=cIsUtility>attack speed and movement speed</style> by {(1 / StaticValues.superParryAttackSpeedDebuff) * 100}%.");
                RegisterLookingGlassBuff(language, Buffs.sonicBoomDebuff, "Sonic Boom Debuff", $"Reduces <style=cIsUtility>armor</style> by {StaticValues.sonicBoomDebuffArmorReduction}.");
                RegisterLookingGlassBuff(language, Buffs.crossSlashDebuff, "Sonic Cross Slash Debuff", $"Reduces <style=cIsUtility>armor</style> by {StaticValues.superSonicBoomDebuffArmorReduction}.");
            }
        }

        private static void RegisterLookingGlassBuff(Language language, BuffDef buff, string name, string description) // There's a method just like this in lookingglass but I can't access it due to protection level. I might be missing something 
        {
            LookingGlassLanguageAPI.SetupToken(language, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(language, $"DESCRIPTION_{buff.name}", description);
        }

        private static void RiskOfOptionsSetup()
        {
            Sprite icon = (Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSonicIcon"));
            ModSettingsManager.SetModIcon(icon);
            float minLocation = -500;
            float maxLocation = 500;
            ModSettingsManager.AddOption(new SliderOption(Modules.Config.BoostMeterLocationX(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = minLocation, max = maxLocation, formatString = "{0:0}" }));
            ModSettingsManager.AddOption(new SliderOption(Modules.Config.BoostMeterLocationY(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = minLocation, max = maxLocation, formatString = "{0:0}" }));
            
            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.KeyPressHomingAttack()));
            
            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.ForceUnlockParry()));

            Modules.Config.ForceUnlockParry().SettingChanged += SonicTheHedgehogCharacter.UnlockParryConfig;

            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.ForceUnlockMastery()));

            Modules.Config.ForceUnlockMastery().SettingChanged += SonicTheHedgehogCharacter.UnlockMasteryConfig;

            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.EnableLogs()));
        }

        private void SonicRecalculateStats(CharacterBody self, RecalculateStatsAPI.StatHookEventArgs stats)
        {
            if (self)
            {
                if (self.HasBuff(Buffs.boostBuff))
                {
                    //stats.baseMoveSpeedAdd += PowerBoostLogic.ShouldPowerBoost(self) ? StaticValues.powerBoostSpeedFlatCoefficient : StaticValues.boostSpeedFlatCoefficient;
                    //stats.moveSpeedMultAdd += PowerBoostLogic.ShouldPowerBoost(self) ? StaticValues.powerBoostSpeedCoefficient : StaticValues.boostSpeedCoefficient;
                    HedgehogUtils.Boost.BoostLogic.BoostStats(self, stats, PowerBoostLogic.ShouldPowerBoost(self) ? StaticValues.powerBoostListedSpeedCoefficient : StaticValues.boostListedSpeedCoefficient);
                    stats.armorAdd += StaticValues.boostArmor;
                }

                if (self.HasBuff(Buffs.superBoostBuff))
                {
                    HedgehogUtils.Boost.BoostLogic.BoostStats(self, stats, StaticValues.superBoostListedSpeedCoefficient);
                    stats.armorAdd += StaticValues.boostArmor;
                }

                if (self.HasBuff(Buffs.ballBuff))
                {
                    stats.armorAdd += StaticValues.ballArmor;
                }

                if (self.HasBuff(Buffs.parryBuff))
                {
                    stats.attackSpeedMultAdd += StaticValues.parryAttackSpeedBuff;
                    stats.moveSpeedMultAdd += StaticValues.parryMovementSpeedBuff;
                }

                if (self.HasBuff(Buffs.superParryDebuff))
                {
                    stats.baseMoveSpeedAdd -= self.baseMoveSpeed / StaticValues.superParryMovementSpeedDebuff;
                    stats.armorAdd -= StaticValues.superParryArmorDebuff;
                    stats.baseAttackSpeedAdd -= self.baseAttackSpeed / StaticValues.superParryAttackSpeedDebuff;
                }
                if (self.HasBuff(Buffs.grandSlamJuggleDebuff))
                {
                    stats.moveSpeedReductionMultAdd += StaticValues.grandSlamJuggleSpeedReductionMult;
                }

                if (self.HasBuff(Buffs.sonicBoomDebuff))
                {
                    stats.armorAdd -= StaticValues.sonicBoomDebuffArmorReduction * self.GetBuffCount(Buffs.sonicBoomDebuff);
                }
                if (self.HasBuff(Buffs.crossSlashDebuff))
                {
                    stats.armorAdd -= StaticValues.superSonicBoomDebuffArmorReduction * self.GetBuffCount(Buffs.crossSlashDebuff);
                }
            }
        }
        /*private void WhereIsRecalcStatAPIAcceleration(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self.HasBuff(Modules.Buffs.boostBuff))
            {
                self.acceleration *= 6f;
            }
        }*/

        // GEARBOX THANK YOU! SOTS 3 added JitterBoneBlacklist component which fixes this problem for me
        // This is so jank and doesn't even work consistently anymore because of skins but idk what else to do to stop jitter bones from being on Sonic
        /*private void IHateJitterBones(On.RoR2.JitterBones.orig_Start orig, JitterBones self)
        {
            if (self.skinnedMeshRenderer && self.skinnedMeshRenderer.name == "SonicMesh")
            {
                UnityEngine.Object.Destroy(self);
            }
            orig(self);
        }*/

        private void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damage)
        {
            if (self && NetworkServer.active)
            {
                EntityStateMachine stateMachine = EntityStateMachine.FindByCustomName(self.gameObject, "Body");
                if (stateMachine)
                {
                    EntityState state = stateMachine.state;
                    NetworkIdentity network = self.gameObject.GetComponent<NetworkIdentity>();
                    if (typeof(Parry).IsAssignableFrom(state.GetType()) && network)
                    {
                        ((Parry)state).OnTakeDamage(damage);
                        new SonicParryHit(network.netId, damage).Send(NetworkDestination.Clients);
                    }
                }
            }
            orig(self, damage);
            if (damage.damageType.HasModdedDamageType(DamageTypes.grandSlamJuggle) && NetworkServer.active
                && self
                && self.body
                && !self.body.bodyFlags.HasFlag(CharacterBody.BodyFlags.IgnoreKnockup))
            {
                self.body.AddTimedBuff(Buffs.grandSlamJuggleDebuff, 1, 1);
            }
        }

        private void AddGrandSlamJuggleFloat(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buff)
        {
            orig(self, buff);
            if (self)
            {
                if (buff && buff == Buffs.grandSlamJuggleDebuff)
                {
                    GrandSlamJuggleFloat juggleFloat = self.GetComponent<GrandSlamJuggleFloat>();
                    if (!juggleFloat)
                    {
                        self.gameObject.AddComponent<GrandSlamJuggleFloat>();
                    }
                }
            }
        }


        private void ConfigUnlocks(On.RoR2.UserProfile.orig_OnLogin orig, UserProfile self)
        {
            orig(self);
            if (!self.HasAchievement(DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE") && Modules.Config.ForceUnlockParry().Value)
            {
                self.AddAchievement(DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE", true);
            }
            if (!self.HasAchievement(DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE") && Modules.Config.ForceUnlockMastery().Value)
            {
                self.AddAchievement(DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE", true);
            }
        }
    }
}