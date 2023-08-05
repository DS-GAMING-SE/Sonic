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

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace SonicTheHedgehog
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    //[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
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
        public const string MODVERSION = "0.2.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "DS_GAMING";

        public static SonicTheHedgehogPlugin instance;
        public static bool emoteAPILoaded = false;

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

            Hook();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
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

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            // a simple stat hook, adds armor after stats are recalculated
            if (self)
            {
                if (self.HasBuff(Modules.Buffs.boostBuff))
                {
                    self.acceleration *= 6f;
                    if (!self.HasBuff(Modules.Buffs.superSonicBuff))
                    {
                        self.moveSpeed *= self.healthComponent.health / self.healthComponent.fullHealth >= 0.9f ? StaticValues.powerBoostSpeedCoefficient : StaticValues.boostSpeedCoefficient;
                        self.armor += StaticValues.boostArmor;
                    }
                    else
                    {
                        self.moveSpeed *= StaticValues.superBoostSpeedCoefficient;
                    }
                }

                if (self.HasBuff(Modules.Buffs.superSonicBuff))
                {
                    self.moveSpeed *= StaticValues.superSonicMovementSpeed;
                    self.attackSpeed*=StaticValues.superSonicAttackSpeed;
                    self.damage *= StaticValues.superSonicBaseDamage;
                    self.jumpPower *= StaticValues.superSonicJumpHeight;
                    self.acceleration *= 5;
                }

                if (self.HasBuff(Modules.Buffs.ballBuff))
                {
                    self.armor += StaticValues.ballArmor;
                }

                Components.BoostLogic boost = self.GetComponent<Components.BoostLogic>();
                if (boost)
                {
                    boost.CalculateBoostVariables();
                }
            }
        }
    }
}