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
                Log.Error("Form that needs items has no itemTracker");
            }
            return true;
        }

        public virtual bool CanTransform(SuperSonicComponent component)
        {
            bool hasItems = HasItems(component);
            Log.Message("FormHandler with form " + form.ToString() + "\nTeam Super? " + teamSuper + ". Has Items? " + hasItems + ". Number of transforms? " + numberOfTimesTransformed + " out of max " + form.maxTransforms);
            return (hasItems && (form.maxTransforms <= 0 || numberOfTimesTransformed < form.maxTransforms)) || teamSuper;
        }

        public virtual void OnTransform(SuperSonicComponent super)
        {
            if (!teamSuper)
            {
                NetworkteamSuper = true;
                NetworknumberOfTimesTransformed += 1;
                teamSuperTimer = teamSuperTimerDuration;
                if (form.consumeItems)
                {
                    itemTracker.RemoveItems(super);
                }
                if (Config.AnnounceSuperTransformation().Value)
                {
                    if (super.body.master && super.body.master.playerCharacterMasterController && super.body.master.playerCharacterMasterController.networkUser)
                    {
                        Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
                        {
                            baseToken = form.name + "_ANNOUNCE_TEXT",
                            subjectAsNetworkUser = super.body.master.playerCharacterMasterController.networkUser,
                            paramTokens = new string[] { Language.GetString(form.name) }
                        });
                    }
                    else
                    {
                        Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
                        {
                            baseToken = form.name + "_ANNOUNCE_TEXT",
                            subjectAsCharacterBody = super.body,
                            paramTokens = new string[] { Language.GetString(form.name) }
                        });
                    }
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
                    NetworkteamSuper = false;
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

        void RemoveItems(SuperSonicComponent super);
    }

    [RequireComponent(typeof(FormHandler))]
    public class SyncedItemTracker : NetworkBehaviour, INeededItemTracker
    {
        public FormHandler handler;
        
        [SyncVar]
        public bool allItems;

        [SyncVar]
        private static byte _serverItemSharing;

        public static FormItemSharing serverItemSharing
        {
            get { return (FormItemSharing)_serverItemSharing; }
        }

        public List<NeededItem> missingItems;

        // This value is only accurate when serverItemSharing is on MajorityRule
        public int highestItemCount;

        public int numNeededItems
        {
            get; private set;
        }

        public bool eventsSubscribed;

        private bool itemsDirty;

        private void OnEnable()
        {
            handler = this.GetComponent<FormHandler>();
            numNeededItems = handler.form.numberOfNeededItems;
            SubscribeEvents(true);
        }

        private void OnDisable()
        {
            SubscribeEvents(false);
        }
        
        public bool ItemRequirementMet(SuperSonicComponent component)
        {
            return allItems && CanTakeSharedItems(handler.form, component);
        }

        public void FixedUpdate()
        {
            if (itemsDirty)
            {
                CheckItems();
            }
        }

        public void CheckItems()
        {
            missingItems = new List<NeededItem>();
            itemsDirty = false;
            if (!handler) { Log.Warning("no handler yet"); return; }
            if (handler.form.neededItems == null) { Log.Warning("Form says it needs items but has no list of needed items... curious..."); return; }
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
                    missing.count = item.count - collectiveItemCount;
                    missingItems.Add(missing);
                }
            }
            NetworkallItems = missingItems.Count == 0;
            Log.Message("Missing items for "+ handler.form.ToString() + ": " + string.Concat(missingItems.Select(x => x.ToString())));
        }
        public bool CanTakeSharedItems(FormDef form, SuperSonicComponent super)
        {
            if (super.formToItemTracker.TryGetValue(form, out ItemTracker itemTracker))
            {
                switch (serverItemSharing)
                {
                    case FormItemSharing.None:
                        return itemTracker.allItems;
                    case FormItemSharing.MajorityRule:
                        return itemTracker.numItemsCollected >= highestItemCount;
                    case FormItemSharing.Contributor:
                        return itemTracker.numItemsCollected > 0;
                    default:
                        return true;
                }
            }
            else
            {
                Log.Error("CanTakeSharedItems run without valid ItemTracker");
                return serverItemSharing == FormItemSharing.All;
            }
        }

        public void CheckHighestItemCount()
        {
            if (serverItemSharing != FormItemSharing.MajorityRule) { return; }
            CheckHighestItemCountArgs = new CheckHighestItemCountEventArgs();
            if (CheckHighestItemCountEvent != null)
            {
                foreach (CheckHighestItemCountEventHandler @event in CheckHighestItemCountEvent.GetInvocationList())
                {
                    @event(CheckHighestItemCountArgs);
                }
            }
            highestItemCount = CheckHighestItemCountArgs.highestItemCount;
            Log.Message("highestItemCount " + highestItemCount);
        }

        public event CheckHighestItemCountEventHandler CheckHighestItemCountEvent;
        public delegate void CheckHighestItemCountEventHandler(CheckHighestItemCountEventArgs e);

        public CheckHighestItemCountEventArgs CheckHighestItemCountArgs;

        public class CheckHighestItemCountEventArgs : EventArgs
        {
            public int highestItemCount;
        }

        public void OnInventoryChanged(Inventory inventory)
        {
            if (inventory.TryGetComponent(out CharacterMaster master))
            {
                if (master.playerCharacterMasterController) // Only check items again if a player's inventory changes
                {
                    SetItemsDirty(); // Only check items once per frame
                }
            }
        }

        public void SetItemsDirty()
        {
            itemsDirty = true;
        }

        public void RemoveItems(SuperSonicComponent super)
        {
            foreach (NeededItem item in handler.form.neededItems)
            {
                int neededItems = item.count;
                if (super.body.master)
                {
                    int numToConstume = Math.Min(super.body.master.inventory.GetItemCount(item), neededItems);
                    neededItems -= numToConstume;
                    super.body.master.inventory.RemoveItem(item, numToConstume);
                    if (neededItems <= 0)
                    {
                        continue;
                    }
                }
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
                    Config.NeededItemSharing().SettingChanged += UpdateFormItemSharingConfig;
                    if (NetworkServer.active)
                    {
                        Network_serverItemSharing = (byte)Config.NeededItemSharing().Value;
                    }
                    Log.Message("Subscribed to inventory events");
                    CheckItems();
                }
                else
                {
                    Inventory.onInventoryChangedGlobal -= OnInventoryChanged;
                    Config.NeededItemSharing().SettingChanged -= UpdateFormItemSharingConfig;
                    eventsSubscribed = false;
                }
            }
        }

        public void UpdateFormItemSharingConfig(object sender, EventArgs args)
        {
            if (NetworkServer.active)
            {
                Network_serverItemSharing = (byte)Config.NeededItemSharing().Value;
                if (Config.NeededItemSharing().Value == FormItemSharing.MajorityRule)
                {
                    CheckHighestItemCount();
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

        public byte Network_serverItemSharing
        {
            get
            {
                return _serverItemSharing;
            }
            [param: In]
            set
            {
                base.SetSyncVar<byte>(value, ref _serverItemSharing, 2U);
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (initialState)
            {
                writer.Write(allItems);
                writer.Write(_serverItemSharing);
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
            if ((base.syncVarDirtyBits & 2U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(_serverItemSharing);
            }
            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                allItems = reader.ReadBoolean();
                _serverItemSharing = reader.ReadByte();
                return;
            }
            int num = (int)reader.ReadPackedUInt32();
            if ((num & 1U) != 0U)
            {
                allItems = reader.ReadBoolean();
            }
            if ((num & 2U) != 0U)
            {
                _serverItemSharing = reader.ReadByte();
                if (serverItemSharing == FormItemSharing.MajorityRule)
                {
                    CheckHighestItemCount();
                }
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

        public void RemoveItems(SuperSonicComponent super)
        {
            if (super.body)
            {
                if (super.body.master)
                {
                    foreach (NeededItem item in handler.form.neededItems)
                    {
                        if (super.body.master.inventory.GetItemCount(item) >= item.count)
                        {
                            super.body.master.inventory.RemoveItem(item, item.count);
                        }
                        else
                        {
                            Log.Warning("Does not have the items to be removed for transforming");
                            super.body.master.inventory.RemoveItem(item, super.body.master.inventory.GetItemCount(item));
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