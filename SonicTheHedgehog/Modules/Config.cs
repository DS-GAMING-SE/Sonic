using BepInEx.Configuration;
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
    }
}