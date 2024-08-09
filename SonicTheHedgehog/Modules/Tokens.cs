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

            string superSonicColor = "<color=#ffee00>";

            #region Passives

            LanguageAPI.Add(prefix + "MOMENTUM_PASSIVE_NAME", "Momentum");
            LanguageAPI.Add(prefix + "MOMENTUM_PASSIVE_DESCRIPTION",
                $"<style=cIsUtility>Build up speed</style> by <style=cIsUtility>running down hill</style> to move up to <style=cIsUtility>{MomentumPassive.speedMultiplier * 100}% faster</style>. <style=cIsHealth>Lose speed by running up hill to move up to {Mathf.Floor((MomentumPassive.speedMultiplier * 100) / 3)}% slower.</style>\nIf <style=cIsUtility>flying</style>, <style=cIsUtility>build up speed</style> by <style=cIsUtility>moving in a straight line.</style>");

            #endregion

            #region Super Form

            LanguageAPI.Add(prefix + "SUPER_PREFIX", "Super {0}");

            LanguageAPI.Add(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SUPER_FORM", "Super");

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
                $"\n{superSonicColor}Every close range attack fires a projectile dealing {(100f * StaticValues.superMeleeExtraDamagePercent)}% of the attack's damage.</color>");

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
                $"Press or hold to enter the <style=cIsUtility>parry stance</style> for a brief period of time. {parryOnHitDescription}");

            #endregion

            #region Super Parry
            string idwAttackName = $"IDW Attack";
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_PARRY_NAME", $"{superSonicColor}Perfect Parry</color>");
            LanguageAPI.Add(prefix + "SUPER_SECONDARY_PARRY_DESCRIPTION",
                $"Enter the <style=cIsUtility>parry stance</style> for a {superSonicColor}very brief period of time</color>. {parryOnHitDescription} \n{superSonicColor}Reduce all nearby enemies' attack speed and movement speed by {(1 / StaticValues.superParryAttackSpeedDebuff) * 100}% and armor by {StaticValues.superParryArmorDebuff}. \nReplace this skill with \"{idwAttackName}\". \n\nThis can only be triggered once.</color>");

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

            #region Emeralds
            
            LanguageAPI.Add(prefix + "EMERALD_TEMPLE_CONTEXT", "Receive Emerald");
            LanguageAPI.Add(prefix + "EMERALD_TEMPLE_YELLOW", "Chaos Emerald: Yellow");
            LanguageAPI.Add(prefix + "EMERALD_TEMPLE_BLUE", "Chaos Emerald: Blue");
            LanguageAPI.Add(prefix + "EMERALD_TEMPLE_RED", "Chaos Emerald: Red");
            LanguageAPI.Add(prefix + "EMERALD_TEMPLE_GRAY", "Chaos Emerald: Gray");
            LanguageAPI.Add(prefix + "EMERALD_TEMPLE_GREEN", "Chaos Emerald: Green");
            LanguageAPI.Add(prefix + "EMERALD_TEMPLE_CYAN", "Chaos Emerald: Cyan");
            LanguageAPI.Add(prefix + "EMERALD_TEMPLE_PURPLE", "Chaos Emerald: Purple");

            string chaosEmeraldDesc = $" of the <style=cIsUtility>seven</style> Chaos Emeralds." + Environment.NewLine + $"When all <style=cIsUtility>seven</style> are brought together by you and/or other players, press {superSonicColor}V</color> to transform from <style=cIsUtility>Sonic</style> into {superSonicColor}Super Sonic</color> for {superSonicColor}{Modules.StaticValues.superSonicDuration}</color> seconds. Transforming {superSonicColor}upgrades all of your skills</color>. Increases <style=cIsDamage>damage</style> by <style=cIsDamage>+{100f * StaticValues.superSonicBaseDamage}%</style>. Increases <style=cIsDamage>attack speed</style> by <style=cIsDamage>+{100f * StaticValues.superSonicAttackSpeed}%</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>+{100f * StaticValues.superSonicMovementSpeed}%</style>. Grants <style=cIsHealing>complete invincibility</style> and <style=cIsUtility>flight</style>." + Environment.NewLine + Environment.NewLine + "This will <style=cIsHealth>consume</style> all seven Chaos Emeralds.";
            string chaosEmeraldPickup = $" out of <style=cIsUtility>seven</style>. When all are brought together by you and/or other players, <style=cIsUtility>Sonic</style> can transform into {superSonicColor}Super Sonic</color> by pressing {superSonicColor}V</color>, granting {superSonicColor}upgraded skills</color>, <style=cIsHealing>invincibility</style>, <style=cIsUtility>flight</style>, and <style=cIsDamage>incredible power</style> for {superSonicColor}{Modules.StaticValues.superSonicDuration}</color> seconds. This will <style=cIsHealth>consume</style> all the Chaos Emeralds.";

            LanguageAPI.Add(prefix + "YELLOW_EMERALD", "Chaos Emerald: <style=cIsDamage>Yellow</style>");
            LanguageAPI.Add(prefix + "YELLOW_EMERALD_PICKUP", $"<style=cIsDamage>One</style>" + chaosEmeraldPickup);
            LanguageAPI.Add(prefix + "YELLOW_EMERALD_DESC", $"<style=cIsDamage>One</style>" + chaosEmeraldDesc);
            
            LanguageAPI.Add(prefix + "BLUE_EMERALD", "Chaos Emerald: <color=#2b44d6>Blue</color>");
            LanguageAPI.Add(prefix + "BLUE_EMERALD_PICKUP", $"<color=#2b44d6>One</color>" + chaosEmeraldPickup);
            LanguageAPI.Add(prefix + "BLUE_EMERALD_DESC", $"<color=#2b44d6>One</color>" + chaosEmeraldDesc);
            
            LanguageAPI.Add(prefix + "RED_EMERALD", "Chaos Emerald: <style=cDeath>Red</style>");
            LanguageAPI.Add(prefix + "RED_EMERALD_PICKUP", $"<style=cDeath>One</style>" + chaosEmeraldPickup);
            LanguageAPI.Add(prefix + "RED_EMERALD_DESC", $"<style=cDeath>One</style>" + chaosEmeraldDesc);


            LanguageAPI.Add(prefix + "GRAY_EMERALD", "Chaos Emerald: <color=#b8c5d6>Gray</color>");
            LanguageAPI.Add(prefix + "GRAY_EMERALD_PICKUP", "<color=#b8c5d6>One</color>" + chaosEmeraldPickup);
            LanguageAPI.Add(prefix + "GRAY_EMERALD_DESC", "<color=#b8c5d6>One</color>" + chaosEmeraldDesc);
            
            LanguageAPI.Add(prefix + "GREEN_EMERALD", "Chaos Emerald: <style=cIsHealing>Green</style>");
            LanguageAPI.Add(prefix + "GREEN_EMERALD_PICKUP", $"<style=cIsHealing>One</style>" + chaosEmeraldPickup);
            LanguageAPI.Add(prefix + "GREEN_EMERALD_DESC", $"<style=cIsHealing>One</style>" + chaosEmeraldDesc);

            LanguageAPI.Add(prefix + "CYAN_EMERALD", "Chaos Emerald: <style=cIsUtility>Cyan</style>");
            LanguageAPI.Add(prefix + "CYAN_EMERALD_PICKUP", $"<style=cIsUtility>One</style>" + chaosEmeraldPickup);
            LanguageAPI.Add(prefix + "CYAN_EMERALD_DESC", $"<style=cIsUtility>One</style>" + chaosEmeraldDesc);
             
            LanguageAPI.Add(prefix + "PURPLE_EMERALD", "Chaos Emerald: <color=#c437c0>Purple</color>");
            LanguageAPI.Add(prefix + "PURPLE_EMERALD_PICKUP", "<color=#c437c0>One</color>" + chaosEmeraldPickup);
            LanguageAPI.Add(prefix + "PURPLE_EMERALD_DESC", "<color=#c437c0>One</color>" + chaosEmeraldDesc);

            #region Emerald Lore

            //string dataScraperOpening = "<style=cMono>Welcome to DataScraper (v3.1.53 – beta branch)\n$ Scraping memory... error.\n$ Resolving...\n$";

            //string dataScraperEnding = "\n$ Combing for relevant data... done.\nDisplaying partial result.</style>\n\n";

            //StringBuilder sb = new StringBuilder();

            //LanguageAPI.Add(prefix + "BLUE_EMERALD_LORE", dataScraperOpening + FileNotFoundEmeraldLogHelper(sb, 0) + dataScraperEnding +
            //    "Only a fraction of us were able to make it off world before it attacked. It was as if death itself had claimed our homeworld, leaving nothing but smoldering rock where our home planet once was.\n\nThe emeralds powered our engines. It was only with their power that any of us managed to escape.\n\nAll we could do then was move forward into the darkness with only the glimmering light of the emeralds to guide us.");

            //LanguageAPI.Add(prefix + "CYAN_EMERALD_LORE", dataScraperOpening + FileNotFoundEmeraldLogHelper(sb, 1) + dataScraperEnding +
            //    "There's far more to these gems than we know about. It couldn't have just been random chance that drew us to this world. The emeralds reacted to something.. no.. something took control of the emeralds, and by extension, our ships. Whatever it is, it's connected to the emeralds in some way. In the end, it doesn't really matter why it brought us here anyway. I had long since gotten used to the chaos.\n\nThe world that strange force had brought us to was a primitive one, many millenia behind us. We chose to isolate ourselves on an uninhabited archipelago to avoid interfering too much with this world's inhabitants. With our numbers so slim, these islands had plenty of room for us. Besides, we are no conquerors.\n\nNo one should have their home taken away from them.");

            //LanguageAPI.Add(prefix + "GRAY_EMERALD_LORE", dataScraperOpening + FileNotFoundEmeraldLogHelper(sb, 2) + dataScraperEnding +
            //    "---. In cyber space we kept our history, our memories, our hopes, our souls. In the digital dream, it felt as if the home we had lost was still with us.");

            //LanguageAPI.Add(prefix + "GREEN_EMERALD_LORE", dataScraperOpening + FileNotFoundEmeraldLogHelper(sb, 3) + dataScraperEnding +
            //    "Before, we had run away and lost almost everything. Now, not only was what little we had left in danger once more, we had also dragged a planet that's not our own into this conflict.\n\nWe could've run again, we could've tried to hide on another world, we could've left this world to die like ours.\n\nHow much would we lose in our rushed and desperate escape? Was there any guarantee it wouldn't find us again? How many more worlds would be in danger from this... thing?\n\nWe could've run away.\n\nWe didn't.");

            //LanguageAPI.Add(prefix + "PURPLE_EMERALD_LORE", dataScraperOpening + FileNotFoundEmeraldLogHelper(sb, 4) + dataScraperEnding +
            //    "The emeralds powered our greatest weapons in this fight. Once again, they were the key to our survival. But, once again, their power wasn't enough to destroy the entity that threatened us.\n\n----. It has been locked away in cyber space. The toll this took on us was too great. ");

            //LanguageAPI.Add(prefix + "RED_EMERALD_LORE", dataScraperOpening + FileNotFoundEmeraldLogHelper(sb, 5) + dataScraperEnding +
            //    "All that remains are our memories. Blips of love, of fear, of hope, of life, tethered to the charms we once held so close to our hearts. Even me and my ramblings of the past are all data stored in cyber space. It's the only thing keeping the last of our civilization from being forgotten. --It pains me that there's nothing more I can do--. Even so, I hope no one finds us, lest they release the very thing that reduced us to this state.\n\nAfter everything thats happened, life moves on, with or without us. I no longer have any desire to rebuild all that has been lost. I remain only to ensure that no world will end up like ours.");

            //LanguageAPI.Add(prefix + "YELLOW_EMERALD_LORE", dataScraperOpening + FileNotFoundEmeraldLogHelper(sb, 6) + dataScraperEnding +
            //    "To those who find any of what we've left behind, please take what you wish. Perhaps it can save you from suffering a fate like ours.\n\nWhen it mattered most, I wasn't able to do anything. I was unable to save anyone, not even myself. Maybe being able to save you, whoever you may be, will be enough for me to move on.");

            #endregion

            #endregion

            #endregion
        }

        internal static string FileNotFoundEmeraldLogHelper(StringBuilder sb, int index)
        {
            sb.Clear();
            int count = 0;
            while (count < 7)
            {
                if (count == index)
                {
                    sb.Append(" [ ]");
                }
                else
                {
                    sb.Append(" [X]");
                }
                count++;
            }
            return sb.ToString();
        }
    }
}