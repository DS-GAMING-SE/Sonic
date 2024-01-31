using EntityStates;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Skills;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using SonicTheHedgehog.SkillStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        private TemporaryOverlay flashOverlay;
        private static Material flashMaterial;

        public static SkillDef melee;

        public static SkillDef sonicBoom;
        public static SkillDef parry;
        public static SkillDef idwAttack;
        public static SkillDef emptyParry;

        public static SkillDef boost;

        public static SkillDef grandSlam;

        private void Start()
        {
            body = GetComponent<CharacterBody>();
            model = body.modelLocator.modelTransform.gameObject.GetComponent<CharacterModel>();
            modelAnimator = model.transform.GetComponent<Animator>();
            superSonicState = EntityStateMachine.FindByCustomName(base.gameObject, "SonicForms");
            GetSuperModel();
            flashMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashBright.mat").WaitForCompletion();
        }

        public void Transform(EntityStateMachine entityState, Inventory inventory)
        {
            if (entityState.SetInterruptState(new SuperSonicTransformation(), InterruptPriority.Frozen))
            {
                if (NetworkServer.active)
                {
                    SuperSonicHandler.instance.OnTransform();
                }
                else
                {
                    new SuperSonicTransform(GetComponent<NetworkIdentity>().netId).Send(NetworkDestination.Server);
                }
            }
        }

        public void TransformEnd()
        {
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
            if (superSonicMaterial)
            {
                model.baseRendererInfos[0].defaultMaterial = superSonicMaterial;
            }
            
            if (modelAnimator) // Animations
            {
                modelAnimator.SetFloat("isSuperFloat", 1f);
            }

            if (superSonicModel) // Model
            {
                defaultModel = model.mainSkinnedMeshRenderer.sharedMesh;
                model.mainSkinnedMeshRenderer.sharedMesh = superSonicModel;
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

            if (superSonicModel) // Model
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

        public virtual void GetSuperModel()
        {
            string skinName = model.GetComponentInChildren<ModelSkinController>().skins[body.skinIndex].nameToken;
            switch (skinName)
            {
                case SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME":
                    superSonicMaterial = Materials.CreateHopooMaterial("matSuperSonic");
                    superSonicModel = Assets.mainAssetBundle.LoadAsset<GameObject>("SuperSonicMesh")
                        .GetComponent<SkinnedMeshRenderer>().sharedMesh;
                    break;
                case SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME":
                    superSonicMaterial = Materials.CreateHopooMaterial("matSuperMetalSonic");
                    superSonicModel = null;
                    break;
            }
        }
    }
}