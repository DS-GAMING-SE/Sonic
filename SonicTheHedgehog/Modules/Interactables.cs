using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace SonicTheHedgehog.Modules
{
    public class ChaosEmeraldSpawnHandler : MonoBehaviour
    {
        public static ChaosEmeraldSpawnHandler instance { get; private set; }

        public static List<ChaosEmeraldInteractable.EmeraldColor> available;

        private void OnEnable()
        {
            if (!ChaosEmeraldSpawnHandler.instance)
            {
                ChaosEmeraldSpawnHandler.instance = this;
                return;
            }
            Debug.LogErrorFormat(this, "Duplicate instance of singleton class {0}. Only one should exist at a time.", new object[]
            {
                base.GetType().Name
            });
        }

        private void OnDisable()
        {
            if (ChaosEmeraldSpawnHandler.instance == this)
            {
                ChaosEmeraldSpawnHandler.instance = null;
            }
        }

        public void ResetAvailable()
        {
            available = new List<ChaosEmeraldInteractable.EmeraldColor>(new ChaosEmeraldInteractable.EmeraldColor[]
            {ChaosEmeraldInteractable.EmeraldColor.Yellow, ChaosEmeraldInteractable.EmeraldColor.Blue, ChaosEmeraldInteractable.EmeraldColor.Red,
                ChaosEmeraldInteractable.EmeraldColor.Gray, ChaosEmeraldInteractable.EmeraldColor.Green, ChaosEmeraldInteractable.EmeraldColor.Cyan, ChaosEmeraldInteractable.EmeraldColor.Purple });
        }

        public void FilterOwnedEmeralds()
        {
            foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
            {
                if (player.master.inventory.GetItemCount(Items.yellowEmerald) > 0) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Yellow); }
                if (player.master.inventory.GetItemCount(Items.blueEmerald) > 0) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Blue); }
                if (player.master.inventory.GetItemCount(Items.redEmerald) > 0) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Red); }
                if (player.master.inventory.GetItemCount(Items.grayEmerald) > 0) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Gray); }
                if (player.master.inventory.GetItemCount(Items.greenEmerald) > 0) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Green); }
                if (player.master.inventory.GetItemCount(Items.cyanEmerald) > 0) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Cyan); }
                if (player.master.inventory.GetItemCount(Items.purpleEmerald) > 0) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Purple); }
            }
        }
    }
    
    public sealed class ChaosEmeraldInteractable : MonoBehaviour
    {
        public static DirectorPlacementRule placementRule = new DirectorPlacementRule
        {
            placementMode = DirectorPlacementRule.PlacementMode.Random
        };

        public static GameObject prefabBase;

        public static PurchaseInteraction purchaseInteractionBase;

        private static Vector3 dropVelocity = Vector3.up * 15;

        public PurchaseInteraction purchaseInteraction;

        public PickupDisplay pickupDisplay;

        public PickupIndex pickupIndex;

        [SyncVar]
        public EmeraldColor color;


        public static void Initialize()
        {           
            prefabBase = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SonicAtlasInteractable");

            if (!prefabBase.TryGetComponent<RoR2.PurchaseInteraction>(out purchaseInteractionBase))
            {
                purchaseInteractionBase = prefabBase.AddComponent<RoR2.PurchaseInteraction>();
            }

            prefabBase.GetComponent<Highlight>().targetRenderer = prefabBase.transform.GetChild(1).GetComponent<MeshRenderer>();

            GameObject trigger = prefabBase.transform.GetChild(prefabBase.transform.childCount - 1).gameObject;

            if (trigger.TryGetComponent<RoR2.EntityLocator>(out RoR2.EntityLocator locator))
            {
                locator.entity = prefabBase;
            }
            else
            {
                trigger.AddComponent<RoR2.EntityLocator>().entity = prefabBase;
            }

            purchaseInteractionBase.available = true;
            purchaseInteractionBase.cost = StaticValues.chaosEmeraldCost;
            purchaseInteractionBase.automaticallyScaleCostWithDifficulty = true;
            purchaseInteractionBase.costType = CostTypeIndex.Money;
            purchaseInteractionBase.contextToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +
                                               "_SONIC_THE_HEDGEHOG_BODY_EMERALD_TEMPLE_CONTEXT";

            prefabBase.AddComponent<PingInfoProvider>().pingIconOverride = Assets.mainAssetBundle.LoadAsset<Sprite>("texEmeraldInteractableIcon");

            prefabBase.transform.GetChild(2).gameObject.AddComponent<PickupDisplay>();

            prefabBase.AddComponent<ChaosEmeraldInteractable>();

            Content.AddNetworkedObjectPrefab(prefabBase);
        }

        private void Start()
        {
            Debug.Log("Emerald Start");

            if (NetworkServer.active)
            {
                this.color = ChaosEmeraldSpawnHandler.available[0];
                ChaosEmeraldSpawnHandler.available.Remove(this.color);
            }
            pickupDisplay = base.GetComponentInChildren<PickupDisplay>();
            purchaseInteraction = base.GetComponent<PurchaseInteraction>();
            pickupIndex = GetPickupIndex();

            UpdateColor();

            purchaseInteraction.onPurchase.AddListener(OnPurchase);
        }

        public PickupIndex GetPickupIndex()
        {
            switch (color)
            {
                default:
                    return PickupCatalog.FindPickupIndex(Items.yellowEmerald.itemIndex);
                case EmeraldColor.Blue:
                    return PickupCatalog.FindPickupIndex(Items.blueEmerald.itemIndex);
                case EmeraldColor.Red:
                    return PickupCatalog.FindPickupIndex(Items.redEmerald.itemIndex);
                case EmeraldColor.Gray:
                    return PickupCatalog.FindPickupIndex(Items.grayEmerald.itemIndex);
                case EmeraldColor.Green:
                    return PickupCatalog.FindPickupIndex(Items.greenEmerald.itemIndex);
                case EmeraldColor.Cyan:
                    return PickupCatalog.FindPickupIndex(Items.cyanEmerald.itemIndex);
                case EmeraldColor.Purple:
                    return PickupCatalog.FindPickupIndex(Items.purpleEmerald.itemIndex);
            }
        }

        public void UpdateColor()
        {  
            if (purchaseInteraction)
            {
                purchaseInteraction.displayNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +
                                                   "_SONIC_THE_HEDGEHOG_BODY_EMERALD_TEMPLE_" + color.ToString().ToUpper();
            }

            pickupDisplay.SetPickupIndex(pickupIndex);
        }

        public void OnPurchase(Interactor interactor)
        {
            pickupDisplay.SetPickupIndex(PickupIndex.none);
            Debug.Log("Bought " + color + " Chaos Emerald.");
            purchaseInteraction.available = false;
            DropPickup();
        }

        [Server]
        public void DropPickup()
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.ShopTerminalBehavior::DropPickup()' called on client");
                return;
            }
            PickupDropletController.CreatePickupDroplet(this.pickupIndex, (pickupDisplay.transform).position, base.transform.TransformVector(dropVelocity));
        }

        public enum EmeraldColor
        {
            Yellow = 0,
            Blue = 1,
            Red = 2,
            Gray = 3,
            Green = 4,
            Cyan = 5,
            Purple = 6
        }

    }
}