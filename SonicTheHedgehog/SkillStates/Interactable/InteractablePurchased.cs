using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class InteractablePurchased : EntityState
    {
        public const float dropTime = 0.3f;
        
        public ChaosEmeraldInteractable interactable;

        public Animator animator;

        private bool soundPlayed = false;

        private bool dropped = false;

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Emerald Interactable purchase state enter");
            this.animator = this.gameObject.GetComponentInChildren<Animator>();
            this.interactable = this.gameObject.GetComponent<ChaosEmeraldInteractable>();
            base.gameObject.transform.Find("RingParent/PurchaseParticle").gameObject.GetComponent<ParticleSystem>().Play();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= dropTime/2 && !soundPlayed)
            {
                soundPlayed = true;
                Util.PlaySound("Play_emerald_purchase", base.gameObject);
            }
            if (base.fixedAge >= dropTime && !dropped)
            {
                dropped = true;
                if (!interactable) { return; }
                interactable.DropPickup();
                interactable.Disappear();
            }
        }
    }
}