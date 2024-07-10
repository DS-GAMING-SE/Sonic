using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class SuperSonic : SonicFormBase
    {
        protected CharacterModel model;

        protected Transform chest;

        private TemporaryOverlay temporaryOverlay;

        private GameObject superAura;
        private GameObject warning;
        private LoopSoundManager.SoundLoopPtr superLoop;

        private static float cameraDistance = -15;
        private CharacterCameraParamsData cameraParams = new CharacterCameraParamsData
        {
            maxPitch = 70f,
            minPitch = -70f,
            pivotVerticalOffset = 1.3f,
            idealLocalCameraPos = new Vector3(0f, 0f, cameraDistance),
            wallCushion = 0.1f
        };
        private CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;

        public static SkillDef melee;

        public static SkillDef sonicBoom;
        public static SkillDef parry;
        public static SkillDef idwAttack;
        public static SkillDef emptyParry;

        public static SkillDef boost;

        public static SkillDef grandSlam;

        public override void OnEnter()
        {
            base.OnEnter();
            chest = base.FindModelChild("Chest");
            if (chest)
            {
                this.superAura = GameObject.Instantiate<GameObject>(Modules.Assets.superSonicAura, base.FindModelChild("Chest"));
            }

            ApplyOutline();

            superLoop = LoopSoundManager.PlaySoundLoopLocal(base.gameObject, Assets.superLoopSoundDef);

            this.camOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = this.cameraParams,
                priority = 0f
            }, 0f);

            if (base.isAuthority)
            {
                FireBlastAttack();

                Heal(1);

                if (base.skillLocator)
                {
                    SkillOverrides(true);
                }
                EffectManager.SimpleMuzzleFlash(Modules.Assets.superSonicTransformationEffect, base.gameObject, "Chest", true);
            }
            if (NetworkServer.active)
            {
                RoR2.Util.CleanseBody(base.characterBody, true, false, true, true, true, false);
            }

        }

        public override void OnExit()
        {
            LoopSoundManager.StopSoundLoopLocal(superLoop);

            if (temporaryOverlay)
            {
                temporaryOverlay.RemoveFromCharacterModel();
            }
            
            if (this.superAura)
            {
                Destroy(this.superAura);
            }
            // Aura had despawning problem because all assets loaded are automatically given a component that makes them go away after 12 seconds
            if (this.warning)
            {
                Destroy(this.warning);
            }

            if (base.isAuthority && base.skillLocator)
            {
                SkillOverrides(false);
                base.skillLocator.secondary.UnsetSkillOverride(this, emptyParry, GenericSkill.SkillOverridePriority.Contextual);
            }

            base.cameraTargetParams.RemoveParamsOverride(this.camOverrideHandle, 0.5f);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.superAura)
            {
                this.superAura.SetActive(this.characterModel.invisibilityCount <= 0);
            }
            if (base.fixedAge >= StaticValues.superSonicDuration - StaticValues.superSonicWarningDuration && !warning && chest)
            {
                this.warning = GameObject.Instantiate<GameObject>(Modules.Assets.superSonicWarning, chest);
            }
        }

        public virtual void SkillOverrides(bool set)
        {
            if (!base.skillLocator) { return; }
            SkillHelper(base.skillLocator.primary, SonicTheHedgehogCharacter.primarySkillDef, melee, set);
            if (!SkillHelper(base.skillLocator.secondary, SonicTheHedgehogCharacter.sonicBoomSkillDef, sonicBoom, set))
            {
                SkillHelper(base.skillLocator.secondary, SonicTheHedgehogCharacter.parrySkillDef, parry, set);
            }
            SkillHelper(base.skillLocator.utility, SonicTheHedgehogCharacter.boostSkillDef, boost, set);
            SkillHelper(base.skillLocator.special, SonicTheHedgehogCharacter.grandSlamSkillDef, grandSlam, set);
        }

        protected bool SkillHelper(GenericSkill slot, SkillDef original, SkillDef upgrade, bool set)
        {
            if (slot)
            {
                if (slot.baseSkill == original)
                {
                    if (set)
                    {
                        slot.SetSkillOverride(this, upgrade, GenericSkill.SkillOverridePriority.Upgrade);
                        return true;
                    }
                    else
                    {
                        slot.UnsetSkillOverride(this, upgrade, GenericSkill.SkillOverridePriority.Upgrade);
                        return true;
                    }
                }
            }
            return false;
        }
        public void ParryActivated()
        {
            base.skillLocator.secondary.SetSkillOverride(this, idwAttack, GenericSkill.SkillOverridePriority.Contextual);
        }

        public void IDWAttackActivated()
        {
            base.skillLocator.secondary.UnsetSkillOverride(this, idwAttack, GenericSkill.SkillOverridePriority.Contextual);
            base.skillLocator.secondary.SetSkillOverride(this, emptyParry, GenericSkill.SkillOverridePriority.Contextual);
        }

        private void ApplyOutline()
        {
            if (base.characterBody.modelLocator)
            {
                if (base.characterBody.modelLocator.modelTransform)
                {
                    model = base.characterBody.modelLocator.modelTransform.gameObject.GetComponent<CharacterModel>();
                }
            }

            if (model)
            {
                temporaryOverlay = model.gameObject.AddComponent<TemporaryOverlay>(); // Outline
                temporaryOverlay.originalMaterial = Assets.superSonicOverlay;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.enabled = true;
                temporaryOverlay.AddToCharacerModel(model);
            }
        }

        private void FireBlastAttack()
        {
            if (base.isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 20;
                blastAttack.procCoefficient = 0;
                blastAttack.position = base.transform.position;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = false;
                blastAttack.baseDamage = 0;
                blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                blastAttack.damageType = DamageType.Generic;
                blastAttack.baseForce = 7000;
                blastAttack.teamIndex = base.teamComponent.teamIndex;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blastAttack.Fire();
            }
        }
    }
}