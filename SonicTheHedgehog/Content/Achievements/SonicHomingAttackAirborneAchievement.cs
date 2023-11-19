using IL.RoR2.Achievements;
using RoR2;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SonicTheHedgehog.Modules.Achievements
{
    [RegisterAchievement(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE", "SonicSkills.Parry", null, null)]
    public class SonicHomingAttackAirborneAchievement : RoR2.Achievements.BaseAchievement
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
            if (base.localUser.cachedBody && base.localUser.cachedBody.characterMotor.isGrounded && this.bodyStateMachine.state.GetType() != typeof(HomingAttack))
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
                this.pityTimer = 0;
                Debug.Log(this.count.ToString());
                if (this.count >= countRequired)
                {
                    base.Grant();
                }
            }
        }
    }
}