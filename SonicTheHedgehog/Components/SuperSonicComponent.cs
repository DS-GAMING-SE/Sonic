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

        public FormDef form;

        public Material formMaterial;
        public Material defaultMaterial;

        public Mesh formMesh;
        public Mesh defaultModel;

        private CharacterBody body;
        private CharacterModel model;
        private Animator modelAnimator;

        private TemporaryOverlay temporaryOverlay;
        private TemporaryOverlay flashOverlay;
        private static Material flashMaterial;

        public static SkillDef melee;

        public static SkillDef sonicBoom;
        public static SkillDef parry;
        public static SkillDef idwAttack;
        public static SkillDef emptyParry;

        public static SkillDef boost;

        public static SkillDef grandSlam;

        public Dictionary<FormDef, ItemTracker> formToItemTracker = new Dictionary<FormDef, ItemTracker>();

        private void Start()
        {
            body = GetComponent<CharacterBody>();
            model = body.modelLocator.modelTransform.gameObject.GetComponent<CharacterModel>();
            modelAnimator = model.transform.GetComponent<Animator>();
            superSonicState = EntityStateMachine.FindByCustomName(base.gameObject, "SonicForms");
            flashMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashBright.mat").WaitForCompletion();

            CreateUnsyncItemTrackers();
        }

        public void CreateUnsyncItemTrackers()
        {
            foreach (FormDef form in Forms.formsCatalog)
            {
                if (form.neededItems.Count()>0 && !form.shareItems)
                {
                    CreateTrackerForForm(form);
                }
            }
        }

        public virtual void CreateTrackerForForm(FormDef form)
        {
            ItemTracker itemTracker = body.gameObject.AddComponent<ItemTracker>();
            itemTracker.form = form;
            formToItemTracker.Add(form, itemTracker);
        }

        public void FixedUpdate()
        {
            if (body.hasAuthority && body.isPlayerControlled && form != Forms.superSonicDef) // Adding isPlayerControlled I guess fixed super transforming all Sonics
            {
                if (Config.SuperTransformKey().Value.IsPressed())
                {
                    if (Forms.formToHandlerObject.TryGetValue(Forms.superSonicDef, out GameObject handlerObject))
                    {
                        FormHandler handler = handlerObject.GetComponent(typeof(FormHandler)) as FormHandler;
                        if (handler.CanTransform(this))
                        {
                            Debug.Log("Attempt Super Transform");
                            Transform();
                        }
                    }
                }
            }
        }

        public void Transform()
        {
            EntityStateMachine bodyState = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            if (!bodyState) { return; }
            if (!Forms.formToHandlerObject.TryGetValue(Forms.superSonicDef, out GameObject handlerObject)) { return; }
            FormHandler handler = handlerObject.GetComponent(typeof(FormHandler)) as FormHandler;
            if (bodyState.SetInterruptState(new SuperSonicTransformation { emeraldAnimation = !handler.NetworkteamSuper }, InterruptPriority.Frozen))
            {
                if (NetworkServer.active)
                {
                    //FormHandler.instance.OnTransform();
                    handler.OnTransform(base.gameObject);
                }
                else
                {
                    new SuperSonicTransform(GetComponent<NetworkIdentity>().netId, Forms.superSonicDef.formIndex).Send(NetworkDestination.Server);
                }
            }
        }

        public void OnTransform(FormDef form)
        {
            this.form = form;
            if (!form) return;
            GetSuperModel(model.GetComponentInChildren<ModelSkinController>().skins[body.skinIndex].nameToken);
            SuperModel();
        }

        public void TransformEnd()
        {
            this.form = null;
            body.skillLocator.secondary.UnsetSkillOverride(this, SuperSonicComponent.idwAttack,
                GenericSkill.SkillOverridePriority.Contextual);
            body.skillLocator.secondary.UnsetSkillOverride(this, SuperSonicComponent.emptyParry,
                GenericSkill.SkillOverridePriority.Contextual);
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
            
            if (modelAnimator && form.superAnimations) // Animations
            {
                modelAnimator.SetFloat("isSuperFloat", 1f);
            }

            if (formMesh) // Model
            {
                defaultModel = model.mainSkinnedMeshRenderer.sharedMesh;
                model.mainSkinnedMeshRenderer.sharedMesh = formMesh;
            }

            if (model)
            {
                temporaryOverlay = model.gameObject.AddComponent<TemporaryOverlay>(); // Outline
                temporaryOverlay.originalMaterial = Assets.superSonicOverlay;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.enabled = true;
                temporaryOverlay.AddToCharacerModel(model);


                flashOverlay = model.gameObject.AddComponent<TemporaryOverlay>(); // Flash
                flashOverlay.duration = 1;
                flashOverlay.animateShaderAlpha = true;
                flashOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0.7f, 1f, 0f);
                flashOverlay.originalMaterial = flashMaterial;
                flashOverlay.destroyComponentOnEnd = true;
                flashOverlay.AddToCharacerModel(model);
            }
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

            if (temporaryOverlay) // Outline
            {
                temporaryOverlay.RemoveFromCharacterModel();
            }

            if (model) // Flash
            {
                flashOverlay = model.gameObject.AddComponent<TemporaryOverlay>();
                flashOverlay.duration = 0.35f;
                flashOverlay.animateShaderAlpha = true;
                flashOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                flashOverlay.originalMaterial = flashMaterial;
                flashOverlay.destroyComponentOnEnd = true;
                flashOverlay.AddToCharacerModel(model);
            }
        }

        public void ParryActivated()
        {
            body.skillLocator.secondary.SetSkillOverride(this, SuperSonicComponent.idwAttack,
                GenericSkill.SkillOverridePriority.Contextual);
        }

        public void IDWAttackActivated()
        {
            body.skillLocator.secondary.UnsetSkillOverride(this, SuperSonicComponent.idwAttack,
                GenericSkill.SkillOverridePriority.Contextual);
            body.skillLocator.secondary.SetSkillOverride(this, SuperSonicComponent.emptyParry,
                GenericSkill.SkillOverridePriority.Contextual);
        }

        public virtual void GetSuperModel(string skinName)
        {
            if (form.renderDictionary.TryGetValue(skinName, out RenderReplacements replacements))
            {
                formMesh = replacements.mesh;
                formMaterial = replacements.material;
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
        
        public Inventory inventory;

        public bool allItems;

        private bool eventsSubscribed;
        
        private void Start()
        {
            inventory = GetComponent<CharacterBody>().inventory;
        }

        private void OnDisable()
        {
            SubscribeEvents(false);
        }

        public void SubscribeEvents(bool subscribe)
        {
            if (eventsSubscribed ^ subscribe)
            {
                if (subscribe)
                {
                    inventory.onInventoryChanged += CheckItems;
                    eventsSubscribed = true;
                    CheckItems();
                }
                else
                {
                    inventory.onInventoryChanged -= CheckItems;
                    eventsSubscribed = false;
                }
            }
        }

        public void CheckItems()
        {
            foreach (NeededItem item in form.neededItems)
            {
                if (inventory.GetItemCount(item) < item.count)
                {
                    allItems = false;
                    return;
                }
            }
            allItems = true;
        }
    }
}