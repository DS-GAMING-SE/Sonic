using EntityStates;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Skills;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using SonicTheHedgehog.Modules.Forms;
using SonicTheHedgehog.SkillStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;

namespace SonicTheHedgehog.Components
{
    public class SuperSonicComponent : NetworkBehaviour
    {
        public EntityStateMachine superSonicState;

        [Tooltip("The form you have selected. Not necessarily the form you are currently in, but the one that you're focused on. Attempting to transform will transform you into this form.")]
        public FormDef targetedForm;

        [Tooltip("The form you're currently in. If not transformed into anything, this will be null.")]
        public FormDef activeForm;

        public Material formMaterial;
        public Material defaultMaterial;

        public Mesh formMesh;
        public Mesh defaultModel;

        private CharacterBody body;
        private CharacterModel model;
        private Animator modelAnimator;

        public Dictionary<FormDef, ItemTracker> formToItemTracker = new Dictionary<FormDef, ItemTracker>();

        private void Start()
        {
            body = base.GetComponent<CharacterBody>();
            if (!body.isPlayerControlled)
            {
                Destroy(this);
            }
            model = body.modelLocator.modelTransform.gameObject.GetComponent<CharacterModel>();
            modelAnimator = model.transform.GetComponent<Animator>();
            superSonicState = EntityStateMachine.FindByCustomName(base.gameObject, "SonicForms");

            CreateUnsyncItemTrackers();
        }

        public void CreateUnsyncItemTrackers()
        {
            foreach (FormDef form in FormCatalog.formsCatalog)
            {
                if (form.requiresItems && !form.shareItems)
                {
                    CreateTrackerForForm(form);
                }
            }
        }

        public virtual void CreateTrackerForForm(FormDef form)
        {
            ItemTracker itemTracker = body.gameObject.AddComponent<ItemTracker>();
            itemTracker.form = form;
            itemTracker.body = body;
            formToItemTracker.Add(form, itemTracker);
        }

        public void FixedUpdate()
        {
            if (body.hasAuthority && body.isPlayerControlled) // Adding isPlayerControlled I guess fixed super transforming all Sonics
            {
                DecideTargetForm();
                if (targetedForm != null && activeForm != targetedForm)
                {
                    if (Forms.formToHandlerObject.TryGetValue(targetedForm, out GameObject handlerObject))
                    {
                        FormHandler handler = handlerObject.GetComponent(typeof(FormHandler)) as FormHandler;
                        if (handler.CanTransform(this))
                        {
                            Log.Message("Attempt Transform");
                            Transform();
                        }
                    }
                }
            }
        }

        public void DecideTargetForm()
        {
            targetedForm = null;
            foreach (FormDef form in FormCatalog.formsCatalog)
            {
                if (form.keybind.Value.IsDown())
                {
                    targetedForm = form;
                    break;
                }
            }
        }

        public void Transform()
        {
            EntityStateMachine bodyState = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            if (!bodyState) { return; }
            if (!Forms.formToHandlerObject.TryGetValue(targetedForm, out GameObject handlerObject)) { return; }
            FormHandler handler = handlerObject.GetComponent(typeof(FormHandler)) as FormHandler;
            TransformationBase transformState = (TransformationBase)EntityStateCatalog.InstantiateState(targetedForm.transformState.stateType);
            transformState.fromTeamSuper = handler.teamSuper;
            if (bodyState.SetInterruptState(transformState, InterruptPriority.Frozen))
            {
                if (NetworkServer.active)
                {
                    //FormHandler.instance.OnTransform();
                    handler.OnTransform(base.gameObject);
                }
                else
                {
                    new SuperSonicTransform(GetComponent<NetworkIdentity>().netId, targetedForm.formIndex).Send(NetworkDestination.Server);
                }
            }
        }

        public void SetNextForm(FormDef form)
        {
            if (form != null)
            {
                SonicFormBase formState = (SonicFormBase)EntityStateCatalog.InstantiateState(form.formState.stateType);
                formState.form = form;
                this.superSonicState.SetNextState(formState);
            }
            else
            {
                this.superSonicState.SetNextStateToMain();
            }
        }

        public void OnTransform(FormDef form)
        {
            this.activeForm = form;
            if (!form) { return; }
            ModelSkinController skin = model.GetComponentInChildren<ModelSkinController>();
            if (!skin) { return; }
            if (skin.skins.Length > body.skinIndex) // heretic causing errors without this check
            {
                GetSuperModel(skin.skins[body.skinIndex].nameToken);
                SuperModel();
            }
        }

        public void TransformEnd()
        {
            if (body.HasBuff(activeForm.buff))
            {
                if (activeForm.duration > 0)
                {
                    body.RemoveOldestTimedBuff(activeForm.buff);
                }
                else
                {
                    body.RemoveBuff(activeForm.buff);
                }
            }
            this.activeForm = null;
            ResetModel();
        }

        // Thank you DxsSucuk
        public void SuperModel()
        {
            defaultMaterial = model.baseRendererInfos[0].defaultMaterial; // Textures
            if (formMaterial)
            {
                model.baseRendererInfos[0].defaultMaterial = formMaterial;
            }
            
            if (modelAnimator && activeForm.superAnimations) // Animations
            {
                modelAnimator.SetFloat("isSuperFloat", 1f);
            }

            if (formMesh) // Model
            {
                defaultModel = model.mainSkinnedMeshRenderer.sharedMesh;
                model.mainSkinnedMeshRenderer.sharedMesh = formMesh;
            }

            model.materialsDirty = true;
        }

        public void ResetModel()
        {
            model.baseRendererInfos[0].defaultMaterial = defaultMaterial; // Textures

            if (modelAnimator) // Animations
            {
                modelAnimator.SetFloat("isSuperFloat", 0f);
            }

            if (formMesh) // Model
            {
                model.mainSkinnedMeshRenderer.sharedMesh = defaultModel;
            }

            model.materialsDirty = true;
        }

        public virtual void GetSuperModel(string skinName)
        {
            if (activeForm.renderDictionary.TryGetValue(skinName, out RenderReplacements replacements))
            {
                formMesh = replacements.mesh;
                formMaterial = replacements.material;
            }
            else
            {
                if (defaultModel)
                {
                    formMesh = defaultModel;
                }
                if (defaultMaterial)
                {
                    formMaterial = defaultMaterial;
                }
            }


            /*switch (skinName)
            {
                case SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME":
                    formMaterial = Materials.CreateHopooMaterial("matSuperSonic");
                    formMesh = Assets.mainAssetBundle.LoadAsset<GameObject>("SuperSonicMesh")
                        .GetComponent<SkinnedMeshRenderer>().sharedMesh;
                    break;
                case SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME":
                    formMaterial = Materials.CreateHopooMaterial("matSuperMetalSonic");
                    formMesh = null;
                    break;
            }*/
        }
    }

    public class ItemTracker : MonoBehaviour
    {
        public FormDef form;

        public CharacterBody body;

        public Inventory inventory;

        private bool itemsDirty;

        public bool allItems;

        private bool eventsSubscribed;

        // HOW DO I GET THE INVENTORY?!?!?
        private void OnDisable()
        {
            SubscribeEvents(false);
        }

        private void FixedUpdate()
        {
            if (!eventsSubscribed)
            {
                if (body)
                {
                    if (body.inventory)
                    {
                        Log.Message("inventory found");
                        inventory = body.inventory;
                        SubscribeEvents(true);
                    }
                }
            }
            else
            {
                if (itemsDirty)
                {
                    CheckItems();
                }
            }
        }

        public void SubscribeEvents(bool subscribe)
        {
            if (inventory)
            {
                if (eventsSubscribed ^ subscribe)
                {
                    if (subscribe)
                    {
                        inventory.onInventoryChanged += SetItemsDirty;
                        Log.Message("subscribe");
                        eventsSubscribed = true;
                        SetItemsDirty();
                    }
                    else
                    {
                        inventory.onInventoryChanged -= SetItemsDirty;
                        eventsSubscribed = false;
                    }
                }
            }
        }

        public void SetItemsDirty()
        {
            itemsDirty = true;
        }

        public void CheckItems() // You can tell how much suffering part of code has brought its writer by seeing how many logs there are
        {
            itemsDirty = false;
            if (!form) { Log.Error("No form??"); allItems= false; return; }
            if (!inventory) { Log.Error("No inventory????????"); allItems = false; return; }
            foreach (NeededItem item in form.neededItems)
            {
                if (item == ItemIndex.None) { Log.Error("No item????????"); return; }
                if (inventory.GetItemCount(item) < item.count)
                {
                    allItems = false;
                    Log.Message("Missing items for " + form.ToString() + ": \n" + (new NeededItem { item = item.item, count = (uint)(item.count - inventory.GetItemCount(item))}).ToString());
                    return;
                }
            }
            Log.Message("All items needed for " + form.ToString());
            allItems = true;
        }
    }
}