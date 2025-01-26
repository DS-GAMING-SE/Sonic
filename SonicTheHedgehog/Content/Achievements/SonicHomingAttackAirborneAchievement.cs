﻿using RoR2.Achievements;
using RoR2;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SonicTheHedgehog.Modules.Achievements
{
    // I have no god damn clue what SonicSkills.Parry is or what a reward identifier is I just ignore it and hope it works
    [RegisterAchievement(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE", "SonicSkills.Parry", null, 3)]
    public class SonicHomingAttackAirborneAchievement : BaseAchievement
    {
        // Hit countRequired different enemies with a homing attack without touching the ground (Unlocks Parry)
        private int count;
        private readonly List<HealthComponent> hitEnemies = new List<HealthComponent>();
        public static int countRequired = 8;

        private static float pityTime = 0.3f;

        private float pityTimer;

        private EntityStateMachine bodyStateMachine;


        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            this.bodyStateMachine = EntityStateMachine.FindByCustomName(base.localUser.cachedBody.gameObject, "Body");
            RoR2Application.onFixedUpdate += OnFixedUpdate;
        }

        public override void OnBodyRequirementBroken()
        {
            base.OnBodyRequirementBroken();
            RoR2Application.onFixedUpdate -= OnFixedUpdate;
        }

        public override void OnInstall()
        {
            base.OnInstall();
            HomingAttack.onAuthorityHitEnemy += OnHitEnemy;
        }

        public override void OnUninstall()
        {
            HomingAttack.onAuthorityHitEnemy -= OnHitEnemy;
            base.OnUninstall();
        }

        private void OnFixedUpdate()
        {
            if (base.localUser.cachedBody && base.localUser.cachedBody.characterMotor.isGrounded && !typeof(HomingAttack).IsAssignableFrom(this.bodyStateMachine.state.GetType()))
            {
                this.pityTimer += Time.fixedDeltaTime;
                if (this.pityTimer >= pityTime && count > 0)
                {
                    this.count = 0;
                    this.hitEnemies.Clear();
                }
            }
        }

        private void OnHitEnemy(HomingAttack state, HurtBox hurtBox)
        {
            if (!this.hitEnemies.Contains(hurtBox.healthComponent))
            {
                this.count += 1;
                this.hitEnemies.Add(hurtBox.healthComponent);
                if (count == 1)
                {
                    this.pityTimer = 0;
                }
                Log.Message("Homing attack achievement progress: "+this.count.ToString());
                if (this.count >= countRequired)
                {
                    base.Grant();
                }
            }
        }
    }
}