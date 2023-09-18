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

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace SonicTheHedgehog
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI"
    })]

    public class SonicTheHedgehogPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.ds_gaming.SonicTheHedgehog";
        public const string MODNAME = "SonicTheHedgehog";
        public const string MODVERSION = "1.0.2";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "DS_GAMING";

        public static SonicTheHedgehogPlugin instance;
        public static bool emoteAPILoaded = false;
        public static bool betterUILoaded = false;
        public static bool riskOfOptionsLoaded = false;

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            // survivor initialization
            new SonicTheHedgehogCharacter().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            emoteAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
            Log.Message("Emote API exists? "+emoteAPILoaded);

            betterUILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.xoxfaby.BetterUI");
            Log.Message("Better UI exists? " + betterUILoaded);

            riskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            Log.Message("Risk of Options exists? " + riskOfOptionsLoaded);

            if (betterUILoaded)
            {
                BetterUISetup();
            }

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
            On.RoR2.CharacterBody.RecalculateStats += WhereIsRecalcStatAPIAcceleration;
            On.RoR2.GenericSkill.CanApplyAmmoPack += CanApplyAmmoPackToBoost;
            On.RoR2.GenericSkill.ApplyAmmoPack += ApplyAmmoPackToBoost;
            On.RoR2.JitterBones.Start += IHateJitterBones;
            RecalculateStatsAPI.GetStatCoefficients += SonicRecalculateStats;
            if (emoteAPILoaded)
            {
                EmoteSkeleton();
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

        private static void BetterUISetup()
        {
            ProcCoefficientInfo melee = new ProcCoefficientInfo
            {
                name = "Melee / Homing Attack",
                procCoefficient = StaticValues.meleeProcCoefficient
            };
            /*ProcCoefficientInfo homing = new ProcCoefficientInfo
            {
                name = "Homing Attack",
                procCoefficient = StaticValues.homingAttackProcCoefficient
            };
            */
            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_MELEE_NAME", melee); /*new List<ProcCoefficientInfo>
            {
                melee//,homing
            });*/
            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SECONDARY_SONIC_BOOM_NAME", new ProcCoefficientInfo
            {
                name = "Sonic Boom",
                procCoefficient = StaticValues.sonicBoomProcCoefficient
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
            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SPECIAL_GRAND_SLAM_NAME", new List<ProcCoefficientInfo>
            {
                spin,kick
            });

            RegisterBuffInfo(Buffs.boostBuff, "Sonic Boost", $"+{StaticValues.boostArmor} Armor. If health is above 90%, +{StaticValues.powerBoostSpeedCoefficient*100}% movement speed. Otherwise, +{StaticValues.boostSpeedCoefficient*100}% movement speed");
            RegisterBuffInfo(Buffs.ballBuff, "Sonic Ball", $"+{StaticValues.ballArmor} Armor.");
            RegisterBuffInfo(Buffs.superSonicBuff, "Super Sonic", $"Upgrades all of your skills. +{100f * StaticValues.superSonicBaseDamage}% Damage. +{100f * StaticValues.superSonicAttackSpeed}% Attack speed. +{100f * StaticValues.superSonicMovementSpeed}% Movement speed. Complete invincibility and flight.");
        }

        private static void RiskOfOptionsSetup()
        {
            Sprite icon = (Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSonicIcon"));
            ModSettingsManager.SetModIcon(icon);
            float minLocation = -500;
            float maxLocation = 500;
            ModSettingsManager.AddOption(new SliderOption(Modules.Config.BoostMeterLocationX(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = minLocation, max = maxLocation, formatString = "{0:0}" }));
            ModSettingsManager.AddOption(new SliderOption(Modules.Config.BoostMeterLocationY(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = minLocation, max = maxLocation, formatString = "{0:0}" }));
        }

        private void SonicRecalculateStats(CharacterBody self, RecalculateStatsAPI.StatHookEventArgs stats)
        {
            if (self)
            {
                if (self.HasBuff(Modules.Buffs.boostBuff))
                {
                    if (!self.HasBuff(Modules.Buffs.superSonicBuff))
                    {
                        stats.moveSpeedMultAdd += self.healthComponent.health / self.healthComponent.fullHealth >= 0.9f ? StaticValues.powerBoostSpeedCoefficient : StaticValues.boostSpeedCoefficient;
                        stats.armorAdd += StaticValues.boostArmor;
                    }
                    else
                    {
                        stats.moveSpeedMultAdd += StaticValues.superBoostSpeedCoefficient;
                    }
                }

                if (self.HasBuff(Modules.Buffs.superSonicBuff))
                {
                    stats.moveSpeedMultAdd += StaticValues.superSonicMovementSpeed;
                    stats.attackSpeedMultAdd += StaticValues.superSonicAttackSpeed;
                    stats.damageMultAdd += StaticValues.superSonicBaseDamage;
                    stats.jumpPowerMultAdd += StaticValues.superSonicJumpHeight;
                }

                if (self.HasBuff(Modules.Buffs.ballBuff))
                {
                    stats.armorAdd += StaticValues.ballArmor;
                }

                Components.BoostLogic boost = self.GetComponent<Components.BoostLogic>();
                if (boost)
                {
                    boost.CalculateBoostVariables();
                }

                MomentumPassive momentum = self.GetComponent<MomentumPassive>();
                if (momentum && momentum.momentumEquipped)
                {
                    if (momentum.momentum >= 0)
                    {
                        stats.moveSpeedMultAdd += (momentum.momentum * MomentumPassive.speedMultiplier);
                    }
                    else
                    {
                        stats.moveSpeedReductionMultAdd += Mathf.Abs(momentum.momentum) * (MomentumPassive.speedMultiplier/3);
                    }
                }
            }
        }
        private void WhereIsRecalcStatAPIAcceleration(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self.HasBuff(Modules.Buffs.superSonicBuff))
            {
                self.acceleration *= 5;
            }

            if (self.HasBuff(Modules.Buffs.boostBuff))
            {
                self.acceleration *= 6f;
            }
        }

        private bool CanApplyAmmoPackToBoost(On.RoR2.GenericSkill.orig_CanApplyAmmoPack orig, GenericSkill self)
        {
            if (self.activationState.stateType == typeof(Boost))
            {
                BoostLogic boost = self.characterBody.GetComponent<BoostLogic>();
                return boost && boost.boostMeter < boost.maxBoostMeter; 
            }
            return orig(self);
        }
        private void ApplyAmmoPackToBoost(On.RoR2.GenericSkill.orig_ApplyAmmoPack orig, GenericSkill self)
        {
            orig(self);
            if (self.activationState.stateType == typeof(Boost))
            {
                BoostLogic boost = self.characterBody.GetComponent<BoostLogic>();
                if (boost)
                {
                    boost.AddBoost(BoostLogic.boostRegenPerBandolier);
                }
            }
        }

        private void IHateJitterBones(On.RoR2.JitterBones.orig_Start orig, JitterBones self)
        {
            if (self.skinnedMeshRenderer && self.skinnedMeshRenderer.name == "SonicMesh")
            {
                Object.Destroy(self);
            }
            orig(self);
        }
    }
}