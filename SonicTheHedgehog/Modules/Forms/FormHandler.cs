using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Networking.NetworkSystem;
using EntityStates;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;
using SonicTheHedgehog.Modules.Forms;
using SonicTheHedgehog.Components;
using HarmonyLib;
using Newtonsoft.Json.Utilities;

namespace SonicTheHedgehog.Modules
{
    public class FormHandler : NetworkBehaviour
    {
        // Basically everything except the SyncVars is only handled by server and won't be accurate for clients
        public INeededItemTracker itemTracker;

        public FormDef form;

        public bool eventsSubscribed = false;

        [SyncVar]
        public bool teamSuper;

        [SyncVar]
        public int numberOfTimesTransformed = 0;

        public const float teamSuperTimerDuration = 10f;
        public float teamSuperTimer;

        
        private void OnEnable()
        {
            if (!(form && FormCatalog.formsCatalog.Contains(form))) { Log.Error("FormHandler does not have a valid formDef set."); }
            if (!Forms.Forms.formToHandlerObject.ContainsKey(form))
            {
                itemTracker = GetComponent<INeededItemTracker>();
                Forms.Forms.formToHandlerObject.Add(form, gameObject);
                Log.Message("FormHandler for form " + form.ToString() + " created");
                return;
            }
            Log.Error("Duplicate instance of formHandler "+ form.ToString() +". Only one should exist at a time.");
        }

        private void OnDisable()
        {
            if (Forms.Forms.formToHandlerObject.GetValueSafe(form) == this.gameObject)
            {
                Forms.Forms.formToHandlerObject.Remove(form);
                SetEvents(false);
            }
        }

        public virtual void SetEvents(bool active)
        {
            if (active && !eventsSubscribed)
            {
                RoR2Application.onFixedUpdate += OnFixedUpdate;
                eventsSubscribed = true;
            }
            if (!active && eventsSubscribed)
            {
                RoR2Application.onFixedUpdate -= OnFixedUpdate;
                eventsSubscribed = false;
            }
        }

        public virtual bool HasItems(SuperSonicComponent superSonicComponent)
        {
            if (form.requiresItems)
            {
                if (itemTracker != null)
                {
                    return itemTracker.ItemRequirementMet(superSonicComponent);
                }
            }
            return true;
        }

        public virtual bool CanTransform(SuperSonicComponent component)
        {
            bool hasItems = HasItems(component);
            Log.Message("FormHandler with form " + form.ToString() + "\nTeam Super? " + teamSuper + ". Has Items? " + hasItems + ". Number of transforms? " + numberOfTimesTransformed + " out of max " + form.maxTransforms);
            return (hasItems && (form.maxTransforms <= 0 || numberOfTimesTransformed < form.maxTransforms)) || teamSuper;
        }

        public virtual void OnTransform(GameObject body)
        {
            if (!teamSuper)
            {
                NetworkteamSuper = true;
                NetworknumberOfTimesTransformed += 1;
                teamSuperTimer = teamSuperTimerDuration;
                if (form.consumeItems)
                {
                    itemTracker.RemoveItems(body);
                }
            }
        }

        public virtual void OnFixedUpdate()
        {
            if (teamSuperTimer > 0)
            {
                teamSuperTimer -= Time.deltaTime;
                if (teamSuperTimer <= 0)
                {
                    teamSuper = false;
                    Log.Message("Team Super window ended");
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

        public int NetworknumberOfTimesTransformed
        {
            get
            {
                return numberOfTimesTransformed;
            }
            [param: In]
            set
            {
                base.SetSyncVar<int>(value, ref numberOfTimesTransformed, 2U);
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (initialState)
            {
                writer.Write(teamSuper);
                writer.Write(numberOfTimesTransformed);
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
                writer.Write(numberOfTimesTransformed);
            }
            if (!flag)
            {
                writer.WritePackedUInt32(base.syncVarDirtyBits);
            }
            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                teamSuper = reader.ReadBoolean();
                numberOfTimesTransformed = reader.ReadInt32();
                return;
            }
            int num = (int)reader.ReadPackedUInt32();
            if ((num & 1U) != 0U)
            {
                teamSuper = reader.ReadBoolean();
            }
            if ((num & 2U) != 0U)
            {
                numberOfTimesTransformed = reader.ReadInt32();
            }
        }
    }

    public interface INeededItemTracker
    {
        bool ItemRequirementMet(SuperSonicComponent component);

        void RemoveItems(GameObject body);
    }

    [RequireComponent(typeof(FormHandler))]
    public class SyncedItemTracker : NetworkBehaviour, INeededItemTracker
    {
        public FormHandler handler;
        
        [SyncVar]
        public bool allItems;

        public List<NeededItem> missingItems;

        public bool eventsSubscribed;

        private void OnEnable()
        {
            handler = this.GetComponent<FormHandler>();
            SubscribeEvents(true);
        }

        private void OnDisable()
        {
            SubscribeEvents(false);
        }
        
        public bool ItemRequirementMet(SuperSonicComponent component)
        {
            return allItems;
        }

        public void CheckItems()
        {
            missingItems = new List<NeededItem>();
            if (!handler) { Log.Warning("no handler yet"); return; }
            if (handler.form.neededItems == null) { return; } // What did they do to SystemInitializer I JUST started using it and now it doesn't work after DLC update
            foreach (NeededItem item in handler.form.neededItems)
            {
                int collectiveItemCount = 0;
                foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
                {
                    if (player)
                    {
                        if (!player.master || !player.master.inventory) { continue; }
                        collectiveItemCount += player.master.inventory.GetItemCount(item);
                    }
                }
                if (collectiveItemCount < item.count)
                {
                    NeededItem missing= item;
                    missing.count = (uint)(item.count - collectiveItemCount);
                    missingItems.Add(missing);
                }
            }
            NetworkallItems = missingItems.Count() == 0;
            Log.Message("Missing items for "+ handler.form.ToString() + ": " + string.Concat(missingItems.Select(x => x.ToString())));
        }

        public void OnInventoryChanged(Inventory inventory)
        {
            if (inventory.TryGetComponent(out CharacterMaster master))
            {
                if (master.playerCharacterMasterController) // Only check items again if a player's inventory changes
                {
                    CheckItems();
                }
            }
        }

        public void RemoveItems(GameObject body)
        {
            foreach (NeededItem item in handler.form.neededItems)
            {
                int neededItems = (int)item.count;
                foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
                {
                    int numToConstume = Math.Min(player.master.inventory.GetItemCount(item), neededItems);
                    neededItems -= numToConstume;
                    player.master.inventory.RemoveItem(item, numToConstume);
                    if (neededItems <= 0)
                    {
                        break;
                    }
                }
                if (neededItems > 0)
                {
                    Log.Warning("Does not have the items to be removed for transforming");
                }
            }
        }

        public void SubscribeEvents(bool subscribe)
        {
            if (eventsSubscribed ^ subscribe)
            {
                if (subscribe)
                {
                    Inventory.onInventoryChangedGlobal += OnInventoryChanged;
                    eventsSubscribed = true;
                    Log.Message("Subscribed to inventory events");
                    CheckItems();
                }
                else
                {
                    Inventory.onInventoryChangedGlobal -= OnInventoryChanged;
                    eventsSubscribed = false;
                }
            }
        }
        public bool NetworkallItems
        {
            get
            {
                return allItems;
            }
            [param: In]
            set
            {
                base.SetSyncVar<bool>(value, ref allItems, 1U);
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (initialState)
            {
                writer.Write(allItems);
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
                writer.Write(allItems);
            }
            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                allItems = reader.ReadBoolean();
                return;
            }
            int num = (int)reader.ReadPackedUInt32();
            if ((num & 1U) != 0U)
            {
                allItems = reader.ReadBoolean();
            }
        }
    }

    [RequireComponent(typeof(FormHandler))]
    public class UnsyncedItemTracker : MonoBehaviour, INeededItemTracker
    {
        public FormHandler handler;

        private void Start()
        {
            handler = this.GetComponent<FormHandler>();
        }

        public bool ItemRequirementMet(SuperSonicComponent component)
        {
            Log.Message("Checking unsynceditemtracker");
            return component.formToItemTracker.GetValueSafe(handler.form).allItems;
        }

        public void RemoveItems(GameObject body)
        {
            if (body.TryGetComponent(out CharacterBody characterBody))
            {
                if (characterBody.master)
                {
                    foreach (NeededItem item in handler.form.neededItems)
                    {
                        if (characterBody.master.inventory.GetItemCount(item) >= item.count)
                        {
                            characterBody.master.inventory.RemoveItem(item, (int)item.count);
                        }
                        else
                        {
                            Log.Warning("Does not have the items to be removed for transforming");
                            characterBody.master.inventory.RemoveItem(item, characterBody.master.inventory.GetItemCount(item));
                        }
                    }
                }
            }
        }
    }

    public class SuperSonicHandler : FormHandler
    {
        public static List<ChaosEmeraldInteractable.EmeraldColor> available;

        public void FilterOwnedEmeralds()
        {
            available = new List<ChaosEmeraldInteractable.EmeraldColor>(new ChaosEmeraldInteractable.EmeraldColor[]
            {ChaosEmeraldInteractable.EmeraldColor.Yellow, ChaosEmeraldInteractable.EmeraldColor.Blue, ChaosEmeraldInteractable.EmeraldColor.Red,
                ChaosEmeraldInteractable.EmeraldColor.Gray, ChaosEmeraldInteractable.EmeraldColor.Green, ChaosEmeraldInteractable.EmeraldColor.Cyan, ChaosEmeraldInteractable.EmeraldColor.Purple });

            if (itemTracker.GetType().IsAssignableFrom(typeof(SyncedItemTracker)))
            {
                if (!((SyncedItemTracker)itemTracker).missingItems.Contains(Items.yellowEmerald.itemIndex)) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Yellow); }
                if (!((SyncedItemTracker)itemTracker).missingItems.Contains(Items.blueEmerald.itemIndex)) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Blue); }
                if (!((SyncedItemTracker)itemTracker).missingItems.Contains(Items.redEmerald.itemIndex)) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Red); }
                if (!((SyncedItemTracker)itemTracker).missingItems.Contains(Items.grayEmerald.itemIndex)) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Gray); }
                if (!((SyncedItemTracker)itemTracker).missingItems.Contains(Items.greenEmerald.itemIndex)) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Green); }
                if (!((SyncedItemTracker)itemTracker).missingItems.Contains(Items.cyanEmerald.itemIndex)) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Cyan); }
                if (!((SyncedItemTracker)itemTracker).missingItems.Contains(Items.purpleEmerald.itemIndex)) { available.Remove(ChaosEmeraldInteractable.EmeraldColor.Purple); }
            }
        }
    }
}