using BepInEx.Configuration;
using RiskOfOptions.Options;
using RiskOfOptions.Utils;
using UnityEngine;

namespace SonicTheHedgehog.Modules
{
    public static class Config
    {
        public static void ReadConfig()
        {

        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {

            return SonicTheHedgehogPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }
        public static ConfigEntry<float> BoostMeterLocationX()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<float>("Boost Meter", "X Location", 90f, "X Coordinate of the boost meter's location relative to the crosshair. Default is 90.");
        }

        public static ConfigEntry<float> BoostMeterLocationY()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<float>("Boost Meter", "Y Location", -50f, "Y Coordinate of the boost meter's location relative to the crosshair. Default is -50.");
        }

        public static ConfigEntry<bool> KeyPressHomingAttack()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<bool>("Controls", "Key-press Homing Attack", false, "Determines whether you need to press the primary skill key to use the homing attack. If false, you will also be able to activate a homing attack by pressing or holding the primary skill key. Default is false.");
        }

        public static ConfigEntry<bool> ForceUnlockParry()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<bool>("Unlockables", "Sonic: Spinning Upside Down", false, "Automatically unlock the achievement \"Sonic: Spinning Upside Down\". Turning this setting off will relock the achievement. Relocking the achievement may require restarting the game to make it possible to achieve again.");
        }

        public static ConfigEntry<bool> ForceUnlockMastery()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<bool>("Unlockables", "Sonic: Mastery", false, "Automatically unlock the achievement \"Sonic: Mastery\". Turning this setting off will relock the achievement. Relocking the achievement may require restarting the game to make it possible to achieve again.");
        }

        public static ConfigEntry<bool> EmeraldsWithoutSonic()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<bool>("Chaos Emeralds", "Spawn Emeralds Without Sonic", false, "Determines whether the Chaos Emeralds are allowed to spawn even if no one is playing as Sonic. Host's config takes priority. Default is false.");
        }

        public static ConfigEntry<int> EmeraldsPerStage()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<int>("Chaos Emeralds", "Emeralds Per Stage", 3, "The maximum number of Chaos Emeralds that can spawn in one stage. Host's config takes priority. Default is 3.");
        }

        public static ConfigEntry<int> EmeraldsPerSimulacrumStage()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<int>("Chaos Emeralds", "Emeralds Per Simulacrum Stage", 5, "The maximum number of Chaos Emeralds that can spawn in one stage in Simulacrum. Host's config takes priority. Default is 5.");
        }

        public static ConfigEntry<int> EmeraldCost()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<int>("Chaos Emeralds", "Cost", 50, "How much it costs to buy a Chaos Emerald. Default is 50.\nHost's config takes priority.\nFor reference:\nChest: 25\nLarge Chest: 50\nAltar of Gold: 200\nLegendary Chest: 400");
        }

        public static ConfigEntry<bool> ConsumeEmeraldsOnUse()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<bool>("Chaos Emeralds", "Consume Emeralds On Use", true, "Determines whether the Chaos Emeralds will be consumed when transforming into Super Sonic. If not, the emeralds will stay but won't be able to be used until the next stage. Host's config takes priority. Default is true.");
        }
    }
}