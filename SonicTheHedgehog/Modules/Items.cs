using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace SonicTheHedgehog.Modules
{
    public static class Items
    {
        // Emerald item tier
        internal static ItemTierDef emeraldTier;
        
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
            emeraldTier = ScriptableObject.CreateInstance<ItemTierDef>();
            emeraldTier.tier = ItemTier.AssignedAtRuntime;
            emeraldTier.isDroppable = true;
            emeraldTier.canRestack = false;
            emeraldTier.pickupRules = ItemTierDef.PickupRules.Default;
            emeraldTier.name = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_CHAOS_EMERALD";
            emeraldTier.highlightPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/HighlightTier1Item.prefab").WaitForCompletion();
            emeraldTier.dropletDisplayPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/Tier1Orb.prefab").WaitForCompletion();
            emeraldTier.canScrap = false;
            emeraldTier.colorIndex = ColorCatalog.ColorIndex.Tier1Item;
            emeraldTier.darkColorIndex = ColorCatalog.ColorIndex.Tier1ItemDark;
            emeraldTier.bgIconTexture = Assets.mainAssetBundle.LoadAsset<Texture>("texBGEmerald");

            Content.AddItemTierDef(emeraldTier);

            // The first string input on this method, the name of the itemDef, is an internal name and CANNOT have spaces or other special characters
            // THIS was the reason the mastery skin wasn't working. THIS was the reason RunReports were breaking

            yellowEmerald = AddNewItem("ChaosEmeraldYellow", "YELLOW_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texYellowEmeraldIcon"),
                CreateEmeraldPrefab("YellowEmerald.prefab"));
            
            redEmerald = AddNewItem("ChaosEmeraldRed", "RED_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRedEmeraldIcon"),
                CreateEmeraldPrefab("RedEmerald.prefab"));
            
            grayEmerald = AddNewItem("ChaosEmeraldGray", "GRAY_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGrayEmeraldIcon"),
                CreateEmeraldPrefab("GrayEmerald.prefab"));
            
            blueEmerald = AddNewItem("ChaosEmeraldBlue", "BLUE_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texBlueEmeraldIcon"),
                CreateEmeraldPrefab("BlueEmerald.prefab"));
            
            cyanEmerald = AddNewItem("ChaosEmeraldCyan", "CYAN_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texCyanEmeraldIcon"),
                CreateEmeraldPrefab("CyanEmerald.prefab"));
            
            greenEmerald = AddNewItem("ChaosEmeraldGreen", "GREEN_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGreenEmeraldIcon"),
                CreateEmeraldPrefab("GreenEmerald.prefab"));
            
            purpleEmerald = AddNewItem("ChaosEmeraldPurple", "PURPLE_EMERALD", true, emeraldTier,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texPurpleEmeraldIcon"),
                CreateEmeraldPrefab("PurpleEmerald.prefab"));
        }

        internal static GameObject CreateEmeraldPrefab(string assetName)
        {
            GameObject emerald = Assets.mainAssetBundle.LoadAsset<GameObject>(assetName);
            ModelPanelParameters panel = emerald.AddComponent<ModelPanelParameters>();
            panel.focusPointTransform = emerald.transform.Find("FocusPoint");

            panel.cameraPositionTransform = emerald.transform.Find("FocusPoint/CameraPosition");

            panel.minDistance = 0.6f;
            panel.maxDistance = 1.5f;
            return emerald;
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
            //itemDef.deprecatedTier = ItemTier.AssignedAtRuntime;
            itemDef._itemTierDef = itemTier;
            
            itemDef.nameToken = prefix + token; // stylised name
            itemDef.pickupToken = prefix + token + "_PICKUP";
            itemDef.descriptionToken = prefix + token + "_DESC";
            itemDef.loreToken = prefix + token + "_LORE";
            itemDef.tags = new[]
            {
                ItemTag.CannotCopy,
                ItemTag.CannotSteal,
                ItemTag.CannotDuplicate,
                ItemTag.WorldUnique,
                ItemTag.AIBlacklist
            };

            itemDef.CreatePickupDef();

            Modules.Content.AddItemDef(itemDef);

            return itemDef;
        }
    }
}