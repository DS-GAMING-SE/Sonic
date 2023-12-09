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

        private static Material yellowEmerald;
        private static Material blueEmerald;
        private static Material redEmerald;
        private static Material grayEmerald;
        private static Material greenEmerald;
        private static Material cyanEmerald;
        private static Material purpleEmerald;


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

            prefabBase.AddComponent<PingInfoProvider>().pingIconOverride = Assets.mainAssetBundle.LoadAsset<Sprite>("texEmeraldInteractableIcon");

            prefabBase.AddComponent<ChaosEmeraldInteractable>();

            yellowEmerald = Materials.CreateHopooMaterial("matYellow");
            blueEmerald = Materials.CreateHopooMaterial("matBlue");
            redEmerald = Materials.CreateHopooMaterial("matRed");
            grayEmerald = Materials.CreateHopooMaterial("matGray");
            greenEmerald = Materials.CreateHopooMaterial("matGreen");
            cyanEmerald = Materials.CreateHopooMaterial("matCyan");
            purpleEmerald = Materials.CreateHopooMaterial("matPurple");
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
                    material = yellowEmerald;
                    break;
                case EmeraldColor.Blue:
                    material = blueEmerald;
                    break;
                case EmeraldColor.Red:
                    material = redEmerald;
                    break;
                case EmeraldColor.Gray:
                    material = grayEmerald;
                    break;
                case EmeraldColor.Green:
                    material = greenEmerald;
                    break;
                case EmeraldColor.Cyan:
                    material = cyanEmerald;
                    break;
                case EmeraldColor.Purple:
                    material = purpleEmerald;
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

            Debug.Log("Bought " + color + " Chaos Emerald.");
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