using System;

namespace SonicTheHedgehog.Modules
{
    internal static class StaticValues
    {
        internal static string descriptionText = "Sonic is a fast melee fighter who specializes in movement and single target damage.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Homing attack lets you quickly close the distance between you and an enemy, letting you quickly rack up damage with melee attacks." + Environment.NewLine + Environment.NewLine
             + "< ! > Sonic Boom is a fast projectile that lets you attack from a distance." + Environment.NewLine + Environment.NewLine
             + "< ! > Boost lets you move significantly faster than normal. Use it to dodge attacks or traverse the map." + Environment.NewLine + Environment.NewLine
             + "< ! > Grand Slam is a powerful single target attack that's perfect for taking down bosses." + Environment.NewLine + Environment.NewLine;
        // Melee

        internal const float homingAttackDamageCoefficient = 6f;

        internal const float meleeDamageCoefficient = 2f;

        internal const float finalMeleeDamageCoefficient = 6f;

        internal const float meleeBaseSpeed = 0.35f;

        internal const float finalMeleeBaseSpeed = 0.75f;

        internal const float ballArmor = 100f;

        internal const float meleeProcCoefficient = 1f;

        internal const float homingAttackProcCoefficient = 1f;

        // Sonic Boom

        internal const float sonicBoomCount = 2;

        internal const float sonicBoomFireTime = 0.19f;
        
        internal const float sonicBoomDamageCoefficient = 1.6f;

        internal const float sonicBoomProcCoefficient = 0.5f;

        // Parry

        internal const float parryMinimumDuration = 0.5f;

        internal const float parryMaximumDuration = 1f;

        internal const float parryEndLag = 0.6f;

        internal const float parryFailEndLag = 0.7f;

        internal const float parryCooldownReduction = 3f;

        internal const float parryBoostRecharge = 15f;

        internal const float parryLingeringInvincibilityDuration = 1.7f;

        internal const float parryBuffDuration = 3f;

        internal const float parryAttackSpeedBuff = 0.4f;

        internal const float parryMovementSpeedBuff = 0.3f;

        // Boost

        internal const float boostListedSpeedCoefficient = 0.35f;

        internal const float powerBoostListedSpeedCoefficient = 0.65f;

        internal const float boostSpeedFlatCoefficient = 8.5f * 0.175f;
        
        internal const float boostSpeedCoefficient = 0.15f;

        internal const float powerBoostSpeedFlatCoefficient = 8.5f * 0.35f;

        internal const float powerBoostSpeedCoefficient = 0.2305f;

        internal const float boostArmor = 50f;

        // Scepter Boost

        internal const float scepterBoostDamageCoefficient = 9f;

        internal const float scepterBoostProcCoefficient = 1f;

        internal const float defaultPowerBoostSpeed = 24f;

        internal const float scepterBoostICD = 1f;
        
        // Grand Slam

        internal const float grandSlamSpinDamageCoefficient = 1.4f;

        internal const float grandSlamFinalDamageCoefficient = 22f;

        internal const float grandSlamSpinProcCoefficient = 0.5f;

        internal const float grandSlamFinalProcCoefficient = 1.5f;

        // Chaos Emeralds --------------------------

        internal const int chaosEmeraldCost = 50;

        internal const int chaosEmeraldsMaxPerStage = 3;

        internal const int chaosEmeraldsMaxPerStageSimulacrum = 5;

        // Super Sonic -----------------------------

        internal const float superSonicDuration = 50f;
        
        internal const float superSonicMovementSpeed = 1f; // value multiplied by base move speed stat

        internal const float superSonicAttackSpeed = 0.3f;

        internal const float superSonicBaseDamage = 0.7f;

        internal const float superSonicJumpHeight = 0.5f;

        // Super Melee

        internal const float superMeleeExtraDamagePercent = 0.4f;

        internal const float superMeleeExtraProcCoefficient = 0f;

        // Super Sonic Boom

        internal const float superSonicBoomDamageCoefficient = sonicBoomDamageCoefficient*2;

        // Super Parry

        internal const float superParryMaxDuration = 0.4f;

        internal const float superParryMovementSpeedDebuff = 2f; //half

        internal const float superParryArmorDebuff = 100f;

        internal const float superParryAttackSpeedDebuff = 2f; //half

        internal const float superParryDebuffDuration = 10;

        // IDW Attack

        internal const float idwAttackattackDuration = 2f;

        internal const float idwAttackDamageCoefficient = 8f;

        internal const float idwAttackProcCoefficient = 0.7f;

        // Super Sonic Boost

        internal const float superBoostListedSpeedCoefficient = 1f;

        internal const float superBoostSpeedFlatCoefficient = 8.5f * 0.5f;

        internal const float superBoostSpeedCoefficient = 0.33f;

        // Super Sonic Grand Slam

        internal const float superGrandSlamDOTDamage = 6f;

        internal const float superGrandSlamDOTProcCoefficient = 0.3f;

        internal const float superGrandSlamDOTLifetime = 6f;

    }
}