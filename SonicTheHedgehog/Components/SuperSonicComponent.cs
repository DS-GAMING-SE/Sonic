using EntityStates;
using RoR2;
using RoR2.Skills;
using SonicTheHedgehog.SkillStates;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.Components
{
    public class SuperSonicComponent : NetworkBehaviour
    {
        public EntityStateMachine superSonicState;
        public Material superSonicMaterial;
        public Material defaultMaterial;
        private CharacterBody body;
        private CharacterModel characterModel;

        public static SkillDef melee;
        public static SkillDef sonicBoom;
        public static SkillDef boost;
        public static SkillDef grandSlam;

        public bool canTransform = true;


        private void Start()
        {
            body = GetComponent<CharacterBody>();
            characterModel = body.modelLocator.modelTransform.gameObject.GetComponent<CharacterModel>();
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

        public void Transform(EntityStateMachine entityState)
        {
            if (entityState.SetInterruptState(new SuperSonicTransformation(), InterruptPriority.Frozen))
            {
                canTransform = false;
            }
        }

        public void UpdateModel()
        {
            characterModel.baseRendererInfos[0].defaultMaterial = superSonicMaterial;
        }

        public void ResetModel()
        {
            characterModel.baseRendererInfos[0].defaultMaterial = defaultMaterial;
        }

        public void ResetSuperSonic(Stage stage)
        {
            canTransform = true;
        }

        public void OnInventoryChanged(Inventory inventory)
        {

        }
    }
}