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

        // Sonic Boom

        internal const float sonicBoomCount = 2;

        internal const float sonicBoomFireTime = 0.19f;
        
        internal const float sonicBoomDamageCoefficient = 1.6f;

        // Boost

        internal const float boostSpeedCoefficient = 0.35f;

        internal const float powerBoostSpeedCoefficient = 0.65f;

        internal const float boostArmor = 50f;
        
        // Grand Slam

        internal const float grandSlamDashDamageCoefficient = 1.4f;

        internal const float grandSlamFinalDamageCoefficient = 22f;

        // Super Sonic -----------------------------

        internal const float superSonicDuration = 50f;
        
        internal const float superSonicMovementSpeed = 0.8f;

        internal const float superSonicAttackSpeed = 0.3f;

        internal const float superSonicBaseDamage = 0.7f;

        internal const float superSonicJumpHeight = 0.5f;

        // Super Melee

        internal const float superMeleeExtraDamagePercent = 0.6f;

        // Super Sonic Boom

        internal const float superSonicBoomDamageCoefficient = sonicBoomDamageCoefficient*2;

        // Super Sonic Boost

        internal const float superBoostSpeedCoefficient = 2f;

        // Super Sonic Grand Slam

        internal const float superGrandSlamDOTDamage = 3f;

    }
}