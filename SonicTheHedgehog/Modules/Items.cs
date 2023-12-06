using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SonicTheHedgehog.Modules
{
    public static class Items
    {
        // Definition of all the different chaos emeralds.
        internal static ItemDef yellowEmerald;
        internal static ItemDef redEmerald;
        internal static ItemDef grayEmerald;
        internal static ItemDef blueEmerald;
        internal static ItemDef cyanEmerald;
        internal static ItemDef greenEmerald;
        internal static ItemDef purpleEmerald;

        internal static void RegisterItems()
        {
            yellowEmerald = AddNewItem("Yellow Emerald", "YELLOW_EMERALD", true, ItemTier.Tier2,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texYellowEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("YellowEmerald.prefab"));
            
            redEmerald = AddNewItem("Red Emerald", "RED_EMERALD", true, ItemTier.Tier2,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRedEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("RedEmerald.prefab"));
            
            grayEmerald = AddNewItem("Gray Emerald", "GRAY_EMERALD", true, ItemTier.Tier2,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGrayEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("GrayEmerald.prefab"));
            
            blueEmerald = AddNewItem("Blue Emerald", "BLUE_EMERALD", true, ItemTier.Tier2,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texBlueEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("BlueEmerald.prefab"));
            
            cyanEmerald = AddNewItem("Cyan Emerald", "CYAN_EMERALD", true, ItemTier.Tier2,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texCyanEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("CyanEmerald.prefab"));
            
            greenEmerald = AddNewItem("Green Emerald", "GREEN_EMERALD", true, ItemTier.Tier2,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGreenEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("GreenEmerald.prefab"));
            
            purpleEmerald = AddNewItem("Purple Emerald", "PURPLE_EMERALD", true, ItemTier.Tier2,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texPurpleEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("PurpleEmerald.prefab"));
        }

        // simple helper method
        internal static ItemDef AddNewItem(string itemName, string token, bool canRemove, ItemTier tier, Sprite icon, GameObject pickupModelPrefab)
        {
            string prefix = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_";
            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = tier;
            itemDef.pickupModelPrefab = pickupModelPrefab;
            itemDef.pickupIconSprite = icon;
            itemDef.canRemove = canRemove;
            
            itemDef.nameToken = prefix + token; // stylised name
            itemDef.pickupToken = prefix + token + "_PICKUP";
            itemDef.descriptionToken = prefix + token + "_DESC";
            itemDef.loreToken = prefix + token + "_LORE";
            itemDef.tags = new[]
            {
                ItemTag.Utility,
                ItemTag.CannotCopy
            };

            Modules.Content.AddItemDef(itemDef);

            return itemDef;
        }
    }
}