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
            yellowEmerald = AddNewItem("Yellow Emerald", ItemTier.Tier2,
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                Assets.mainAssetBundle.LoadAsset<GameObject>("SonicEmoteSupport.prefab"));
            
            redEmerald = AddNewItem("Red Emerald", ItemTier.Tier2,
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                Assets.mainAssetBundle.LoadAsset<GameObject>("SonicEmoteSupport.prefab"));
            
            grayEmerald = AddNewItem("Gray Emerald", ItemTier.Tier2,
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                Assets.mainAssetBundle.LoadAsset<GameObject>("SonicEmoteSupport.prefab"));
            
            blueEmerald = AddNewItem("Blue Emerald", ItemTier.Tier2,
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                Assets.mainAssetBundle.LoadAsset<GameObject>("SonicEmoteSupport.prefab"));
            
            cyanEmerald = AddNewItem("Cyan Emerald", ItemTier.Tier2,
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                Assets.mainAssetBundle.LoadAsset<GameObject>("SonicEmoteSupport.prefab"));
            
            greenEmerald = AddNewItem("Green Emerald", ItemTier.Tier2,
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                Assets.mainAssetBundle.LoadAsset<GameObject>("SonicEmoteSupport.prefab"));
            
            purpleEmerald = AddNewItem("Purple Emerald", ItemTier.Tier2,
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                Assets.mainAssetBundle.LoadAsset<GameObject>("SonicEmoteSupport.prefab"));
        }

        // simple helper method
        internal static ItemDef AddNewItem(string itemName, ItemTier tier, Sprite icon, GameObject pickupModelPrefab)
        {
            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = tier;
            itemDef.pickupModelPrefab = pickupModelPrefab;
            itemDef.pickupIconSprite = icon;

            Modules.Content.AddItemDef(itemDef);

            return itemDef;
        }
    }
}