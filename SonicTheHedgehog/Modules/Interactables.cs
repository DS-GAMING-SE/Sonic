using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Networking.NetworkSystem;
using EntityStates;
using UnityEngine.AddressableAssets;

namespace SonicTheHedgehog.Modules
{
    public class SuperSonicHandler : NetworkBehaviour
    {
        // Basically everything except the SyncVars is only handled by server and won't be accurate for clients
        public static SuperSonicHandler instance { get; private set; }
        public static GameObject handlerPrefab;

        public static List<ChaosEmeraldInteractable.EmeraldColor> available;
        
        public bool eventsSubscribed = false;

        [SyncVar]
        public static bool allEmeralds;

        [SyncVar]
        public static bool teamSuper;

        public const float teamSuperTimerDuration = 10f;
        public static float teamSuperTimer;

        public static void Initialize()
        {
            handlerPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SuperSonicHandler");
            handlerPrefab.AddComponent<SuperSonicHandler>();
            handlerPrefab.AddComponent<NetworkIdentity>();
            //handlerPrefab = new GameObject("SuperSonicHandler", typeof(SuperSonicHandler));
            PrefabAPI.RegisterNetworkPrefab(handlerPrefab);
            //Content.AddNetworkedObjectPrefab(handlerPrefab);
        }
        
        private void OnEnable()
        {
            if (!instance)
            {
                instance = this;
                return;
            }
            Debug.LogErrorFormat(this, "Duplicate instance of singleton class {0}. Only one should exist at a time.", new object[]
            {
                base.GetType().Name
            });
        }

        private void OnDisable()
        {
            if (instance == this)
            {
                instance = null;
                SetEvents(false);
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

        public void SetEvents(bool active)
        {
            if (active && !eventsSubscribed)
            {
                Inventory.onInventoryChangedGlobal += OnInventoryChanged;
                RoR2Application.onFixedUpdate += OnFixedUpdate;
                eventsSubscribed = true;
                UpdateEmeraldCheck();
            }
            if (!active && eventsSubscribed)
            {
                RoR2Application.onFixedUpdate -= OnFixedUpdate;
                Inventory.onInventoryChangedGlobal -= OnInventoryChanged;
                eventsSubscribed = false;
            }
        }

        public void OnInventoryChanged(Inventory inventory)
        {
            UpdateEmeraldCheck();
        }

        public void UpdateEmeraldCheck()
        {
            bool yellow = false;
            bool blue = false;
            bool red = false;
            bool gray = false;
            bool green = false;
            bool cyan = false;
            bool purple = false;
            foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
            {
                if (player.master.inventory.GetItemCount(Items.yellowEmerald) > 0) { yellow = true; }
                if (player.master.inventory.GetItemCount(Items.blueEmerald) > 0) { blue = true; }
                if (player.master.inventory.GetItemCount(Items.redEmerald) > 0) { red = true; }
                if (player.master.inventory.GetItemCount(Items.grayEmerald) > 0) { gray = true; }
                if (player.master.inventory.GetItemCount(Items.greenEmerald) > 0) { green = true; }
                if (player.master.inventory.GetItemCount(Items.cyanEmerald) > 0) { cyan = true; }
                if (player.master.inventory.GetItemCount(Items.purpleEmerald) > 0) { purple = true; }
            }

            NetworkallEmeralds = yellow && blue && red && gray && green && cyan && purple;
        }

        public void RemoveEmeralds()
        {
            bool yellow = false;
            bool blue = false;
            bool red = false;
            bool gray = false;
            bool green = false;
            bool cyan = false;
            bool purple = false;
            foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
            {
                if (player.master.inventory.GetItemCount(Items.yellowEmerald) > 0 && yellow == false) { player.master.inventory.RemoveItem(Items.yellowEmerald); yellow = true; }
                if (player.master.inventory.GetItemCount(Items.blueEmerald) > 0 && blue == false) { player.master.inventory.RemoveItem(Items.blueEmerald); blue = true; }
                if (player.master.inventory.GetItemCount(Items.redEmerald) > 0 && red == false) { player.master.inventory.RemoveItem(Items.redEmerald); red = true; }
                if (player.master.inventory.GetItemCount(Items.grayEmerald) > 0 && gray == false) { player.master.inventory.RemoveItem(Items.grayEmerald); gray = true; }
                if (player.master.inventory.GetItemCount(Items.greenEmerald) > 0 && green == false) { player.master.inventory.RemoveItem(Items.greenEmerald); green = true; }
                if (player.master.inventory.GetItemCount(Items.cyanEmerald) > 0 && cyan == false) { player.master.inventory.RemoveItem(Items.cyanEmerald); cyan = true; }
                if (player.master.inventory.GetItemCount(Items.purpleEmerald) > 0 && purple == false) { player.master.inventory.RemoveItem(Items.purpleEmerald); purple = true; }
            }
        }

        public virtual bool CanTransform()
        {
            Debug.Log("All Emeralds? " + allEmeralds + ". Team Super? " + teamSuper);
            return allEmeralds || teamSuper;
        }

        public void OnTransform()
        {
            if (!teamSuper)
            {
                NetworkteamSuper = true;
                teamSuperTimer = teamSuperTimerDuration;
                RemoveEmeralds();
            }
        }

        public void OnFixedUpdate()
        {
            if (teamSuperTimer > 0)
            {
                teamSuperTimer -= Time.deltaTime;
                if (teamSuperTimer <= 0)
                {
                    NetworkteamSuper = false;
                    Debug.Log("Team Super window ended");
                }
            }
        }

        public bool NetworkteamSuper
        {
            get
            {
                return teamSuper;
            }
            [param: In]
            set
            {
                base.SetSyncVar<bool>(value, ref teamSuper, 1U);
            }
        }

        public bool NetworkallEmeralds
        {
            get
            {
                return allEmeralds;
            }
            [param: In]
            set
            {
                base.SetSyncVar<bool>(value, ref allEmeralds, 2U);
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (initialState)
            {
                writer.Write(teamSuper);
                writer.Write(allEmeralds);
                return true;
            }
            bool flag = false;
            if ((base.syncVarDirtyBits & 1U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(teamSuper);
            }
            if ((base.syncVarDirtyBits & 2U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(allEmeralds);
            }
            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                teamSuper = reader.ReadBoolean();
                allEmeralds = reader.ReadBoolean();
                return;
            }
            int num = (int)reader.ReadPackedUInt32();
            if ((num & 1U) != 0U)
            {
                teamSuper = reader.ReadBoolean();
            }
            if ((num & 2U) != 0U)
            {
                allEmeralds = reader.ReadBoolean();
            }
        }
    }
    
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