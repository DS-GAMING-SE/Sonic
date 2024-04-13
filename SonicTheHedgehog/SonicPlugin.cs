﻿using BepInEx;
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
using UnityEngine.SceneManagement;
using System;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace SonicTheHedgehog
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
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
        public const string MODVERSION = "2.0.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "DS_GAMING";

        public static SonicTheHedgehogPlugin instance;
        public static bool emoteAPILoaded = false;
        public static bool betterUILoaded = false;
        public static bool riskOfOptionsLoaded = false;
        public static bool ancientScepterLoaded = false;

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);

            emoteAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
            Log.Message("Emote API exists? " + emoteAPILoaded);

            betterUILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.xoxfaby.BetterUI");
            Log.Message("Better UI exists? " + betterUILoaded);

            riskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            Log.Message("Risk of Options exists? " + riskOfOptionsLoaded);

            ancientScepterLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            Log.Message("Ancient Scepter exists? " + ancientScepterLoaded);

            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.Items.RegisterItems(); // silly Items thingy.
            Modules.Forms.Forms.Initialize();
            SuperSonicHandler.Initialize();
            ChaosEmeraldInteractable.Initialize();
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            NetworkingAPI.RegisterMessageType<SonicParryHit>();
            NetworkingAPI.RegisterMessageType<ScepterBoostDamage>();
            NetworkingAPI.RegisterMessageType<SuperSonicTransform>();

            // survivor initialization
            new SonicTheHedgehogCharacter().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

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
            //On.RoR2.GenericSkill.RunRecharge += RunRechargeBoost;

            On.RoR2.JitterBones.Start += IHateJitterBones;

            On.RoR2.HealthComponent.TakeDamage += TakeDamage;

            On.RoR2.Util.GetBestBodyName += SuperNamePrefix;

            RecalculateStatsAPI.GetStatCoefficients += SonicRecalculateStats;

            On.RoR2.UserProfile.OnLogin += ConfigUnlocks;
            if (emoteAPILoaded)
            {
                EmoteSkeleton();
            }

            On.RoR2.SceneDirector.Start += SceneDirectorOnStart;
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
            ProcCoefficientInfo superMeleeGhost = new ProcCoefficientInfo
            {
                name = "Projectile",
                procCoefficient = StaticValues.superMeleeExtraProcCoefficient
            };
            AddSkill(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_PRIMARY_MELEE_NAME", melee); /*new List<ProcCoefficientInfo>
            {
                melee//,homing
            });*/
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

            ModSettingsManager.AddOption(new KeyBindOption(Modules.Config.SuperTransformKey()));
        }

        private void SonicRecalculateStats(CharacterBody self, RecalculateStatsAPI.StatHookEventArgs stats)
        {
            if (self)
            {
                if (self.HasBuff(Buffs.boostBuff))
                {
                    if (!self.HasBuff(Buffs.superSonicBuff))
                    {
                        stats.baseMoveSpeedAdd += self.healthComponent.health / self.healthComponent.fullHealth >= 0.9f ? StaticValues.powerBoostSpeedFlatCoefficient : StaticValues.boostSpeedFlatCoefficient;
                        stats.moveSpeedMultAdd += self.healthComponent.health / self.healthComponent.fullHealth >= 0.9f ? StaticValues.powerBoostSpeedCoefficient : StaticValues.boostSpeedCoefficient;
                        stats.armorAdd += StaticValues.boostArmor;
                    }
                    else
                    {
                        stats.baseMoveSpeedAdd += StaticValues.superBoostSpeedFlatCoefficient;
                        stats.moveSpeedMultAdd += StaticValues.superBoostSpeedCoefficient;
                    }
                }

                if (self.HasBuff(Buffs.superSonicBuff))
                {
                    stats.baseMoveSpeedAdd += StaticValues.superSonicMovementSpeed * self.baseMoveSpeed;
                    stats.attackSpeedMultAdd += StaticValues.superSonicAttackSpeed;
                    stats.damageMultAdd += StaticValues.superSonicBaseDamage;
                    stats.jumpPowerMultAdd += StaticValues.superSonicJumpHeight;
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

                BoostLogic boost = self.GetComponent<Components.BoostLogic>();
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

                if (self.HasBuff(Buffs.superParryDebuff))
                {
                    stats.baseMoveSpeedAdd -= self.baseMoveSpeed / StaticValues.superParryMovementSpeedDebuff;
                    stats.armorAdd -= StaticValues.superParryArmorDebuff;
                    stats.baseAttackSpeedAdd -= self.baseAttackSpeed / StaticValues.superParryAttackSpeedDebuff;
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
            if (self.activationState.stateType == typeof(Boost) || self.activationState.stateType == typeof(ScepterBoost))
            {
                BoostLogic boost = self.characterBody.GetComponent<BoostLogic>();
                if (boost)
                {
                    return boost.boostMeter < boost.maxBoostMeter;
                } 
            }
            return orig(self);
        }
        private void ApplyAmmoPackToBoost(On.RoR2.GenericSkill.orig_ApplyAmmoPack orig, GenericSkill self)
        {
            orig(self);
            if (self.activationState.stateType == typeof(Boost) || self.activationState.stateType == typeof(ScepterBoost))
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
                UnityEngine.Object.Destroy(self);
            }
            orig(self);
        }

        private void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damage)
        {
            EntityStateMachine stateMachine = EntityStateMachine.FindByCustomName(self.gameObject, "Body");
            if (stateMachine)
            {
                EntityState state = stateMachine.state;
                NetworkIdentity network = self.gameObject.GetComponent<NetworkIdentity>();
                if (state.GetType() == typeof(Parry) && network)
                {
                    ((Parry)state).OnTakeDamage(damage);
                    new SonicParryHit(network.netId, damage).Send(NetworkDestination.Clients);
                }
            }
            if (self.body.HasBuff(Buffs.superSonicBuff))
            {
                damage.rejected = true;
                EffectManager.SpawnEffect(HealthComponent.AssetReferences.damageRejectedPrefab, new EffectData
                {
                    origin = damage.position
                }, true);
            }
            orig(self, damage);
        }

        private static string SuperNamePrefix(On.RoR2.Util.orig_GetBestBodyName orig, GameObject bodyObject)
        {
            if (bodyObject)
            {
                CharacterBody body = bodyObject.GetComponent<CharacterBody>();
                if (body)
                {
                    if (body.HasBuff(Buffs.superSonicBuff))
                    {
                        string text = orig(bodyObject);
                        text = Language.GetStringFormatted(DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PREFIX", new object[]
                        {
                            text
                        });
                        return text;
                    }
                }
            }
            return orig(bodyObject);
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


        private void SceneDirectorOnStart(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            if (!NetworkServer.active) return;

            SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();
            /*if (sceneName == "intro")
            {
                return;
            }

            if (sceneName == "title")
            {
                // TODO:: create prefab of super sonic floating in the air silly style.
                Vector3 vector = new Vector3(38, 23, 36);
            }*/

            if (!SuperSonicHandler.instance)
            {
                NetworkServer.Spawn(GameObject.Instantiate<GameObject>(SuperSonicHandler.handlerPrefab));
            }

            bool someoneIsSonic = false;
            foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
            {
                if (BodyCatalog.FindBodyIndex(player.master.bodyPrefab) == BodyCatalog.FindBodyIndex("SonicTheHedgehog"))
                {
                    someoneIsSonic = true;
                }
            }
            Debug.Log("Anyone playing Sonic? " + someoneIsSonic);
            if (!someoneIsSonic)
            {
                SuperSonicHandler.instance.SetEvents(false);
                return;
            }

            // Metamorphosis causes issues with emeralds spawning because character rerolls happen after emeralds spawn. Emeralds would only spawn the stage after you were Sonic
            bool metamorphosis = RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.randomSurvivorOnRespawnArtifactDef);
            Debug.Log("Metamorphosis? " + metamorphosis);
            if (metamorphosis)
            {
                SuperSonicHandler.instance.SetEvents(false);
                return;
            }

            SuperSonicHandler.instance.SetEvents(true);

            SuperSonicHandler.instance.ResetAvailable();

            SuperSonicHandler.instance.FilterOwnedEmeralds();

            if (SuperSonicHandler.available.Count > 0 && scene && scene.sceneType == SceneType.Stage && !scene.cachedName.Contains("moon") && !scene.cachedName.Contains("voidraid") && !scene.cachedName.Contains("voidstage")) 
            {
                int maxEmeralds = Run.instance is InfiniteTowerRun ? StaticValues.chaosEmeraldsMaxPerStageSimulacrum : StaticValues.chaosEmeraldsMaxPerStage;
                
                SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();

                spawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
                spawnCard.sendOverNetwork = true;

                spawnCard.prefab = ChaosEmeraldInteractable.prefabBase;

                for (int i = 0; i < maxEmeralds && i < SuperSonicHandler.available.Count; i++)
                {
                    DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, ChaosEmeraldInteractable.placementRule, Run.instance.stageRng));
                }
            }
        }
    }
}