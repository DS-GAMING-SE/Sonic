using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using SonicTheHedgehog.Modules.Forms;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace SonicTheHedgehog.SkillStates
{
    public class SonicFormBase : BaseState
    {
        public FormDef form;

        protected SuperSonicComponent superSonicComponent;

        protected bool buffApplied;

        protected CharacterModel characterModel;

        private TemporaryOverlay flashOverlay;
        private static Material flashMaterial;

        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                this.characterModel = modelTransform.GetComponent<CharacterModel>();
            }

            superSonicComponent = base.GetComponent<SuperSonicComponent>();

            superSonicComponent.OnTransform(form);

            if (form.flight)
            {
                UpdateFlight(true);
            }

            AddBuff();

        }

        public virtual void AddBuff()
        {
            if (NetworkServer.active)
            {
                if (form.duration <= 0)
                {
                    base.characterBody.AddBuff(form.buff);
                }
                else
                {
                    base.characterBody.AddTimedBuff(form.buff, form.duration, 1);
                }
            }
        }

        public override void OnExit()
        {
            if (form.flight)
            {
                UpdateFlight(false);
            }
            superSonicComponent.TransformEnd();
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterBody.HasBuff(form.buff))
            {
                if (!buffApplied)
                {
                    buffApplied = true;
                }
            }
            else if (buffApplied)
            {
                if (superSonicComponent)
                {
                    superSonicComponent.superSonicState.SetNextState(new BaseSonic());
                }
                return;
            }
        }

        public virtual void Heal(float healFraction)
        {
            if (base.characterBody.healthComponent && base.isAuthority)
            {
                ProcChainMask proc = default(ProcChainMask);
                proc.AddProc(ProcType.RepeatHeal);
                proc.AddProc(ProcType.CritHeal);
                base.characterBody.healthComponent.HealFraction(healFraction, proc);
            }
        }

        protected void Flash(float duration)
        {
            if (characterModel)
            {
                if (!flashMaterial)
                {
                    flashMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashBright.mat").WaitForCompletion();
                }

                flashOverlay = characterModel.gameObject.AddComponent<TemporaryOverlay>(); // Flash
                flashOverlay.duration = duration;
                flashOverlay.animateShaderAlpha = true;
                flashOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0.7f, 1f, 0f);
                flashOverlay.originalMaterial = flashMaterial;
                flashOverlay.destroyComponentOnEnd = true;
                flashOverlay.AddToCharacerModel(characterModel);
            }
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

        private void UpdateFlight(bool flying)
        {
            if (base.characterBody.GetComponent<ICharacterFlightParameterProvider>() != null)
            {
                CharacterFlightParameters flightParameters = base.characterBody.GetComponent<ICharacterFlightParameterProvider>().flightParameters;
                flightParameters.channeledFlightGranterCount += flying ? 1 : -1;
                base.characterBody.GetComponent<ICharacterFlightParameterProvider>().flightParameters = flightParameters;
            }
            if (base.characterBody.GetComponent<ICharacterGravityParameterProvider>() != null)
            {
                CharacterGravityParameters gravityParameters = base.characterBody.GetComponent<ICharacterGravityParameterProvider>().gravityParameters;
                gravityParameters.channeledAntiGravityGranterCount += flying ? 1 : -1;
                base.characterBody.GetComponent<ICharacterGravityParameterProvider>().gravityParameters = gravityParameters;
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(form.formIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            form = Forms.GetFormDef(reader.ReadFormIndex());
        }
    }
}