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

        public override void OnEnter()
        {
            base.OnEnter();
            this.superAura = GameObject.Instantiate<GameObject>(Modules.Assets.superSonicAura, base.FindModelChild("Chest"));

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
            }

            base.cameraTargetParams.RemoveParamsOverride(this.camOverrideHandle, 0.5f);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.superAura.SetActive(this.characterModel.invisibilityCount <= 0);
            if (base.fixedAge >= StaticValues.superSonicDuration - StaticValues.superSonicWarningDuration && !warning)
            {
                this.warning = GameObject.Instantiate<GameObject>(Modules.Assets.superSonicWarning, base.FindModelChild("Chest"));
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