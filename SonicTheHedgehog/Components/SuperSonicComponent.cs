using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
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
        private CharacterModel model;

        public static SkillDef melee;

        public static SkillDef sonicBoom;
        public static SkillDef parry;

        public static SkillDef boost;

        public static SkillDef grandSlam;


        public bool canTransform=true;


        private void Start()
        {
            body = GetComponent<CharacterBody>();
            model = body.modelLocator.modelTransform.gameObject.GetComponent<CharacterModel>();
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

        // Thank you DxsSucuk
        public void SuperModel()
        {
            defaultMaterial = model.baseRendererInfos[0].defaultMaterial;
            model.baseRendererInfos[0].defaultMaterial = superSonicMaterial;
        }

        public void ResetModel()
        {
            model.baseRendererInfos[0].defaultMaterial = defaultMaterial;
        }

        public void ResetSuperSonic(Stage stage)
        {
            
        }
        public void OnInventoryChanged(Inventory inventory)
        {

        }
    }
}