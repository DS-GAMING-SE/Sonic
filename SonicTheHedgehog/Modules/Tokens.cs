using R2API;
using SonicTheHedgehog.Components;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace SonicTheHedgehog.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region SonicTheHedgehog

            string prefix = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_";

            string desc =
                "Sonic is a fast melee fighter who specializes in movement and single target damage.<color=#CCD3E0>" +
                Environment.NewLine + Environment.NewLine;
            desc = desc +
                   "< ! > Melee is your go-to tool for just about any situation. Use Sonic's close range attacks for a consistent rush of damage. Use homing attacks to close the gap or quickly take down weak enemies." +
                   Environment.NewLine + Environment.NewLine;
            desc = desc +
                   "< ! > Sonic boom is a fast barrage of projectiles. All charges refill at the same time, so you can quickly keep attacking." +
                   Environment.NewLine + Environment.NewLine;
            desc = desc +
                   "< ! > Boost lets you move significantly faster than normal. Use it to dodge attacks or traverse the map." +
                   Environment.NewLine + Environment.NewLine;
            desc = desc +
                   "< ! > Grand Slam is a powerful attack that closes in on an enemy and does major single target damage." +
                   Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, racing off to the next adventure.";
            string outroFailure = "..and so he vanished, as quickly as he arrived.";
            //..and so he left, still treading new ground.
            //..and so he left, onwards to new frontiers.
            //..and so he left, looking for the next thrill.
            //..and so he left, racing off to the next adventure.

            //..and so he vanished, a brilliant light within the abyss.
            //..and so he vanished, finishing his last run.
            //..and so he vanished, as quickly as he arrived.

            LanguageAPI.Add(prefix + "NAME", "Sonic");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Fastest Thing Alive");
            LanguageAPI.Add(prefix + "LORE", "maybe one day there will be a logbook entry");
            //lore idea: something about ancients getting scooped up by providence before their planet got blown up, drew chaos emeralds to ror planet
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins

            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");

            #endregion

            LanguageAPI.Add(prefix + "HOMING_KEYWORD",
                "<style=CKeywordName>Homing</style><style=cSub>Targets the enemy closest to the crosshair, indicated by the blue reticle.");

            string superSonicColor = "<color=#ffee00>";

            #region Passives

            LanguageAPI.Add(prefix + "MOMENTUM_PASSIVE_NAME", "Momentum");
            LanguageAPI.Add(prefix + "MOMENTUM_PASSIVE_DESCRIPTION",
                $"<style=cIsUtility>Build up speed</style> by <style=cIsUtility>running down hill</style> to move up to <style=cIsUtility>{MomentumPassive.speedMultiplier * 100}% faster</style>. <style=cIsHealth>Lose speed by running up hill to move up to {Mathf.Floor((MomentumPassive.speedMultiplier * 100) / 3)}% slower.</style>\nIf <style=cIsUtility>flying</style>, <style=cIsUtility>build up speed</style> by <style=cIsUtility>moving in a straight line.</style>");

            #endregion

            #region Super Prefix

            LanguageAPI.Add(prefix + "SUPER_PREFIX", "Super {0}");

            #endregion

            #region Primary

            LanguageAPI.Add(prefix + "PRIMARY_MELEE_NAME", "Melee");
            string meleeDescription =
                $"Melee nearby enemies dealing <style=cIsDamage>{100f * StaticValues.meleeDamageCoefficient}% damage</style>. Every 5th hit deals <style=cIsDamage>{100f * StaticValues.finalMeleeDamageCoefficient}% damage</style>. Targeting an enemy in the distance will use the <style=cIsUtility>Homing Attack</style>, dealing <style=cIsDamage>{100f * StaticValues.homingAttackDamageCoefficient}% damage</style>.";
            LanguageAPI.Add(prefix + "PRIMARY_MELEE_DESCRIPTION", meleeDescription);

            #endregion

            #region Super Primary

            LanguageAPI.Add(prefix + "SUPER_PRIMARY_MELEE_NAME", $"{superSonicColor}Super Melee</color>");
            LanguageAPI.Add(prefix + "SUPER_PRIMARY_MELEE_DESCRIPTION",
                meleeDescription +
                $" {superSonicColor}Every close range attack fires a projectile dealing {(100f * StaticValues.superMeleeExtraDamagePercent)}% of the attack's damage.</color>");

            #endregion

            #region Sonic Boom

            LanguageAPI.Add(prefix + "SECONDARY_SONIC_BOOM_NAME", "Sonic Boom");
            LanguageAPI.Add(prefix + "SECONDARY_SONIC_BOOM_DESCRIPTION",
                $"Fire shockwaves dealing <style=cIsDamage>{Modules.StaticValues.sonicBoomCount}x{100f * StaticValues.sonicBoomDamageCoefficient}% damage</style>.");

            #endregion

            #region Super Sonic Boom

            LanguageAPI.Add(prefix + "SUPER_SECONDARY_SONIC_BOOM_NAME", $"{superSonicColor}Cross Slash</color>");
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_SONIC_BOOM_DESCRIPTION",
                $"Fire shockwaves dealing <style=cIsDamage>{Modules.StaticValues.sonicBoomCount}x</style>{superSonicColor}{100f * StaticValues.superSonicBoomDamageCoefficient}</color><style=cIsDamage>% damage</style>.");

            #endregion

            #region Parry

            LanguageAPI.Add(prefix + "SECONDARY_PARRY_NAME", "Parry");
            string parryOnHitDescription =
                $"Getting hit in this stance will <style=cIsHealing>negate all damage</style>, give <style=cIsDamage>+{StaticValues.parryAttackSpeedBuff * 100}% attack speed</style>, give <style=cIsUtility>+{StaticValues.parryMovementSpeedBuff * 100}% movement speed</style>, and <style=cIsUtility>reduce</style> all other skill cooldowns by <style=cIsUtility>{StaticValues.parryCooldownReduction}s.</style>";
            LanguageAPI.Add(prefix + "SECONDARY_PARRY_DESCRIPTION",
                $"Enter the <style=cIsUtility>parry stance</style> for a brief period of time. {parryOnHitDescription}");

            #endregion

            #region Super Parry

            LanguageAPI.Add(prefix + "SUPER_SECONDARY_PARRY_NAME", $"{superSonicColor}Perfect Parry</color>");
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_PARRY_DESCRIPTION",
                $"Enter the <style=cIsUtility>parry stance</style> for a {superSonicColor}very brief period of time</color>. {parryOnHitDescription} {superSonicColor}Reduce all nearby enemies attack speed and movement speed by {(1 / StaticValues.superParryAttackSpeedDebuff) * 100}% and armor by {StaticValues.superParryArmorDebuff}. Replace this skill with \"The funny IDW attack, name pending\". \n\nThis can only be triggered once.</color>");

            #endregion

            #region IDW Attack

            LanguageAPI.Add(prefix + "SUPER_SECONDARY_IDW_ATTACK_NAME",
                $"{superSonicColor}IDW Attack (name pending)</color>");
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_IDW_ATTACK_DESCRIPTION",
                $"<style=cIsUtility>Homing</style>. With an incredible display of speed, repeatedly deal <style=cIsDamage>{StaticValues.idwAttackDamageCoefficient * 100f}% damage</style> in a large area. \n\n{superSonicColor}This can only be triggered once.</color>");

            #endregion

            #region Boost

            LanguageAPI.Add(prefix + "UTILITY_BOOST_NAME", "Boost");
            string boostDescription =
                $"Spend boost meter to <style=cIsUtility>move {100f * StaticValues.boostListedSpeedCoefficient}% faster</style> than normal. If <style=cIsDamage>health</style> is <style=cIsDamage>near full</style>, <style=cIsUtility>move {100f * StaticValues.powerBoostListedSpeedCoefficient}% faster</style> instead. If airborne, do a short <style=cIsUtility>mid-air dash</style>.";
            LanguageAPI.Add(prefix + "UTILITY_BOOST_DESCRIPTION", boostDescription);

            #endregion

            #region Scepter Boost

            LanguageAPI.Add(prefix + "SCEPTER_UTILITY_BOOST_NAME", $"Thundering Boost");
            string scepterBoostDescription = Helpers.ScepterDescription(
                $"Run into enemies to deal {StaticValues.scepterBoostDamageCoefficient * 100f}% damage. Damage increases based on your movement speed.");
            LanguageAPI.Add(prefix + "SCEPTER_UTILITY_BOOST_DESCRIPTION", boostDescription + scepterBoostDescription);

            #endregion

            #region Super Boost

            LanguageAPI.Add(prefix + "SUPER_UTILITY_BOOST_NAME", $"{superSonicColor}Super Boost</color>");
            string superBoostDescription =
                $"<style=cIsUtility>Move {superSonicColor}{100f * StaticValues.superBoostListedSpeedCoefficient}%</color> faster</style> than normal.";
            LanguageAPI.Add(prefix + "SUPER_UTILITY_BOOST_DESCRIPTION", superBoostDescription);

            #endregion

            #region Super Scepter Boost

            LanguageAPI.Add(prefix + "SUPER_SCEPTER_UTILITY_BOOST_NAME",
                $"{superSonicColor}Super Thundering Boost</color>");
            LanguageAPI.Add(prefix + "SUPER_SCEPTER_UTILITY_BOOST_DESCRIPTION",
                superBoostDescription + scepterBoostDescription);

            #endregion

            #region Special

            LanguageAPI.Add(prefix + "SPECIAL_GRAND_SLAM_NAME", "Grand Slam");
            string grandSlamDescription =
                $"<style=cIsUtility>Homing</style>. Dash forward into an enemy to attack with <style=cIsDamage>{100f * StaticValues.grandSlamSpinDamageCoefficient}% damage</style> repeatedly before unleashing a powerful attack from above dealing <style=cIsDamage>{100f * StaticValues.grandSlamFinalDamageCoefficient}% damage</style>.";
            LanguageAPI.Add(prefix + "SPECIAL_GRAND_SLAM_DESCRIPTION", grandSlamDescription);

            #endregion

            #region Super Special

            LanguageAPI.Add(prefix + "SUPER_SPECIAL_GRAND_SLAM_NAME", $"{superSonicColor}Super Grand Slam</color>");
            LanguageAPI.Add(prefix + "SUPER_SPECIAL_GRAND_SLAM_DESCRIPTION",
                grandSlamDescription +
                $"{superSonicColor} Create afterimages that rain down from the sky dealing {100f * (Modules.StaticValues.superGrandSlamDOTDamage * 3)}% damage per second.</color>");

            #endregion

            #region Special #2

            LanguageAPI.Add(prefix + "SPECIAL_SUPER_TRANSFORMATION_NAME", $"{superSonicColor}Super Sonic</color>");
            LanguageAPI.Add(prefix + "SPECIAL_SUPER_TRANSFORMATION_DESCRIPTION",
                $"Transform into {superSonicColor}Super Sonic</color> for {Modules.StaticValues.superSonicDuration} seconds. {superSonicColor}Upgrades all of your skills</color>. Increases <style=cIsDamage>damage</style> by <style=cIsDamage>+{100f * StaticValues.superSonicBaseDamage}%</style>. Increases <style=cIsDamage>attack speed</style> by <style=cIsDamage>+{100f * StaticValues.superSonicAttackSpeed}%</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>+{100f * StaticValues.superSonicMovementSpeed}%</style>. Gain <style=cIsHealing>complete invincibility</style> and <style=cIsUtility>flight</style>." +
                Environment.NewLine + Environment.NewLine + "This can only be used once per stage.");

            #endregion

            #region Achievements

            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Sonic: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC",
                "As Sonic, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Sonic: Mastery");

            LanguageAPI.Add("ACHIEVEMENT_" + SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE_NAME",
                "Sonic: Spinning Upside Down");
            LanguageAPI.Add(
                "ACHIEVEMENT_" + SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE_DESCRIPTION",
                $"As Sonic, hit {Achievements.SonicHomingAttackAirborneAchievement.countRequired} different enemies with the homing attack without touching the ground.");

            #endregion

            #region Emeralds
            
            LanguageAPI.Add(prefix + "EMERALD_TEMPLE", "Chaos Temple thingy");

            LanguageAPI.Add(prefix + "YELLOW_EMERALD", "Chaos Emerald Yellow");
            LanguageAPI.Add(prefix + "YELLOW_EMERALD_PICKUP", "One out of 7, silly Yellow!");
            LanguageAPI.Add(prefix + "YELLOW_EMERALD_DESC", "One of the seven chaos emeralds which allow Sonic to become Super Sonic!");
            
            LanguageAPI.Add(prefix + "BLUE_EMERALD", "Chaos Emerald Blue");
            LanguageAPI.Add(prefix + "BLUE_EMERALD_PICKUP", "One out of 7, relaxing Blue!");
            LanguageAPI.Add(prefix + "BLUE_EMERALD_DESC", "One of the seven chaos emeralds which allow Sonic to become Super Sonic!");
            
            LanguageAPI.Add(prefix + "RED_EMERALD", "Chaos Emerald Red");
            LanguageAPI.Add(prefix + "RED_EMERALD_PICKUP", "One out of 7, anger Red!");
            LanguageAPI.Add(prefix + "RED_EMERALD_DESC", "One of the seven chaos emeralds which allow Sonic to become Super Sonic!");
            
            LanguageAPI.Add(prefix + "GRAY_EMERALD", "Chaos Emerald Gray");
            LanguageAPI.Add(prefix + "GRAY_EMERALD_PICKUP", "One out of 7, depressing Gray!");
            LanguageAPI.Add(prefix + "GRAY_EMERALD_DESC", "One of the seven chaos emeralds which allow Sonic to become Super Sonic!");
            
            LanguageAPI.Add(prefix + "GREEN_EMERALD", "Chaos Emerald Green");
            LanguageAPI.Add(prefix + "GREEN_EMERALD_PICKUP", "One out of 7, peaceful Green!");
            LanguageAPI.Add(prefix + "GREEN_EMERALD_DESC", "One of the seven chaos emeralds which allow Sonic to become Super Sonic!");
            
            LanguageAPI.Add(prefix + "CYAN_EMERALD", "Chaos Emerald Cyan");
            LanguageAPI.Add(prefix + "CYAN_EMERALD_PICKUP", "One out of 7, mind numbing Cyan!");
            LanguageAPI.Add(prefix + "CYAN_EMERALD_DESC", "One of the seven chaos emeralds which allow Sonic to become Super Sonic!");
            
            LanguageAPI.Add(prefix + "PURPLE_EMERALD", "Chaos Emerald Purple");
            LanguageAPI.Add(prefix + "PURPLE_EMERALD_PICKUP", "One out of 7, even more silly Purple!");
            LanguageAPI.Add(prefix + "PURPLE_EMERALD_DESC", "One of the seven chaos emeralds which allow Sonic to become Super Sonic!");

            #endregion

            #endregion
        }
    }
}