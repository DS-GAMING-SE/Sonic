using IL.RoR2.Achievements;
using RoR2;
using SonicTheHedgehog.SkillStates;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SonicTheHedgehog.Modules.Achievements
{
    [RegisterAchievement(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICPARRYUNLOCK", "SonicSkills.Parry", null, null)]
    public class SonicHomingAttackAirborneAchievement : RoR2.Achievements.BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("SonicTheHedgehog");
        }

        // Hit countRequired different enemies with a homing attack without touching the ground (Unlocks Parry)
        private int count;
        private readonly List<HealthComponent> hitEnemies = new List<HealthComponent>();
        private static int countRequired = 10;

        public override void OnInstall()
        {
            base.OnInstall();
            base.localUser.cachedBody.characterMotor.onHitGroundAuthority += OnHitGround;
            HomingAttack.onAuthorityHitEnemy += OnHitEnemy;
        }

        public override void OnUninstall()
        {
            base.localUser.cachedBody.characterMotor.onHitGroundAuthority -= OnHitGround;
            HomingAttack.onAuthorityHitEnemy -= OnHitEnemy;
            base.OnUninstall();
        }

        private void OnHitGround(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            this.count = 0;
            this.hitEnemies.Clear();
        }

        private void OnHitEnemy(HomingAttack state, HurtBox hurtBox)
        {
            if (!this.hitEnemies.Contains(hurtBox.healthComponent))
            {
                this.count += 1;
                if (this.count >= countRequired)
                {
                    base.Grant();
                }
            }
        }
    }
}