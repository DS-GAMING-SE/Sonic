using BepInEx.Configuration;
using RoR2;
using RoR2.Achievements;
using RoR2.Stats;
using SonicTheHedgehog.Modules.Survivors;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SonicTheHedgehog.Modules.Achievements
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, 3)]
    public class SonicMountainShrinesAchievement : BaseAchievement
    {
        public const string identifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MOUNTAINSHRINESACHIEVEMENT";
        public const string unlockableIdentifier = SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MOUNTAINSHRINESUNLOCKABLE";
        public static ConfigEntry<int> sonicMountainShrinesConfig = SonicTheHedgehogPlugin.instance.Config.Bind<int>("Unlockables", "Sonic: Tearing Down Titans", 0, $"The number of mountain shrines tracked for the Tearing Down Titans achievement. Setting this number to {requiredMountainShrines} will unlock the achievement. Changing this setting can unlock or relock the achievement. Relocking the achievement may require restarting the game to make it possible to achieve again.");
        public static bool configUpdateBlocker;
        public const int requiredMountainShrines = 4;

        public static void Initialize()
        {
            sonicMountainShrinesConfig.SettingChanged += SonicTheHedgehogCharacter.UnlockCyloopConfig;
        }
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }

        public override void OnBodyRequirementMet()
        {
            TeleporterInteraction.onTeleporterChargedGlobal += AddMountainShrineComplete;
        }

        public override void OnBodyRequirementBroken()
        {
            TeleporterInteraction.onTeleporterChargedGlobal -= AddMountainShrineComplete;
        }

        private void AddMountainShrineComplete(TeleporterInteraction teleporter)
        {
            if (teleporter.shrineBonusStacks > 0 || teleporter.NetworkshowAccessCodesIndicator)
            {
                configUpdateBlocker = true;
                sonicMountainShrinesConfig.Value += TeleporterInteraction.instance.shrineBonusStacks + (TeleporterInteraction.instance.NetworkshowAccessCodesIndicator ? 1 : 0);
                Check();
                configUpdateBlocker = false;
            }
        }
        private void Check()
        {
            if (sonicMountainShrinesConfig.Value >= requiredMountainShrines)
            {
                base.Grant();
            }
        }

        public override float ProgressForAchievement()
        {
            return sonicMountainShrinesConfig.Value / requiredMountainShrines;
        }
    }
}