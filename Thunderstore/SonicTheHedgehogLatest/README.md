# <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texSonicIcon.png?raw=true" width="64"> Sonic The Hedgehog

Sonic's the name and speed is his game! Sonic is a survivor who's all about quick movements and close range single target damage. Sonic's higher than normal movement speed and many movement skills let him jump from enemy to enemy when in combat, or explore the map quickly when looting. Though he's frail, his quick movements and invincibility frames let him avoid danger while he does major damage to nearby enemies. Though close quarters is his specialty, he isn't without ranged options. Sonic's attacks can launch certain enemies, turning them into projectiles

# Skills

| Skill | Icon | Description |
| ---- | -------- | ---- |
| Passive - **Momentum** | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texMomentumIcon.png?raw=true" width="128"> | Build up speed by **running down hill** to move up to 100% faster. Lose speed by **running up hill** to move up to 33% slower. |
| Primary - **Melee** | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texMeleeIcon.png?raw=true" width="128"> | Melee **nearby** enemies dealing 200% damage. Every **5th** hit deals 600% damage. Targeting an enemy in the **distance** will use the **Homing Attack**, dealing 600% damage. This move can **launch** lightweight enemies. <details>5-hit melee combo. Looking at an enemy that's out of melee range but within a certain distance will use the homing attack, quickly bringing you to the enemy and dealing damage on contact. The speed and max distance of the homing attack scales with movement speed. While approaching an enemy with the homing attack, you get 100 armor.</details>  |
| Secondary - **Sonic Boom**  | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texSonicBoomIcon.png?raw=true" width="128"> |Fire shockwaves dealing 2x160% damage and reducing armor by 2x5. <details>Fire 2 small projectiles straight forward that create a small explosion on impact. Start with 3 charges by default. All charges come back at once after a 5 second cooldown.</details>  |
| Utility - **Boost** | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texBoostIcon.png?raw=true" width="128"> | **Spend boost meter** to move 35% faster than normal. If **health is near full**, move 65% faster instead. If **airborne**, do a short **mid-air dash**.<details>While boosting, your movement speed is increased by 35% and you are given 50 armor. If your health is above 90%, you will power boost, increasing speed by 65%. Activating boost gives invincibility for a brief moment. Boosting will drain your boost meter. If the boost meter runs out, you will be unable to boost again until the meter is recharged. The boost meter recharges overtime when not boosting. % based cooldown reduction, such as Alien Head or Brainstalks, will reduce the speed at which the boost meter is drained and increase the speed it comes back. Flat cooldown reduction, such as Purity, will increase the max capacity of the boost meter. By default, you can use boost as a mid-air dash once before having to touch the ground. Any additional utility stocks, such as those from Hardlight Afterburner, will let you use the mid-air dash more times before having to touch the ground.</details> |
| Special - **Grand Slam** | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texGrandSlamIcon.png?raw=true" width="128"> | **Homing**. Dash forward into an enemy to attack with 140% damage **repeatedly** before unleashing a powerful attack from above dealing 2200% damage and **launching** enemies.<details>This attack will home in on the enemy closest to the crosshair, similar to the homing attack. The amount of repeated weak hits you will do is 5 by default, but increases with attack speed. After landing the initial dash attack, you will have invincibility for the rest of the move's duration. 12 second cooldown.</details> |

There's also an **alternate Secondary skill** that can be unlocked through an achievement (Or through the config)

# <img src="https://github.com/DS-GAMING-SE/HedgehogUtils/blob/master/UnityProject/HedgehogUtils/Assets/AssetBundle/Emeralds/Icons/texGreenEmeraldIcon.png?raw=true" width="64"> Super Sonic

## Chaos Emeralds
With the new Artifact of Chaos Emeralds, Chaos Emeralds will spawn scattered around every normal stage. The emeralds must be purchased similarly to a large chest. Three Chaos Emeralds will spawn per stage, except if you're playing Simulacrum, where there will be five. You don't need one person to collect all Chaos Emeralds, as long as the players cumulatively have all seven, everyone will be able to transform. Many values relating to the Chaos Emeralds can be changed in the config.

## Super Sonic
Once all seven Chaos Emeralds have been collected, anyone can transform into their Super form by pressing **V** (Keybind can be changed in the config). Super Sonic gives new upgraded skills, flight, invincibility, and incredible power for 50 seconds. Though this power is unmatched, it takes multiple stages to collect the emeralds, so it is important to use this power wisely. In multiplayer, all players will be able to transform into their Super form if everyone transforms within ten seconds of the first player transforming.

# Compatible Mods
- CustomEmotesAPI
- LookingGlass
- RiskOfOptions
- StandaloneAncientScepter (Upgrades utility skill)

# Special Thanks
- Presti (Lots of help with Super Sonic code)
- FORCED_REASSEMBLY (Rigging the mastery skin)
- Shader Forge (A tool that let me make the custom Chaos Emerald shaders without losing my mind pre SOTS https://github.com/CuteWaterBeary/ShaderForge)
- Sandwich (Writing the Sonic logbook)

# For Modders
There are lots of tools I've made specifically for other modders to expand the Sonic mod or use mechanics from the Sonic mod for their own projects. If you're interested, you can find the code for the Sonic mod [here.](https://github.com/DS-GAMING-SE/Sonic/wiki) The HedgehogUtils mod is what adds mechanics like the Chaos Emeralds, Super forms, Launching attacks, and the Boost skill. You can find the wiki/code for that [here](https://github.com/DS-GAMING-SE/HedgehogUtils)
