using EntityStates;
using RoR2;
using RoR2.Audio;
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
        public void ResetSuperSonic(Stage stage)
        {
            
        }
        public void OnInventoryChanged(Inventory inventory)
        {

        }
    }
}