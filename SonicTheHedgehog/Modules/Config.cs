using BepInEx.Configuration;
using RiskOfOptions.Options;
using RiskOfOptions.Utils;
using SonicTheHedgehog.Modules.Forms;
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
            return SonicTheHedgehogPlugin.instance.Config.Bind<int>("Chaos Emeralds", "Cost", 50, "How much it costs to buy a Chaos Emerald. Host's config takes priority. Default is 50.\nFor reference:\nChest: 25\nLarge Chest: 50\nAltar of Gold: 200\nLegendary Chest: 400");
        }

        public static ConfigEntry<bool> ConsumeEmeraldsOnUse()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<bool>("Chaos Emeralds", "Consume Emeralds On Use", true, "Determines whether the Chaos Emeralds will be consumed when transforming into Super Sonic. If not, the emeralds will stay but won't be able to be used until the next stage. Host's config takes priority. Default is true.");
        }

        public static ConfigEntry<FormItemSharing> NeededItemSharing() // How do you get configs into their own separate category in RiskOfOptions, like separate from the Sonic mod. I've seen Aerolt and StageAesthetics do it but idk how
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<FormItemSharing>("Chaos Emeralds", "Item Sharing", FormItemSharing.All, "Handles how Chaos Emeralds are shared between teammates and determines who has permission to transform based on the items they have. The restrictions only apply to the first person to transform and don't apply to anyone who transforms in the 10 second window afterwards. Host's config takes priority. Default is All.\n\nAssuming all items have been collected across the team...\nAll: Anyone, whether they HAVE ANY ITEMS OR NOT, can transform.\nContributor: Players that have AT LEAST ONE of the needed items can transform\nMajorityRule: The player(s) with the MAJORITY number of the needed items can transform\nNone: Only the player with ALL items can transform\n\nWARNING: A mod that lets you drop items to your teammates is reccommended if you are changing this setting. Otherwise, transforming may be impossible if items are split between players in certain ways.");
        }

        public static ConfigEntry<bool> AnnounceSuperTransformation()
        {
            return SonicTheHedgehogPlugin.instance.Config.Bind<bool>("Chaos Emeralds", "Announce Super Transformation", false, "If true, a message will be sent in chat when someone transforms. The message will include the name of the player who transformed. The message won't be sent for anyone who transforms in the 10 second window after someone else transforms. Host's config takes priority. Default is false.");
        }
    }
}