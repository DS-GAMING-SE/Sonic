using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            ItemTierDef emeraldTier = new ItemTierDef
            {
                _tier = ItemTier.AssignedAtRuntime,
                isDroppable = true,
                canRestack = false,
                pickupRules = ItemTierDef.PickupRules.Default,
                name = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_CHAOS_EMERALD",
                highlightPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/HighlightTier1Item.prefab").WaitForCompletion(),
                dropletDisplayPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/Tier1Orb.prefab").WaitForCompletion(),
                canScrap = false,
                colorIndex = ColorCatalog.ColorIndex.Tier1Item,
                darkColorIndex = ColorCatalog.ColorIndex.Tier1ItemDark,
                bgIconTexture = Assets.mainAssetBundle.LoadAsset<Texture>("texBGEmerald")
            };

            Content.AddItemTierDef(emeraldTier);

            yellowEmerald = AddNewItem("Chaos Emerald: Yellow", "YELLOW_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texYellowEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("YellowEmerald.prefab"));
            
            redEmerald = AddNewItem("Chaos Emerald: Red", "RED_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRedEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("RedEmerald.prefab"));
            
            grayEmerald = AddNewItem("Chaos Emerald: Gray", "GRAY_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGrayEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("GrayEmerald.prefab"));
            
            blueEmerald = AddNewItem("Chaos Emerald: Blue", "BLUE_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texBlueEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("BlueEmerald.prefab"));
            
            cyanEmerald = AddNewItem("Chaos Emerald: Cyan", "CYAN_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texCyanEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("CyanEmerald.prefab"));
            
            greenEmerald = AddNewItem("Chaos Emerald: Green", "GREEN_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGreenEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("GreenEmerald.prefab"));
            
            purpleEmerald = AddNewItem("Chaos Emerald: Purple", "PURPLE_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texPurpleEmeraldIcon"),
                Assets.mainAssetBundle.LoadAsset<GameObject>("PurpleEmerald.prefab"));
        }

        // simple helper method
        internal static ItemDef AddNewItem(string itemName, string token, bool canRemove, ItemTierDef itemTier, Sprite icon, GameObject pickupModelPrefab)
        {
            string prefix = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_";
            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = ItemTier.AssignedAtRuntime;
            itemDef.pickupModelPrefab = pickupModelPrefab;
            itemDef.pickupIconSprite = icon;
            itemDef.canRemove = canRemove;
            itemDef.deprecatedTier = ItemTier.AssignedAtRuntime;
            itemDef._itemTierDef = itemTier;
            
            itemDef.nameToken = prefix + token; // stylised name
            itemDef.pickupToken = prefix + token + "_PICKUP";
            itemDef.descriptionToken = prefix + token + "_DESC";
            itemDef.loreToken = prefix + token + "_LORE";
            itemDef.tags = new[]
            {
                ItemTag.InteractableRelated,
                ItemTag.CannotCopy,
                ItemTag.CannotSteal
            };

            itemDef.CreatePickupDef();

            Modules.Content.AddItemDef(itemDef);

            return itemDef;
        }
    }
}