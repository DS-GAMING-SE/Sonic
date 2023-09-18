﻿using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class SuperSonic : BaseState
    {
        SuperSonicComponent superSonicComponent;
        Material prevMaterial;
        UnityEngine.Object superAura;
        bool superBuffApplied;
        CharacterModel model;
        public override void OnEnter()
        {
            base.OnEnter();
            superSonicComponent = base.GetComponent<SuperSonicComponent>();
            //model = base.characterBody.modelLocator.GetComponent<CharacterModel>();

            Transform transform = base.gameObject.transform;
            this.superAura = UnityEngine.Object.Instantiate<UnityEngine.Object>(Modules.Assets.superSonicAura, base.FindModelChild("MainHurtbox"));
            //this.superAura = PrefabAPI.InstantiateClone(Modules.Assets.superSonicAura, "SuperAura", true);

            //this.model.baseRendererInfos[0].defaultMaterial = Assets.mainAssetBundle.LoadAsset<Material>("matSuperSonic");
            UpdateFlight(true);

            if (base.isAuthority)
            {
                if (base.characterBody.healthComponent)
                {
                    ProcChainMask proc = default(ProcChainMask);
                    proc.AddProc(ProcType.RepeatHeal);
                    proc.AddProc(ProcType.CritHeal);
                    base.characterBody.healthComponent.HealFraction(1, proc);
                }
                if (base.skillLocator)
                {
                    base.skillLocator.primary.SetSkillOverride(this, SuperSonicComponent.melee, GenericSkill.SkillOverridePriority.Upgrade);
                    base.skillLocator.secondary.SetSkillOverride(this, SuperSonicComponent.sonicBoom, GenericSkill.SkillOverridePriority.Upgrade);
                    base.skillLocator.utility.SetSkillOverride(this, SuperSonicComponent.boost, GenericSkill.SkillOverridePriority.Upgrade);
                    base.skillLocator.special.SetSkillOverride(this, SuperSonicComponent.grandSlam, GenericSkill.SkillOverridePriority.Upgrade);
                }

                EffectManager.SimpleMuzzleFlash(Modules.Assets.superSonicTransformationEffect, base.gameObject, "MainHurtbox", true);
            }

            if (NetworkServer.active)
            {
                RoR2.Util.CleanseBody(base.characterBody, true, false, true, true, true, false);
                base.characterBody.AddTimedBuff(Modules.Buffs.superSonicBuff, Modules.StaticValues.superSonicDuration+1, 1);
                base.characterBody.AddTimedBuff(RoR2Content.Buffs.Immune, Modules.StaticValues.superSonicDuration+1, 1);
            }

            superSonicComponent.UpdateModel();
        }

        public override void OnExit()
        {
            UpdateFlight(false);
            if (this.superAura)
            {
                Destroy(this.superAura);
            }
            else
            {
                //Chat.AddMessage("why does aura despawn after 12 seconds that's so random");
            }

            superSonicComponent.ResetModel();

            if (base.isAuthority && base.skillLocator)
            {
                base.skillLocator.primary.UnsetSkillOverride(this, SuperSonicComponent.melee, GenericSkill.SkillOverridePriority.Upgrade);
                base.skillLocator.secondary.UnsetSkillOverride(this, SuperSonicComponent.sonicBoom, GenericSkill.SkillOverridePriority.Upgrade);
                base.skillLocator.utility.UnsetSkillOverride(this, SuperSonicComponent.boost, GenericSkill.SkillOverridePriority.Upgrade);
                base.skillLocator.special.UnsetSkillOverride(this, SuperSonicComponent.grandSlam, GenericSkill.SkillOverridePriority.Upgrade);
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterBody.HasBuff(Modules.Buffs.superSonicBuff))
            {
                if (!superBuffApplied)
                {
                    superBuffApplied = true;
                }
            }
            else if (superBuffApplied)
            {
                if (superSonicComponent)
                {
                    superSonicComponent.superSonicState.SetNextState(new BaseSonic());
                }
                return;
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
    }
}