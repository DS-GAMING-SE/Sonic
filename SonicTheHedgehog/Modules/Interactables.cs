using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using System.Collections.Generic;
using EntityStates;

namespace SonicTheHedgehog.Modules
{
    
    public sealed class ChaosEmeraldInteractable : NetworkBehaviour
    {
        public static DirectorPlacementRule placementRule = new DirectorPlacementRule
        {
            placementMode = DirectorPlacementRule.PlacementMode.Random
        };

        public static GameObject prefabBase;

        public static PurchaseInteraction purchaseInteractionBase;

        private static Vector3 dropVelocity = Vector3.up * 20;

        public static Material ringMaterial;

        public PurchaseInteraction purchaseInteraction;

        public EntityStateMachine stateMachine;

        public PickupDisplay pickupDisplay;

        public PickupIndex pickupIndex;

        [SyncVar]
        public EmeraldColor color;


        public static void Initialize()
        {
            Debug.Log("Starting Emerald Interactable Init");
            prefabBase = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("ChaosEmeraldInteractable");

            Assets.MaterialSwap(prefabBase, "RoR2/Base/Common/VFX/matInverseDistortion.mat", "RingParent/PurchaseParticle/Distortion");

            Debug.Log("Loaded Base");
            prefabBase.AddComponent<NetworkIdentity>();

            if (!prefabBase.TryGetComponent<RoR2.PurchaseInteraction>(out purchaseInteractionBase))
            {
                purchaseInteractionBase = prefabBase.AddComponent<RoR2.PurchaseInteraction>();
            }

            Debug.Log("PurchaseInteraction added");

            prefabBase.GetComponent<Highlight>().targetRenderer = prefabBase.transform.Find("RingParent/Ring").GetComponent<MeshRenderer>();

            GameObject trigger = prefabBase.transform.Find("Trigger").gameObject;

            if (trigger.TryGetComponent<RoR2.EntityLocator>(out RoR2.EntityLocator locator))
            {
                locator.entity = prefabBase;
            }
            else
            {
                trigger.AddComponent<RoR2.EntityLocator>().entity = prefabBase;
            }

            Debug.Log("Trigger done");

            purchaseInteractionBase.available = true;
            purchaseInteractionBase.cost = StaticValues.chaosEmeraldCost;
            purchaseInteractionBase.automaticallyScaleCostWithDifficulty = true;
            purchaseInteractionBase.costType = CostTypeIndex.Money;
            purchaseInteractionBase.contextToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +
                                               "_SONIC_THE_HEDGEHOG_BODY_EMERALD_TEMPLE_CONTEXT";

            var hologramController = prefabBase.AddComponent<RoR2.Hologram.HologramProjector>();
            hologramController.hologramPivot = prefabBase.transform.Find("Hologram");
            hologramController.displayDistance = 10;

            prefabBase.AddComponent<PingInfoProvider>().pingIconOverride = Assets.mainAssetBundle.LoadAsset<Sprite>("texEmeraldInteractableIcon");

            prefabBase.transform.Find("PickupDisplay").gameObject.AddComponent<PickupDisplay>();
            Debug.Log("PickupDisplay done");

            Materials.ShinyMaterial(Assets.mainAssetBundle.LoadAsset<Material>("matRing"));

            Debug.Log("Material Done");
            
            prefabBase.AddComponent<ChaosEmeraldInteractable>();

            var entityStateMachine = prefabBase.AddComponent<EntityStateMachine>();
            entityStateMachine.customName = "Body";
            entityStateMachine.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityState));
            entityStateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityState));

            var networkStateMachine = prefabBase.AddComponent<NetworkStateMachine>();
            Helpers.Append(ref networkStateMachine.stateMachines, new List<EntityStateMachine> { entityStateMachine });

            PrefabAPI.RegisterNetworkPrefab(prefabBase);

            //Content.AddNetworkedObjectPrefab(prefabBase);
        }

        private void Start()
        {
            Debug.Log("Emerald Start");

            pickupDisplay = base.GetComponentInChildren<PickupDisplay>();
            purchaseInteraction = base.GetComponent<PurchaseInteraction>();

            stateMachine = EntityStateMachine.FindByCustomName(gameObject, "Body");

            if (NetworkServer.active)
            {
                purchaseInteraction.onPurchase.AddListener(OnPurchase);
                this.color = SuperSonicHandler.available[0];
                SuperSonicHandler.available.Remove(this.color);
            }
            UpdateColor();

        }

        public PickupIndex GetPickupIndex()
        {
            switch (this.color)
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
                                                   "_SONIC_THE_HEDGEHOG_BODY_EMERALD_TEMPLE_" + this.color.ToString().ToUpper();
            }

            pickupIndex = GetPickupIndex();

            pickupDisplay.SetPickupIndex(pickupIndex);
        }

        public void OnPurchase(Interactor interactor)
        {
            Debug.Log("Bought " + color + " Chaos Emerald.");
            purchaseInteraction.SetAvailable(false);
            this.stateMachine.SetNextState(new SkillStates.InteractablePurchased());
        }

        public void DropPickup()
        {
            pickupDisplay.SetPickupIndex(PickupIndex.none);
            if (!NetworkServer.active)
            {
                //Debug.LogWarning("[Server] function 'ChaosEmeraldInteractable::DropPickup()' called on client");
                return;
            }
            PickupDropletController.CreatePickupDroplet(this.pickupIndex, (pickupDisplay.transform).position, base.transform.TransformVector(dropVelocity));
        }

        public void Disappear()
        {
            gameObject.transform.Find("Trigger").gameObject.SetActive(false);
            gameObject.transform.Find("RingParent/Ring").gameObject.SetActive(false);
        }

        public enum EmeraldColor : byte
        {
            Yellow = 0,
            Blue = 1,
            Red = 2,
            Gray = 3,
            Green = 4,
            Cyan = 5,
            Purple = 6
        }

        // I don't know why the commented out parts of the OnSerialize and OnDeserialize methods break the OnPurchase syncing and animation.
        // I have no idea why, but it doesn't really matter since those lines are only for Chaos Emeralds changing color dynamically, which can't happen anyway :)
        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (initialState)
            {
                writer.Write((byte)color);
                return true;
            }
            bool flag = false;
            /*if ((base.syncVarDirtyBits & 1U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write((byte)color);
            }
            */
            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                this.color = (EmeraldColor)reader.ReadByte();
                return;
            }
            /*int num = (int)reader.ReadPackedUInt32();
            if ((num & 1U) != 0U)
            {
                this.color = (EmeraldColor)reader.ReadByte();
            }
            */
        }

    }
}