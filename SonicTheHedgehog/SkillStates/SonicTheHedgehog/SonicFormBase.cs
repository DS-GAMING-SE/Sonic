﻿using EntityStates;
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

namespace SonicTheHedgehog.SkillStates
{
    public class SonicFormBase : BaseState
    {
        SuperSonicComponent superSonicComponent;

        public FormDef formDef;

        GameObject aura;
        GameObject warning;
        LoopSoundManager.SoundLoopPtr superLoop;

        bool buffApplied;

        CharacterModel characterModel;

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

        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                this.characterModel = modelTransform.GetComponent<CharacterModel>();
            }

            superSonicComponent = base.GetComponent<SuperSonicComponent>();

            this.aura = GameObject.Instantiate<GameObject>(Modules.Assets.superSonicAura, base.FindModelChild("Chest"));

            superSonicComponent.SuperModel();

            superLoop = LoopSoundManager.PlaySoundLoopLocal(base.gameObject, Assets.superLoopSoundDef);

            if (formDef.flight)
            {
                UpdateFlight(true);
            }

            this.camOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = this.cameraParams,
                priority = 0f
            }, 0f);

            if (base.isAuthority)
            {
                FireBlastAttack();

                if (base.characterBody.healthComponent)
                {
                    ProcChainMask proc = default(ProcChainMask);
                    proc.AddProc(ProcType.RepeatHeal);
                    proc.AddProc(ProcType.CritHeal);
                    base.characterBody.healthComponent.HealFraction(1, proc);
                }
                if (base.skillLocator)
                {
                    SkillOverrides(true);
                }
                EffectManager.SimpleMuzzleFlash(Modules.Assets.superSonicTransformationEffect, base.gameObject, "Chest", true);
            }
            if (NetworkServer.active)
            {
                RoR2.Util.CleanseBody(base.characterBody, true, false, true, true, true, false);
                if (formDef.duration <= 0)
                base.characterBody.AddTimedBuff(formDef.buff, formDef.duration+1, 1);
            }

        }

        public virtual void AddBuff()
        {
            if (formDef.duration <= 0)
            {
                base.characterBody.AddBuff(formDef.buff);
            }
            else
            {
                base.characterBody.AddTimedBuff(formDef.buff, formDef.duration, 1);
            }
        }

        public override void OnExit()
        {
            if (formDef.flight)
            {
                UpdateFlight(false);
            }

            superSonicComponent.TransformEnd();

            LoopSoundManager.StopSoundLoopLocal(superLoop);

            if (this.aura)
            {
                Destroy(this.aura);
            }
            // Aura despawned because all assets loaded are automatically given a component that makes them go away after 12 seconds. Why no one tells me this
            if (this.warning)
            {
                Destroy(this.warning);
            }

            if (base.isAuthority && base.skillLocator)
            {
                SkillOverrides(false);
            }

            base.cameraTargetParams.RemoveParamsOverride(this.camOverrideHandle, 0.5f);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.aura)
            {
                this.aura.SetActive(this.characterModel.invisibilityCount <= 0);
            }
            if (base.fixedAge >= formDef.duration - StaticValues.superSonicWarningDuration && !warning)
            {
                this.warning = GameObject.Instantiate<GameObject>(Modules.Assets.superSonicWarning, base.FindModelChild("Chest"));
            }
            if (base.characterBody.HasBuff(formDef.buff))
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

        public virtual void SkillOverrides(bool set)
        {
            if (set)
            {
                if (base.skillLocator.primary.baseSkill == SonicTheHedgehogCharacter.primarySkillDef)
                {
                    base.skillLocator.primary.SetSkillOverride(this, SuperSonicComponent.melee, GenericSkill.SkillOverridePriority.Upgrade);
                }

                if (base.skillLocator.secondary.baseSkill == SonicTheHedgehogCharacter.sonicBoomSkillDef)
                {
                    base.skillLocator.secondary.SetSkillOverride(this, SuperSonicComponent.sonicBoom, GenericSkill.SkillOverridePriority.Upgrade);
                }
                else if (base.skillLocator.secondary.baseSkill == SonicTheHedgehogCharacter.parrySkillDef)
                {
                    base.skillLocator.secondary.SetSkillOverride(this, SuperSonicComponent.parry, GenericSkill.SkillOverridePriority.Upgrade);
                }

                if (base.skillLocator.utility.baseSkill == SonicTheHedgehogCharacter.boostSkillDef)
                {
                    base.skillLocator.utility.SetSkillOverride(this, SuperSonicComponent.boost, GenericSkill.SkillOverridePriority.Upgrade);
                }

                if (base.skillLocator.special.baseSkill == SonicTheHedgehogCharacter.grandSlamSkillDef)
                {
                    base.skillLocator.special.SetSkillOverride(this, SuperSonicComponent.grandSlam, GenericSkill.SkillOverridePriority.Upgrade);
                }
            }
            else
            {
                if (base.skillLocator.primary.baseSkill == SonicTheHedgehogCharacter.primarySkillDef)
                {
                    base.skillLocator.primary.UnsetSkillOverride(this, SuperSonicComponent.melee, GenericSkill.SkillOverridePriority.Upgrade);
                }

                if (base.skillLocator.secondary.baseSkill == SonicTheHedgehogCharacter.sonicBoomSkillDef)
                {
                    base.skillLocator.secondary.UnsetSkillOverride(this, SuperSonicComponent.sonicBoom, GenericSkill.SkillOverridePriority.Upgrade);
                }
                else
                {
                    base.skillLocator.secondary.UnsetSkillOverride(this, SuperSonicComponent.parry, GenericSkill.SkillOverridePriority.Upgrade);
                }

                if (base.skillLocator.utility.baseSkill == SonicTheHedgehogCharacter.boostSkillDef)
                {
                    base.skillLocator.utility.UnsetSkillOverride(this, SuperSonicComponent.boost, GenericSkill.SkillOverridePriority.Upgrade);
                }

                if (base.skillLocator.special.baseSkill == SonicTheHedgehogCharacter.grandSlamSkillDef)
                {
                    base.skillLocator.special.UnsetSkillOverride(this, SuperSonicComponent.grandSlam, GenericSkill.SkillOverridePriority.Upgrade);
                }
            }
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