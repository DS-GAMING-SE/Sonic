using R2API;
using SonicTheHedgehog.Components;
using System;
using System.Drawing;
using System.Text;
using System.Xml.Linq;
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

            LanguageAPI.Add(prefix + "LORE", "<style=cMono>UES Planetary Observation Network Logs\nVessel: Contact Light</style>\n\n>Welcome captain. Please enter log id…\n>082303\n\n[LOG 082303]\nSummation: Third observation of unknown organic entity 072391 by PON Control AI.\n[07:32:15:76] - [Satellite M2Y]: high speed mass detected in atmosphere; Rallypoint Epsilon potentially within initial estimated trajectory; issuing intercontinental bombardment warning to Epsilon; time to impact <9.35 seconds.\r\n[07:32:23:22] - [Satellite M2Y]: Trajectory has abruptly shifted 86.7 degrees at speeds exceeding atmospheric re-entry levels; rescinding warning and requesting additional observation; potential enemy aircraft.\n[07:32:34:62] - [Satellite T3I]: Target is moving less than 20 feet above sea level; speed exceeds 19,300 mph.\n[07:32:38:17] - [PON Control]: 072391 suspected; initiating capture protocol; observation drones S2, S6 and BR to convene at attached waypoint; observation drones T2, K3 and AC to pursue and direct target toward waypoint.\n[07:32:38:23] - [Drones AC/BR/K3/S2/S6/T2]: Orders received; moving out.\n[07:33:14:33] - [Drone T2]: Visual established; confirmed 072391; target traveling across the northern ocean’s surface; appears to be running on the water; assuming high-speed formation with K3 and AC; pursuing.\n[07:33:27:86] - [Drone T2]: Target’s speed exceeds onboard thrusters; target approaching limit of camera range.\n[07:33:29:02] - [PON Control]: Override 767 issued; continue pursuit.\n[07:33:31:07] - [Drones AC/K3/T2]: Overclocking thrusters; speed exceeding 19,500 mph.\n[07:33:34:21] - [Drones AC/K3/T2] : WARNING; thrusters overheating - explosion imminent.\n[07:33:40:87] - [Drone T2]: Re-established visual; target appears to be running backwards; target appears to be winking at this unit.\n[07:33:44:43] - [Drone T2]: Massive increase to target’s speed; target exceeding camera range; shockwave incoming. \n[07:33:45:14] - [Drones AC/K3/T2]: WARNING thrusters’ structural integrity compromised; internal combustion detected; losing altitude.\n[07:33:46:14] - [Drones AC/K3/T2]: WARNING waterlogging detected; systems failing.\n[07:33:47:01] -  [AC/K3/T2 LOST]\n[07:33:49:65] - [PON Control]: Target approaching intercept point; speed exceeding 20,000 mph; drones prepare stun field.\n[07:33:51:01] - [Drones BR/S2/S6]: Stun emitters deployed; ready to fire.\n[07:33:53:13] - [PON Control]: Massive increase to target’s speed; tracking lost; estimated time to intercept point <2 seconds; fire stun field.\n[07:33:54:01] - [Drones BR/S2/S6]: ERROR: Stun emitters missing.\n[07:33:55:23] -  [Drone S2] Audio parsed: \"You’re.\"\n[07:33:55:25] -  [S2 LOST]\n[07:33:56:37] -  [Drone BR] Audio parsed: \"Too.\"\n[07:33:56:40] -  [BR LOST]\n[07:33:57:33] -  [Drone S6] Audio parsed: \"Slow.\"\n[07:33:57:36] -  [S6 LOST]\n[07:34:01:02] -  [TARGET LOST]\n\n<style=cMono>[END LOG]</style>\n\n>Officer commentary appended to case file: \"He’s definitely messing with us.\"");
            //lore idea: something about ancients getting scooped up by providence before their planet got blown up, drew chaos emeralds to ror planet

            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins

            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Metal");

            #endregion

            LanguageAPI.Add(prefix + "HOMING_KEYWORD",
                "<style=CKeywordName>Homing</style><style=cSub>Targets the enemy closest to the crosshair, indicated by the blue reticle.");

            // Use HedgehogUtils.Helpers.SuperFormText() for this
            string superSonicColor = "<color=#ffee00>";

            #region Primary

            LanguageAPI.Add(prefix + "PRIMARY_MELEE_NAME", "Melee");
            string meleeDescription =
                $"Melee nearby enemies dealing <style=cIsDamage>{100f * StaticValues.meleeDamageCoefficient}% damage</style>. Every 5th hit deals <style=cIsDamage>{100f * StaticValues.finalMeleeDamageCoefficient}% damage</style>. Targeting an enemy in the distance will use the <style=cIsUtility>Homing Attack</style>, dealing <style=cIsDamage>{100f * StaticValues.homingAttackDamageCoefficient}% damage</style>.";
            LanguageAPI.Add(prefix + "PRIMARY_MELEE_DESCRIPTION", meleeDescription + " This move can <style=cIsUtility>launch</style> lightweight enemies.");

            #endregion

            #region Super Primary

            LanguageAPI.Add(prefix + "SUPER_PRIMARY_MELEE_NAME", $"{superSonicColor}Super Melee</color>");
            LanguageAPI.Add(prefix + "SUPER_PRIMARY_MELEE_DESCRIPTION",
                meleeDescription +
                $" This move can <style=cIsUtility>launch</style> {superSonicColor}mediumweight</color> enemies.\n{superSonicColor}Every close range attack fires a projectile dealing {(100f * StaticValues.superMeleeExtraDamagePercent)}% of the attack's damage.</color>");

            #endregion

            #region Sonic Boom

            LanguageAPI.Add(prefix + "SECONDARY_SONIC_BOOM_NAME", "Sonic Boom");
            LanguageAPI.Add(prefix + "SECONDARY_SONIC_BOOM_DESCRIPTION",
                $"Fire shockwaves dealing <style=cIsDamage>{Modules.StaticValues.sonicBoomCount}x{100f * StaticValues.sonicBoomDamageCoefficient}% damage</style> and reducing <style=cIsDamage>armor</style> by <style=cIsDamage>{Modules.StaticValues.sonicBoomCount}x{StaticValues.sonicBoomDebuffArmorReduction}</style>.");

            #endregion

            #region Super Sonic Boom

            LanguageAPI.Add(prefix + "SUPER_SECONDARY_SONIC_BOOM_NAME", $"{superSonicColor}Cross Slash</color>");
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_SONIC_BOOM_DESCRIPTION",
                $"Fire shockwaves dealing <style=cIsDamage>{Modules.StaticValues.sonicBoomCount}x</style>{superSonicColor}{100f * StaticValues.superSonicBoomDamageCoefficient}</color><style=cIsDamage>% damage</style> and reducing <style=cIsDamage>armor</style> by <style=cIsDamage>{Modules.StaticValues.sonicBoomCount}x</style>{superSonicColor}{StaticValues.superSonicBoomDebuffArmorReduction}</color>.");

            #endregion

            #region Parry

            LanguageAPI.Add(prefix + "SECONDARY_PARRY_NAME", "Parry");
            string parryOnHitDescription =
                $"Getting hit in this stance will <style=cIsHealing>negate all damage</style>, give +{StaticValues.parryAttackSpeedBuff * 100}% <style=cIsDamage>attack speed</style> and <style=cIsUtility>movement speed</style>, and <style=cIsUtility>reduce</style> all other skill cooldowns by <style=cIsUtility>{StaticValues.parryCooldownReduction}s.</style>";
            LanguageAPI.Add(prefix + "SECONDARY_PARRY_DESCRIPTION",
                $"Press or hold to enter the <style=cIsUtility>parry stance</style> for a brief period of time. {parryOnHitDescription} It will also temporarily <style=cIsUtility>replace this skill</style> with \"<style=cIsUtility>Follow Up</style>\", a wide <style=cIsUtility>launching</style> kick attack.");

            #endregion

            #region Follow Up
            LanguageAPI.Add(prefix + "SECONDARY_PARRY_FOLLOW_UP_NAME", "Follow Up");
            LanguageAPI.Add(prefix + "SECONDARY_PARRY_FOLLOW_UP_DESCRIPTION",
                $"Perform a wide kick dealing <style=cIsDamage>{StaticValues.followUpDamageCoefficient * 100f}%</style> and <style=cIsUtility>launching</style> mediumweight hit enemies.");

            #endregion

            #region Super Follow Up
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_PARRY_FOLLOW_UP_NAME", $"{superSonicColor}Super Follow Up</color>");
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_PARRY_FOLLOW_UP_DESCRIPTION",
                $"Perform a wide kick dealing <style=cIsDamage>{StaticValues.followUpDamageCoefficient * 100f}%</style> and <style=cIsUtility>launching</style> {superSonicColor}heavyweight</color> hit enemies.");

            #endregion

            #region Super Parry
            string idwAttackName = $"IDW Attack";
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_PARRY_NAME", $"{superSonicColor}Perfect Parry</color>");
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_PARRY_DESCRIPTION",
                $"Enter the <style=cIsUtility>parry stance</style> for a {superSonicColor}very brief period of time</color>. {parryOnHitDescription} {superSonicColor}It will also reduce all nearby enemies' attack speed and movement speed by {(1 / StaticValues.superParryAttackSpeedDebuff) * 100}%, reduce armor by {StaticValues.superParryArmorDebuff}, and replace this skill with \"{idwAttackName}\" which deals damage in an area around the targeted enemy. \n\nThis can only be triggered once.</color>");

            #endregion

            #region IDW Attack
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_IDW_ATTACK_NAME",
                $"{superSonicColor}{idwAttackName}</color>");
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_IDW_ATTACK_DESCRIPTION",
                $"<style=cIsUtility>Homing</style>. With an incredible display of speed, repeatedly deal <style=cIsDamage>{StaticValues.idwAttackDamageCoefficient * 100f}% damage</style> in a large area around the targeted enemy. \n\n{superSonicColor}This can only be triggered once.</color>");

            #endregion

            #region Boost

            LanguageAPI.Add(prefix + "UTILITY_BOOST_NAME", "Boost");
            string boostDescription =
                $"Spend boost meter to <style=cIsUtility>move {100f * StaticValues.boostListedSpeedCoefficient}% faster</style> than normal. If <style=cIsDamage>health</style> is <style=cIsDamage>near full</style>, <style=cIsUtility>move {100f * StaticValues.powerBoostListedSpeedCoefficient}% faster</style> instead. If airborne, do a short <style=cIsUtility>mid-air dash</style>.";
            LanguageAPI.Add(prefix + "UTILITY_BOOST_DESCRIPTION", boostDescription);

            #endregion

            #region Scepter Boost

            LanguageAPI.Add(prefix + "SCEPTER_UTILITY_BOOST_NAME", $"Thunderous Boost");
            string scepterBoostDescription = Helpers.ScepterDescription(
                $"Run into enemies to deal {StaticValues.scepterBoostDamageCoefficient * 100f}% damage and launch them. Damage increases based on your movement speed.");
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
                $"{superSonicColor}Super Thunderous Boost</color>");
            LanguageAPI.Add(prefix + "SUPER_SCEPTER_UTILITY_BOOST_DESCRIPTION",
                superBoostDescription + scepterBoostDescription);

            #endregion

            #region Special

            LanguageAPI.Add(prefix + "SPECIAL_GRAND_SLAM_NAME", "Grand Slam");
            string grandSlamDescription =
                $"<style=cIsUtility>Homing</style>. Dash forward into an enemy to attack with <style=cIsDamage>{100f * StaticValues.grandSlamSpinDamageCoefficient}% damage</style> repeatedly before unleashing a powerful attack from above dealing <style=cIsDamage>{100f * StaticValues.grandSlamFinalDamageCoefficient}% damage</style> and <style=cIsUtility>launching</style> enemies.";
            LanguageAPI.Add(prefix + "SPECIAL_GRAND_SLAM_DESCRIPTION", grandSlamDescription);

            #endregion

            #region Super Special

            LanguageAPI.Add(prefix + "SUPER_SPECIAL_GRAND_SLAM_NAME", $"{superSonicColor}Super Grand Slam</color>");
            LanguageAPI.Add(prefix + "SUPER_SPECIAL_GRAND_SLAM_DESCRIPTION",
                grandSlamDescription +
                $"\n{superSonicColor}Create afterimages that rain down from the sky dealing {100f * (Modules.StaticValues.superGrandSlamDOTDamage * 3)}% damage per second.</color>");

            #endregion

            #region Special #2

            LanguageAPI.Add(prefix + "SPECIAL_SUPER_TRANSFORMATION_NAME", $"{superSonicColor}Super Sonic</color>");
            LanguageAPI.Add(prefix + "SPECIAL_SUPER_TRANSFORMATION_DESCRIPTION",
                $"Transform into {superSonicColor}Super Sonic</color> for {Modules.StaticValues.superSonicDuration} seconds. {superSonicColor}Upgrades all of your skills</color>. Increases <style=cIsDamage>damage</style> by <style=cIsDamage>+{100f * StaticValues.superSonicBaseDamage}%</style>. Increases <style=cIsDamage>attack speed</style> by <style=cIsDamage>+{100f * StaticValues.superSonicAttackSpeed}%</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>+{100f * StaticValues.superSonicMovementSpeed}%</style>. Gain <style=cIsHealing>complete invincibility</style> and <style=cIsUtility>flight</style>." +
                Environment.NewLine + Environment.NewLine + "This can only be used once per stage.");

            #endregion

            #region Achievements

            LanguageAPI.Add("ACHIEVEMENT_" + SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE_NAME", 
                "Sonic: Mastery");
            LanguageAPI.Add("ACHIEVEMENT_" + SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICMASTERYUNLOCKABLE_DESCRIPTION",
                "As Sonic, beat the game or obliterate on Monsoon.");

            LanguageAPI.Add("ACHIEVEMENT_" + SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE_NAME",
                "Sonic: Spinning Upside Down");
            LanguageAPI.Add("ACHIEVEMENT_" + SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "SONICPARRYUNLOCKABLE_DESCRIPTION",
                $"As Sonic, hit {Achievements.SonicHomingAttackAirborneAchievement.countRequired} different enemies with the homing attack without touching the ground.");

            #endregion

            #endregion
        }
    }
}