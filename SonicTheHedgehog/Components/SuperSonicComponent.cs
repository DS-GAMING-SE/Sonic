using EntityStates;
using RoR2;
using RoR2.Skills;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.SkillStates;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.Components
{
    public class SuperSonicComponent : NetworkBehaviour
    {
        public EntityStateMachine superSonicState;

        public Material superSonicMaterial;
        public Material defaultMaterial;

        public Mesh superSonicModel;
        public Mesh defaultModel;

        private CharacterBody body;
        private CharacterModel model;
        private Animator modelAnimator;
        private TemporaryOverlay temporaryOverlay;

        public static SkillDef melee;

        public static SkillDef sonicBoom;
        public static SkillDef parry;
        public static SkillDef idwAttack;
        public static SkillDef emptyParry;

        public static SkillDef boost;

        public static SkillDef grandSlam;

        private bool canTransform = true;

        private void Start()
        {
            body = GetComponent<CharacterBody>();
            model = body.modelLocator.modelTransform.gameObject.GetComponent<CharacterModel>();
            modelAnimator = model.transform.GetComponent<Animator>();
            superSonicState = EntityStateMachine.FindByCustomName(base.gameObject, "SonicForms");
            Stage.onServerStageBegin += ResetSuperSonic;
            Inventory.onInventoryChangedGlobal += OnInventoryChanged;
        }

        public void FixedUpdate()
        {
        }

        private void OnDestroy()
        {
            Stage.onServerStageBegin -= ResetSuperSonic;
            Inventory.onInventoryChangedGlobal -= OnInventoryChanged;
        }

        public void Transform(EntityStateMachine entityState, Inventory inventory)
        {
            if (entityState.SetInterruptState(new SuperSonicTransformation(), InterruptPriority.Frozen))
            {
                canTransform = false;
                RemoveEmeralds(inventory);
            }
        }

        public bool CanTransform(Inventory inventory)
        {
            bool hasYellow = inventory.GetItemCount(Items.yellowEmerald) > 0;
            bool hasRed = inventory.GetItemCount(Items.redEmerald) > 0;
            bool hasBlue = inventory.GetItemCount(Items.blueEmerald) > 0;
            bool hasCyan = inventory.GetItemCount(Items.cyanEmerald) > 0;
            bool hasGreen = inventory.GetItemCount(Items.greenEmerald) > 0;
            bool hasGray = inventory.GetItemCount(Items.grayEmerald) > 0;
            bool hasPurple = inventory.GetItemCount(Items.purpleEmerald) > 0;

            return hasYellow && hasRed && hasBlue && hasCyan && hasGreen && hasGray && hasPurple && canTransform;
        }

        public void RemoveEmeralds(Inventory inventory)
        {
            inventory.RemoveItem(Items.yellowEmerald);
            inventory.RemoveItem(Items.redEmerald);
            inventory.RemoveItem(Items.blueEmerald);
            inventory.RemoveItem(Items.cyanEmerald);
            inventory.RemoveItem(Items.greenEmerald);
            inventory.RemoveItem(Items.grayEmerald);
            inventory.RemoveItem(Items.purpleEmerald);
        }

        public void TransformEnd()
        {
            body.skillLocator.secondary.UnsetSkillOverride(this, SuperSonicComponent.idwAttack,
                GenericSkill.SkillOverridePriority.Contextual);
            body.skillLocator.secondary.UnsetSkillOverride(this, SuperSonicComponent.emptyParry,
                GenericSkill.SkillOverridePriority.Contextual);
            canTransform = true;
            ResetModel();
        }

        // Thank you DxsSucuk
        public void SuperModel()
        {
            defaultMaterial = model.baseRendererInfos[0].defaultMaterial; // Textures
            model.baseRendererInfos[0].defaultMaterial = superSonicMaterial;
            
            if (modelAnimator) // Animations
            {
                modelAnimator.SetFloat("isSuperFloat", 1f);
            }

            if (superSonicModel) // Model
            {
                defaultModel = model.mainSkinnedMeshRenderer.sharedMesh;
                model.mainSkinnedMeshRenderer.sharedMesh = superSonicModel;
            }

            if (model) // Outline
            {
                temporaryOverlay = model.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.originalMaterial = Assets.superSonicOverlay;
                temporaryOverlay.enabled = true;
                temporaryOverlay.AddToCharacerModel(model);
            }
        }

        public void ResetModel()
        {
            model.baseRendererInfos[0].defaultMaterial = defaultMaterial; // Textures

            if (modelAnimator) // Animations
            {
                modelAnimator.SetFloat("isSuperFloat", 0f);
            }

            if (superSonicModel) // Model
            {
                model.mainSkinnedMeshRenderer.sharedMesh = defaultModel;
            }

            if (temporaryOverlay) // Outline
            {
                temporaryOverlay.RemoveFromCharacterModel();
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

        public void ResetSuperSonic(Stage stage)
        {
            // awaitStageReset = false;
        }

        public void OnInventoryChanged(Inventory inventory)
        {
            // No longer needed?
        }
    }
}