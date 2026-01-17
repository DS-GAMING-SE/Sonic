# Changelog

## v?

### HedgehogUtils v?
 - (Voicelines) Added a new system for handling voicelines
 
 - (Visuals) The super form transformation now has a subtle glow around your character
 
 - (Internal) Removed unnecessary On.RoR2.UI.HUD.Awake hook used for creating the boost meter. The boost meter hud is now handled through RoR2's Hud Overlay system. This change has also been applied to Sonic's power boost hud. Overall, the code for both of them is much simpler now

## v4.0.6
 - (HedgehogUtils) Updated for newest version
 
 - (Visuals/Bug Fix) Rewrote the code for the death animation. The body now fades away at the end of the animation instead of disappearing at once. This should also fix any instances of the death animation getting cut off or bodies remaining forever after the animation ends. The entity state for this is in HedgehogUtils, so it can be reused for other characters easily.
 
 - (Bug Fix) Fixed rare cases where some modded items would have an item display on Sonic
 
 - (Internal) The sounds for Sonic's jumping and death animation are now handled by RoR2's SfxLocator instead of being hard-coded. Apparently the SfxLocator's jump sound isn't even networked. Syncing Sonic's jump in multiplayer has proven to be harder than I thought
 
 - (Internal) Removed the old superSonicColor in all of Sonic's text and replaced it with HedgehogUtils.Helpers.SuperFormText() for consistency
### HedgehogUtils v1.1.6
 - (Bug Fix) Fixed harmless *"ItemDef 'ChaosEmerald' has an item index of 'None'. Attempting to fix..."* error that appeared in the logs when opening the game
 
 - (Compatibility) Removed [Sandswept's](https://thunderstore.io/package/SandsweptTeam/Sandswept) Delta Construct from the launch blacklist since the issue has been fixed on their end
 
 - (Config) The Enable Logs config now has options for showing no logs, minimal logs, or all logs. Any previous changes to this config may need to be redone. This config is now set to minimal by default. *Why did I think it was a good idea to have logs be disabled by default before?*

### Known Issues
 - Jumping animation is not synced in multiplayer (Apparently they aren't synced for any survivor?! Maybe one day I will fix this...)
 - (HedgehogUtils) Launch projectiles' values aren't properly networked so they don't update any values changed after the projectile is spawned. Things like the unique vfx of a crit launch projectile won't update to clients if the values are updated during the launch, such as if you launch a launch projectile
 - (HedgehogUtils) Some enemies become invisible in their death animations after being killed by a launch
 
## v4.0.5
 - (HedgehogUtils) Updated for newest version
 
 - (Bug Fix) Maybe fixed a multiplayer desync issue involving CustomEmotesAPI
 
 - (Internal) The sounds for Sonic's brake, jump, and death have been moved to HedgehogUtils and the sound ids have changed
### HedgehogUtils v1.1.5
 - (Bug Fix) Fixed corpses being able to be hit by players after being launched in multiplayer
 
## v4.0.4
 - (Assets - Mastery Skin)
	 - Some animations are now different when using the mastery skin
	 - Added specular and fresnel emission, making the skin react better to light and look as shiny as it should be
	 - Made the back of the skin look like it's glowing by hand-drawing baked lighting. The model was sliiiightly adjusted to make this work better
 - (Assets) Added specular to the default skin making it react better to light
 - (Assets) Redone the skill icon for the Momentum passive
 
 - (Bug Fix) Fixed for Alloyed Collective
 - (Bug Fix) Internally set the cooldown of Follow Up to 0 to avoid an unintended interaction with [Starstorm 2](https://thunderstore.io/package/TeamMoonstorm/Starstorm2)'s Universal Charger item. The cooldown of Super Follow Up is unchanged
 - (Bug Fix) Fixed subtle miscoloring in Sonic's keywords (Added missing </style> to Sonic's Homing keyword)
 
### HedgehogUtils v1.1.4
 - (+ Buff) Launched enemies now apply the same enemy-specific on-hit effects as enemies thrown by Drifter
 
 - (Assets) Small improvements to Super form related VFX, mainly the rainbow effects
 - (Assets) Added a post processing effect to super forms. This subtly tints the screen yellow when you're near someone in their super form. Don't worry, it's far from as strong as Magma Worm's screen yellowing. There is a config to turn this effect off
 
 - (Bug Fix) Fixed for Alloyed Collective. Forms should work with temporary items now too, but I haven't tested it.
 - (Bug Fix) Fixed the "Announce Super Transform" text not being formatted correctly. How long has this not been working?
 - (Bug Fix) Fixed an issue where having your utility skill changed to an invalid skill (such as False Son's skill disabling) while Boost Idling would softlock
 
 - (Compatibility) I notice those Chaos Emeralds aren't bolted to the ground...

## v4.0.3

 - (HedgehogUtils) Updated for newest version
 
 - (+ Buff) Super Boost now gives an armor buff like normal Boost does
 
 - (Internal) Super Boost's speed buff now uses the same calculation method as most other Boost skills. This is just for consistency has almost 0 impact on the actual amount of speed you get (at least when I tested it with no other speed items or buffs)
 
### HedgehogUtils v1.1.2

 - (+ Buff) Sliiiiightly reduced the speed the boost meter drains so 2 Alien Heads is enough to reach infinite boost
 
## v4.0.2

 - (Bug Fix) Fixed elites and some other buffs not working properly when this mod is installed

## v4.0.1

 - (+ Buff) Super Scepter Boost can now launch heavier enemies than normal Scepter Boost

 - (Bug Fix) Power Boost's effect will no longer attempt to activate/deactivate when using Super Scepter Boost
 - (Internal) All Scepter Boost skills now inherit from the new ScepterBoostBase. ScepterBoostBase is boost with the Scepter damaging effect and NewScepterBoost adds the Power Boost effect to it. Use ScepterBoostBase as the base for any Boost skill that you don't want to have Power Boost. If you want Power Boost, start from NewScepterBoost.
 
 - (Read Me) Briefly mentioned where people should look for resources related to modding Sonic/HedgehogUtils
 
 - (Dependency) Removed dependency on R2API_TempVisualEffects because I'm pretty sure I was never using it
 
### HedgehogUtils v1.0.1

 - (Bug Fix) Blacklisted [Sandswept's](https://thunderstore.io/package/SandsweptTeam/Sandswept) Delta Construct from being launched to prevent framerate killing error spam on death
 
## v4.0.0
This sure took longer than I expected...

 - (HedgehogUtils) Introducing my new mod called HedgehogUtils. A lot of mechanics I designed for Sonic were things I felt could be used for a lot of other things besides Sonic. Mechanics, such as Sonic's Super form, boost skill, and launch mechanic (more on that below), have been moved into this new mod. Everything in this mod is designed to be usable for other people's mods. **Do you want to make your own Super forms? Do you want to make your own Sonic survivors?** This mod is meant to help do some of the work for you. All my code is open source and there is documentation explaining everything you need to know to implement HedgehogUtils into your own mod.

 - (Chaos Emeralds) Chaos Emeralds spawning is now tied to an artifact. This artifact will enable or disable the spawning of the Chaos Emeralds regardless of what survivors are selected. I didn't like making people dig around in the config to have Chaos Emeralds spawn every game.

 - (Chaos Emeralds) The colors of the Chaos Emeralds that spawn in the world are now random rather than following a set order

 - (Assets) Many animations have been improved, some completely redone.
 
 - (HUD) The Boost Meter will now remain visible when using Super Boost.
     - The boost meter will be shown as being infinite.
	 - The boost meter will now become yellow to match the Super form

 - (Logbook) Added logbook entries for the Chaos Emeralds. I am never trying to write a logbook entry again. I am not a writer but I did my best

 - (+ Buff) Added the new "Launch" mechanic and applied it to most of Sonic's skills. Under certain conditions, Sonic's attacks can launch enemies, turning them into a projectile that flies in the direction hit and damages other enemies they run into.

 - (+ Buff) To go with the new Launch mechanic, Grand Slam's spin attack now holds enemies in place mid-air, opening up some potential for air combos. Launch an enemy up into the air and use Grand Slam to bring them back down. 

 - (+ Buff) Melee and Sonic Boom don't reduce your movement speed as much (Melee speed roughly doubled, Sonic Boom 55% -> 30%, Cross Slash 15% -> 10%)
 
 - (+ Buff) Parry is receiving a rework in the form of a new follow-up attack that can be performed after a successful parry. Now the parry skill is able to do damage, so you are no longer limited to having only two damaging moves on Sonic. This should also make it easier for new players to understand how the Super upgrade of parry already has a follow-up attack.

 - (+ Buff) Sonic Boom now applies an armor reducing debuff on hit ~~to make up for how much cooler Parry is~~
 
 - (= Changes) Parry's buff has been adjusted to account for the new follow up attack. The buff isn't as strong, but lasts longer so you have time to use the follow-up attack and Grand Slam before the buff ends.
     - Parry's attack speed buff has been reduced (40% -> 25%)
	 - Parry's movement speed buff has been reduced (30% -> 25%)
	 - Parry's buff duration has been increased (3s -> 5s)
	 
 - (= Changes) Boost has been recoded from the ground up. It should feel a bit closer to how it does in actual Sonic games. Overall the boost has been made easier to use for exploration but a bit harder to use in combat. Here are some of the noticeable changes.
     - A portion of the boost meter is immediately consumed when you start boosting, which makes using its I-frames require paying some attention to the boost meter.
	 - Boost drains much slower when boosting continuously, making it better for map exploration.
	 - Boost recharges faster.
	 - It takes less cooldown reduction items to reach infinite boost.
	 - You can't take turns as tightly as normal while boosting. There's a new braking animation for attempting to take turns too tightly

 - (Bug Fix) Fixed for SOTS Phase 3
 - (Bug Fix) Fixed Scepter Boost damage not correctly matching movement speed
	 
 - (Config) Configs related to the Chaos Emeralds, Super form, and Boost meter location have been moved to the new HedgehogUtils mod. Any config changes you set previously will need to be redone
 - (Config) Added a config option that changes the duration of the Super form
 - (Config) Added a config option that disables the Super form's invincibility, instead granting 200 armor
 - (Config) Added a config option that disables logs. Logs are now disabled by default

<details>
<summary>v3.0.3</summary>

 - (Assets) Added a loading screen sprite of Sonic if LoadingScreenFix is installed
 
 - (Assets) Redone boost VFX
 
 - (Bug Fix) Fixed for SOTS Phase 1
</details>
 
<details>
<summary>v3.0.2</summary>

 - (Assets) Added an inspect description to the Chaos Emeralds

 - (Bug Fix) Removed obnoxious TestState spam in the logs
 - (Bug Fix) Holding Sonic Boom now properly stacks Luminous Shot

 - (Optimization) Optimized some stuff with item pickups. You should see less log statements about items needed for transforming if you use stuff like Aerolt

 - (Config) Added a new config option that handles how Chaos Emeralds are shared between players, letting you be more restrictive with who gets to activate the Super form when emeralds are split between players
 - (Config) Added a new config option to announce in chat when someone transforms into their Super form
 </details>
 
<details>
<summary>v3.0.1</summary>

 - (Dependency) Forgot R2API_Items... oops
</details>

<details>
<summary>v3.0.0</summary>
 SOTS update came at a bit of an awkward time. At this point i'm like 90% done with the complete rewrite of Super forms but not quite at the point I want to be. Still, I don't want to keep people waiting without a Sonic mod so custom super form mod compatibility will come next update.
 Almost everything related to Super Sonic has been rewritten. For most players, the difference won't be too noticeable, but this will soon allow other modders to make their own Super forms for Sonic or for any other survivor. ~~Next update~~ Next major update will include this and will likely be the final update I make to the Sonic mod.

 - (Assets) Added a new sound for when Chaos Emeralds are purchased
 - (Assets) Reworded Chaos Emerald descriptions, mostly for parity with base game descriptions

 - (+) ALL survivors are now able to turn Super with the Chaos Emeralds

 - (Bug Fix) Fixed mastery achievement often not being unlocked when it should have
 - (Bug Fix) Fixed errors causing broken run history to be generated
 - (Bug Fix) Fixed yellow glow staying when losing Super form while boosting

 - (Compatibility) Added LookingGlass compatibility with buff descriptions and Chaos Emerald information

 - (Config) Added new config options for controlling the number of Chaos Emeralds that can spawn each stage
 - (Config) Added a new config option for allowing Chaos Emeralds to spawn even if no one is playing Sonic
 - (Config) Added a new config option for changing the price of Chaos Emeralds
 - (Config) Added a new config option for allowing the Chaos Emeralds to not be consumed after being used
 
 - (Dependency) Removed dependency on R2API, instead using the separate R2API modules
 - (Dependency) Added LoopingSoundFix as a dependency. I noticed some issues with Super Grand Slam rain projectile looping its sound for the entire stage so it's here just in case. If you don't want it, it can be removed
</details>

<details>
<summary>v2.0.0</summary>

 - (New Form) Added Super Sonic and collectable Chaos Emeralds
 
 - (New Skin) Added a new mastery skin. Thanks to FORCED_REASSEMBLY for the help with making it (Can also be unlocked through the config)
 
 - (Assets) Adjustments to most animations
 - (Assets) Adjustments to many VFX
 - (Assets) Added custom buff icons to Sonic's armor when homing attacking
 
 - (Logbook) Added logbook entries for Sonic. Chaos Emerald logs coming soon
 
 - (+) Flat cooldown reduction (Purity) now increases the boost cap more (25 -> 33.3)
 
 - (Bug Fix) Homing targeting has been adjusted so it now works with Artifact of Chaos
 - (Bug Fix) Homing Attack and Grand Slam movement slows down when approaching enemies so you won't overshoot at high speeds
 - (Bug Fix) Fixed strange movement involving melee and extreme attack speed
 
 - (Read Me) Completely redecorated it with tables and images
 </details>

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