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
        private CharacterBody body;

        public static SkillDef melee;
        public static SkillDef sonicBoom;
        public static SkillDef boost;
        public static SkillDef grandSlam;

        public bool canTransform=false;


        private void Start()
        {
            body = GetComponent<CharacterBody>();
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
        public void ResetSuperSonic(Stage stage)
        {
            
        }
        public void OnInventoryChanged(Inventory inventory)
        {

        }
    }
}