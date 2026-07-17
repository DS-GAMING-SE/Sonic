using BepInEx;
using EmotesAPI;
using EntityStates;
using HarmonyLib;
using IL.RoR2.UI;
using LoadingScreenFix;
using LookingGlass.BuffDescriptions;
using LookingGlass.ItemStatsNameSpace;
using LookingGlass.LookingGlassLanguage;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Achievements;
using SonicTheHedgehog.Modules.Survivors;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Claims;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;
using static BetterUI.Buffs;
using static BetterUI.ProcCoefficientCatalog;

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
    [BepInDependency(EnemiesReturns.EnemiesReturnsPlugin.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]

    public class SonicTheHedgehogPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.ds_gaming.SonicTheHedgehog";
        public const string MODNAME = "SonicTheHedgehog";
        public const string MODVERSION = "5.0.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "DS_GAMING";

        public static SonicTheHedgehogPlugin instance;
        public static bool emoteAPILoaded = false;
        //public static bool betterUILoaded = false;
        public static bool lookingGlassLoaded = false;
        public static bool riskOfOptionsLoaded = false;
        public static bool ancientScepterLoaded = false;
        public static bool loadingScreenFixLoaded = false;
        public static bool enemiesReturnsLoaded = false;


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

            enemiesReturnsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(EnemiesReturns.EnemiesReturnsPlugin.GUID);
            Log.Message("Enemies Returns exists? " + enemiesReturnsLoaded);

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

            On.RoR2.HealthComponent.TakeDamageProcess += TakeDamage;

            On.RoR2.CharacterMotor.ModifyGravity += GrandSlamJuggleAndCyloopFloat;

            RecalculateStatsAPI.GetStatCoefficients += SonicRecalculateStats;

            On.RoR2.UserProfile.OnLogin += ConfigUnlocks;

            On.RoR2.CharacterBody.OnBuffFirstStackGained += REPLACETHISWITHEVENTINHEDGEHOGUTILS;

            if (lookingGlassLoaded)
            {
                RoR2Application.onLoad += LookingGlassSetup;
            }
        }

        private static void SonicLoadingScreenSprite()
        {
            Sprite[] sprites = Modules.Assets.mainAssetBundle.LoadAssetWithSubAssets<Sprite>("SonicLoadingScreen"); // SonicLoadingScreen is a single png sprite sheet that is split into the 4 frames in Unity
            int[] durations = { 1, 1, 1, 1 };
            SimpleSpriteAnimation spriteAnimation = LoadingScreenFix.SimpleSpriteAnimationGenerator.CreateSpriteAnimation(sprites, durations, 8f);
            LoadingScreenFix.LoadingScreenFix.AddSpriteAnimation(spriteAnimation);
            Log.Message("Added Sonic loading screen sprite");
        }
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
                RegisterLookingGlassBuff(language, Buffs.cyloopDebuff, "Sonic Cyloop Debuff", $"Disables <style=cIsUtility>movement</style> and increases <style=cIsUtility>skill</style> damage taken by {StaticValues.cyloopConstrictSkillDamageMultiplier * 100f}%.");
                RegisterLookingGlassBuff(language, Buffs.superCyloopDebuff, "Sonic Super Cyloop Debuff", $"Disables <style=cIsUtility>movement</style> and increases <style=cIsUtility>skill</style> damage taken by {StaticValues.cyloopConstrictSkillDamageMultiplier * 100f}%.");
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
            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.KeyPressHomingAttack()));
            
            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.ForceUnlockParry()));
            Modules.Config.ForceUnlockParry().SettingChanged += SonicTheHedgehogCharacter.UnlockParryConfig;

            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.ForceUnlockMastery()));
            Modules.Config.ForceUnlockMastery().SettingChanged += SonicTheHedgehogCharacter.UnlockMasteryConfig;

            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.ForceUnlockGrandMastery()));
            Modules.Config.ForceUnlockGrandMastery().SettingChanged += SonicTheHedgehogCharacter.UnlockGrandMasteryConfig;

            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.ForceUnlockMeridian()));
            Modules.Config.ForceUnlockMeridian().SettingChanged += SonicTheHedgehogCharacter.UnlockMeridianConfig;

            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.ForceUnlockDecompile()));
            Modules.Config.ForceUnlockDecompile().SettingChanged += SonicTheHedgehogCharacter.UnlockDecompileConfig;

            ModSettingsManager.AddOption(new CheckBoxOption(Modules.Config.ForceUnlockPurge()));
            Modules.Config.ForceUnlockPurge().SettingChanged += SonicTheHedgehogCharacter.UnlockPurgeConfig;

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
                if (Buffs.HasCyloopDebuff(self))
                {
                    stats.moveSpeedRootCount += 1;
                }
            }
        }

        private void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damage)
        {
            if (self && NetworkServer.active) // Move parrying to an IOnIncomingDamageServerReceiver?
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
                if (Buffs.HasCyloopDebuff(self.body))
                {
                    damage.damage *= (1 + StaticValues.cyloopConstrictSkillDamageMultiplier);
                }
            }
            orig(self, damage);
            if (NetworkServer.active && !damage.rejected && self && self.body)
            {
                if (damage.damageType.HasModdedDamageType(DamageTypes.grandSlamJuggle)
                && !self.body.bodyFlags.HasFlag(CharacterBody.BodyFlags.IgnoreKnockup)
                && !self.body.bodyFlags.HasFlag(CharacterBody.BodyFlags.Unmovable))
                {
                    self.body.AddTimedBuff(Buffs.grandSlamJuggleDebuff, 0.6f, 1);
                }
                if (damage.damageType.HasModdedDamageType(DamageTypes.cyloop))
                {
                    self.body.AddTimedBuff(Buffs.cyloopDebuff, StaticValues.cyloopConstrictDuration);
                }
                if (damage.damageType.HasModdedDamageType(DamageTypes.superCyloop))
                {
                    self.body.AddTimedBuff(Buffs.superCyloopDebuff, StaticValues.superCyloopConstrictDuration);
                }
            }
        }
        private void REPLACETHISWITHEVENTINHEDGEHOGUTILS(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buff)
        {
            orig(self, buff);
            if (buff == HedgehogUtils.Buffs.launchedBuff)
            {
                self.ClearTimedBuffs(Buffs.cyloopDebuff);
                self.ClearTimedBuffs(Buffs.superCyloopDebuff);
            }
        }
        // could also mess with Icharactergravityparameters to get rid of gravity entirely
        private void GrandSlamJuggleAndCyloopFloat(On.RoR2.CharacterMotor.orig_ModifyGravity orig, CharacterMotor self, ref float verticalVelocity, ref float gravity, float deltaTime)
        {
            if (self.body && (self.body.HasBuff(Buffs.grandSlamJuggleDebuff) || Buffs.HasCyloopDebuff(self.body)))
            {
                if (verticalVelocity > 0)
                {
                    gravity *= 1.5f;
                }
                else if (verticalVelocity < 3f)
                {
                    gravity = 0f;
                }
                else
                {
                    gravity *= 0.1f;
                }
                return;
            } // me when I don't call orig like a VILLAIN
            orig(self, ref verticalVelocity, ref gravity, deltaTime);
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
            if (!self.HasAchievement(SonicGrandMasteryAchievement.identifier) && Modules.Config.ForceUnlockGrandMastery().Value)
            {
                self.AddAchievement(SonicGrandMasteryAchievement.identifier, true);
            }
            if (!self.HasAchievement(SonicMeridianEventTriggerAchievement.identifier) && Modules.Config.ForceUnlockMeridian().Value)
            {
                self.AddAchievement(SonicMeridianEventTriggerAchievement.identifier, true);
            }
            if (!self.HasAchievement(SonicDecompileAchievement.identifier) && Modules.Config.ForceUnlockDecompile().Value)
            {
                self.AddAchievement(SonicDecompileAchievement.identifier, true);
            }
            if (!self.HasAchievement(SonicPurgeAchievement.identifier) && Modules.Config.ForceUnlockPurge().Value)
            {
                self.AddAchievement(SonicPurgeAchievement.identifier, true);
            }
        }
    }
}