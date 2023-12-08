using RoR2;
using UnityEngine;
using R2API;


namespace SonicTheHedgehog.Modules
{
    public sealed class ChaosEmeraldInteractable : MonoBehaviour
    {
        public static DirectorPlacementRule placementRule = new DirectorPlacementRule
        {
            placementMode = DirectorPlacementRule.PlacementMode.Random
        };

        public static GameObject prefabBase;

        public static PurchaseInteraction purchaseInteractionBase;

        public PurchaseInteraction purchaseInteraction;

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

            purchaseInteractionBase.name = "Silly man";
            purchaseInteractionBase.available = true;
            purchaseInteractionBase.cost = StaticValues.chaosEmeraldCost;
            purchaseInteractionBase.automaticallyScaleCostWithDifficulty = true;
            purchaseInteractionBase.costType = CostTypeIndex.Money;
            purchaseInteractionBase.contextToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +
                                               "_SONIC_THE_HEDGEHOG_BODY_EMERALD_TEMPLE_CONTEXT";
            prefabBase.AddComponent<ChaosEmeraldInteractable>();
        }

        private void Start()
        {
            Debug.Log("Emerald Start");
            purchaseInteraction = GetComponent<PurchaseInteraction>();
            UpdateColor();
            purchaseInteraction.onPurchase.AddListener(OnPurchase);
        }

        public void UpdateColor()
        {
            if (purchaseInteraction)
            {
                purchaseInteraction.displayNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +
                                                   "_SONIC_THE_HEDGEHOG_BODY_EMERALD_TEMPLE_" + color.ToString().ToUpper();
            }
            Material material;
            switch (color)
            {
                default:
                    material = Modules.Assets.mainAssetBundle.LoadAsset<Material>("matYellow");
                    break;
                case EmeraldColor.Blue:
                    material = Modules.Assets.mainAssetBundle.LoadAsset<Material>("matBlue");
                    break;
                case EmeraldColor.Red:
                    material = Modules.Assets.mainAssetBundle.LoadAsset<Material>("matRed");
                    break;
                case EmeraldColor.Gray:
                    material = Modules.Assets.mainAssetBundle.LoadAsset<Material>("matGray");
                    break;
                case EmeraldColor.Green:
                    material = Modules.Assets.mainAssetBundle.LoadAsset<Material>("matGreen");
                    break;
                case EmeraldColor.Cyan:
                    material = Modules.Assets.mainAssetBundle.LoadAsset<Material>("matCyan");
                    break;
                case EmeraldColor.Purple:
                    material = Modules.Assets.mainAssetBundle.LoadAsset<Material>("matPurple");
                    break;
            }
            GameObject emerald = gameObject.transform.GetChild(2).gameObject;
            if (emerald)
            {
                emerald.SetActive(true);
                emerald.GetComponent<MeshRenderer>().material = material;
            }
        }

        public void OnPurchase(Interactor interactor)
        {
            Inventory inventory = interactor.GetComponent<CharacterBody>().inventory;
            if (!inventory)
            {
                return;
            }
            switch (color)
            {
                case EmeraldColor.Blue:
                    inventory.GiveItem(Items.blueEmerald);
                    break;
                case EmeraldColor.Red:
                    inventory.GiveItem(Items.redEmerald);
                    break;
                case EmeraldColor.Gray:
                    inventory.GiveItem(Items.grayEmerald);
                    break;
                case EmeraldColor.Green:
                    inventory.GiveItem(Items.greenEmerald);
                    break;
                case EmeraldColor.Cyan:
                    inventory.GiveItem(Items.cyanEmerald);
                    break;
                case EmeraldColor.Purple:
                    inventory.GiveItem(Items.purpleEmerald);
                    break;
                default:
                    inventory.GiveItem(Items.yellowEmerald);
                    break;
            }

            Debug.Log("Bought this shit dunno, " + color);
            purchaseInteraction.available = false;
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
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