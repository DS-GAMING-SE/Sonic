# Changelog

### 2.0.0

 - (Super Sonic) Added Super Sonic and collectable Chaos Emeralds
 
 - (New Skin) Added a new mastery skin. Thanks to FORCED_REASSEMBLY for the help with making it (Can also be unlocked through the config)
 
 - (Assets) Adjustments to most animations
 - (Assets) Boost VFX now has a distortion effect
 - (Assets) Added custom buff icons to Sonic's armor when homing attacking
 
 - (Logbook) Added logbook entries for Sonic. Chaos Emerald logs coming soon
 
 - (+) Flat cooldown reduction (Purity) now increases the boost cap more (25 -> 33.3)
 
 - (Bug Fix) Homing targeting has been adjusted so it now works with Artifact of Chaos
 - (Bug Fix) Homing Attack and Grand Slam movement slows down when approaching enemies so you won't overshoot at high speeds
 - (Bug Fix) Fixed strange movement involving melee and extreme attack speed
 
 - (Read Me) Completely redecorated it with tables and images

### Known Issues
 - Boost meter is kinda shaky as clients in multiplayer
 - Jumping animation is not synced in multiplayer (Apparently they aren't synced for any survivor?!)

<details>
<summary>v1.2.1</summary>

 - (Bug Fix) Probably fixed NREs from alt secondary skill achievement
 
 </details>

<details>
<summary>v1.2.0</summary>

 - (New Skill) Added a new alternate secondary skill. This can be unlocked by completing a newly added achievement (Or just unlocking it with the config)
 - (New Skill/Compatibility) Added support for StandaloneAncientScepter. Upgrade Boost into Thundering Boost, dealing damage to enemies you run through

 - (Assets) Small adjustments to most VFX
 - (Assets) Small adjustments to a few animations
 - (Assets) Created new "Homing" keyword and added it to applicable skill descriptions
 
 - (+) Boost speed scaling has been adjusted to increase base move speed and move speed multipliers (Default movement speed is the same, but scales better with other movement speed buffs)
 - (+) Momentum passive now has a unique interaction with flying. While flying, build momentum by moving in a straight line
 - (=) The camera now zooms out while boosting
 
 - (Bug Fix) Fixed cases where Sonic's primary wasn't proccing shuriken
 
 - (QoL) Using Grand Slam without a target and then hitting something will now set that enemy as the target for the rest of the attack (I've been trying to do this from the very beginning)
 
 - (Read Me) Added a section listing compatible mods
 - (Changelog) Put older patch notes into dropdown menus
 
 </details>

<details>
<summary>v1.1.0</summary>

 - (Assets) Many animations have been changed slightly
 - (Assets) A new animation has been added for entering boost while standing still

 - (+) Movement while meleeing has been adjusted, giving slightly more movement based on movement speed
 - (+) Using Grand Slam without a target now goes further
 - (=) Adjusted hit stop for melee and homing attack
 - (-) Max targeting range of all homing skills has been reduced
 
 - (Bug Fix) Melee and homing attack sounds no longer desync

 - (QoL) There is now a reticle for aiming homing skills
 - (QoL) New config option for whether homing attacks require a key press to be triggered or whether they can be activated by holding the key down
 
 - (Read Me) Removed mention of homing skills scaling with sprint speed (Sprint speed is jank and I can't get the scaling to work)

</details>

<details>
<summary>v1.0.0</summary>

 - (Assets) The following sound effects have been added
     - Jumping
	 - Starting melee attacks
	 - Landing melee attacks
	 - Starting a homing attack
	 - Landing a homing attack
	 - Firing a sonic boom
	 - Landing a sonic boom
	 - Boosting
	 - Switching between boost types
	 - Charging a Grand Slam
	 - Landing each hit of Grand Slam
	 - Dying
 - (Assets) Updated all icons
 
 - (+) Bandolier now restores a small portion of the boost meter
 - (=) Walking speed reduced but sprinting speed multiplier increased, max speed stays the same (10 and 1.45 -> 8.5 and 1.7)
 
 - (Bug Fix) Replaced v0.4.1's temporary fix to passive skill stuff with a permanent fix
 - (Bug Fix) Fixed small bug with the speed decrease of the momentum passive
 - (Bug Fix) Fixed being permanently stuck after using a homing skill with 0 movement speed
 - (Bug Fix) With the power of extreme jank, Malachite Aspects no longer break every bone in Sonic's body. All jitter bones placed on Sonic will be forcefully removed
 - (Bug Fix) Exiting boost mid-air no longer stops the jump ball animation
 
 - (Compatibility) RiskOfOptions compatibility with settings to adjust boost meter positioning
 - (Compatibility) BetterUI compatibility

 - (Read Me) Added momentum passive and general information about Sonic to the read me
 
</details>

<details>
<summary>v0.4.1</summary>

 - (Optimization) Stat changes now mark stats as dirty instead of calling recalc directly
 
 - (Bug Fix) Temporary band-aid fix to issue with Sonic's passive skill and passive skill family
 
</details>

<details>
<summary>v0.4.0</summary>

 - (New Skill) Added the Momentum passive. This passive makes Sonic speed up when running down hill and slow down when running up hill

 - (Assets) New VFX has been added for the following
     - Starting boost
	 - Starting power boost
	 - Starting a homing attack
	 - Landing a homing attack
	 - Landing any melee attack
	 - Landing the final kick of Grand Slam
 - (Assets) The boost meter will now glow when power boosting
 - (Assets) Added ending text
 - (Assets) Slight tweaks to skill icons
 
 - (Bug Fix) The max time you can spend in Grand Slam's final kick now scales with movement speed, so low movement speed doesn't stop the attack from landing
 
 - (Compatibility) MULTIPLAYER COMPATIBILITY YEEEEAAAHHH
 - (Compatibility) Mostly moved from On.RecalculateStats to RecalculateStatsAPI (RecalculateStatsAPI where is my acceleration stat)
 - (Compatibility) CustomEmotesAPI support
 - (Compatibility) Sonic will now have proper movement and infinite mid-air boosts while flying (Milky Chrysalis)
 - (Compatibility) The slow descent at the end of Milky Chrysalis can now be cancelled by using any homing skill
 - (Compatibility) Sonic is now compatible with utility skill replacements (For the 0 people who would want to replace Boost with Essence of Heresy)

</details>

<details>
<summary>v0.3.0</summary>

 - (Assets) T-Posing no more! The following animations have been added
     - Jumping
	 - Spinning in a ball
	 - Grand Slam's final kick
	 - Ascending/Descending
	 - Dying
	 - Idle animation for not moving for a while
 - (Assets) The boost meter has been redone with these new features
     - No longer breaks from changes in screen resolution
	 - Now has custom visuals to show changes in the boost cap
	 - Infinite boost icon now properly fades away like the normal boost meter does
 - (Assets) Added some vfx to Sonic to indicate power boosting
 - (Assets) Skill descriptions changed slightly

 - (+) Flat cooldown reduction (Purity) now increases the boost cap much more (10 -> 25)
 - (=) Boost speed and power boost speed have been adjusted to make the difference more noticeable (40/60 -> 35/65)
 - (-) The auto-targeting of the final hit of Grand Slam can no longer track upwards

 - (Bug Fix) Fixed boost sometimes not switching between normal and power boost speeds
 - (Bug Fix) Fixed permanent invincibility involving Grand Slam and Volcanic Egg
 
</details>
 
<details>
<summary>v0.2.0</summary>

 - (Assets) Redone textures
 - (Assets) Made visuals for the Sonic Boom projectile
 - (Assets) Small tweaks to some vfx
 - (Assets) Added skill icons
 
 - (+) Melee hitboxes are now slightly larger
 - (+) The final hit of Grand Slam now does more knockback
 - (=) Sonic Boom and the start-up of Grand Slam now slow to a stop instead of stopping immediately
 - (=) Sonic Boom projectile is now smaller to make it collide with the floor less
 - (-) The repeated attacks of Grand Slam can no longer kill
 
 - (Bug Fix) Missing the final kick of Grand Slam will now set your velocity to 0, preventing fall damage
 - (Bug Fix) Fixed a lot of weird animation transitions
 - (Bug Fix) Picking up an item that gives additional utility skill stocks (Hardlight Afterburner) while grounded will now immediately restore all utility stocks
 - (Bug Fix) Sonic is no longer extra small in the character select screen (He's not THAT short)
 
</details>

<details>
<summary>v0.1.0</summary>

 - Initial Release
 
</details>
