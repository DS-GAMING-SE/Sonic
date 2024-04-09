# <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texSonicIcon.png?raw=true" width="64"> Sonic The Hedgehog

Sonic's the name and speed is his game! Sonic is a survivor who's all about quick movements and close range single target damage. Sonic's higher than normal movement speed and many movement skills let him jump from enemy to enemy when in combat, or explore the map quickly when looting. Though he's frail, his quick movements and invincibility frames let him avoid danger while he does major damage to nearby enemies.

# Skills

| Skill | Icon | Description |
| ---- | -------- | ---- |
| Passive - **Momentum** | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texMomentumIcon.png?raw=true" width="128"> | Build up speed by **running down hill** to move up to 100% faster. Lose speed by **running up hill** to move up to 33% slower. |
| Primary - **Melee** | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texMeleeIcon.png?raw=true" width="128"> | Melee **nearby** enemies dealing 200% damage. Every **5th** hit deals 600% damage. Targeting an enemy in the **distance** will use the **Homing Attack**, dealing 600% damage. <details>5-hit melee combo. Looking at an enemy that's out of melee range but within a certain distance will use the homing attack, quickly bringing you to the enemy and dealing damage on contact. The speed and max distance of the homing attack scales with movement speed. While approaching an enemy with the homing attack, you get 100 armor.</details>  |
| Secondary - **Sonic Boom**  | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texSonicBoomIcon.png?raw=true" width="128"> |Fire shockwaves dealing 2x160% damage. <details>Fire 2 small projectiles straight forward that create a small explosion on impact. Start with 3 charges by default. All charges come back at once after a 5 second cooldown.</details>  |
| Alt. Secondary - **Parry**  | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texParryIcon.png?raw=true" width="128"> |Press or hold to enter the **parry stance** for a brief period of time. Getting hit in this stance will **negate all damage**, give **+40% attack speed**, give **+30% movement speed**, and **reduce** all other skill cooldowns by **3s**. <details>You are able to parry from the frame you start the move and until the parry is released. The maximum duration you can hold the parry before releasing is 1 second. You cannot parry some types of damage, including but not limited to blood shrines, void explosions, DOT effects like bleed, or fall damage (though the move does negate fall damage by slowing you down). 3 second cooldown. This skill's cooldown is not reduced from successfully parrying.</details>  |
| Utility - **Boost** | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texBoostIcon.png?raw=true" width="128"> | **Spend boost meter** to move 35% faster than normal. If **health is near full**, move 65% faster instead. If **airborne**, do a short **mid-air dash**.<details>While boosting, your movement speed is increased by 35% and you are given 50 armor. If your health is above 90%, you will power boost, increasing speed by 65%. Activating boost gives invincibility for a brief moment. Boosting will drain your boost meter. If the boost meter runs out, you will be unable to boost again until the meter is recharged. The boost meter recharges overtime when not boosting. % based cooldown reduction, such as Alien Head or Brainstalks, will reduce the speed at which the boost meter is drained and increase the speed it comes back. Flat cooldown reduction, such as Purity, will increase the max capacity of the boost meter. By default, you can use boost as a mid-air dash once before having to touch the ground. Any additional utility stocks, such as those from Hardlight Afterburner, will let you use the mid-air dash more times before having to touch the ground.</details> |
| Scepter Utility - **Thundering Boost** | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texScepterBoostIcon.png?raw=true" width="128"> | **Spend boost meter** to move 35% faster than normal. If **health is near full**, move 65% faster instead. If **airborne**, do a short **mid-air dash**. **SCEPTER:** Run into enemies to deal **900% damage**. Damage increases based on your **movement speed**. <details>Functions the same as normal boost aside from the extra damage. The damage deals 900% assuming default movement speed Sonic with power boost.</details> |
| Special - **Grand Slam** | <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/texGrandSlamIcon.png?raw=true" width="128"> | **Homing**. Dash forward into an enemy to attack with 140% damage **repeatedly** before unleashing a powerful attack from above dealing 2200% damage.<details>This attack will home in on the enemy closest to the crosshair, similar to the homing attack. The amount of repeated weak hits you will do is 5 by default, but increases with attack speed. After landing the initial dash attack, you will have invincibility for the rest of the move's duration. 12 second cooldown.</details> |

# <img src="https://github.com/DS-GAMING-SE/Sonic/blob/master/SonicUnityProject/Assets/SonicAssets/Icons/Emeralds/texGreenEmeraldIcon.png?raw=true" width="64"> Super Sonic

## Chaos Emeralds
If anyone is playing as Sonic and Artifact of Metamorphosis is **off**, Chaos Emeralds will spawn scattered around every normal stage. The emeralds must be purchased similarly to a large chest. Three Chaos Emeralds will spawn per stage, except if you're playing Simulacrum, where there will be five. You don't need one person to collect all Chaos Emeralds, as long as the players cumulatively have all seven, everyone will be able to transform.

## Super Sonic
Once all seven Chaos Emeralds have been collected, anyone playing as Sonic can transform into Super Sonic by pressing **V** (Keybind can be changed in the config). Super Sonic gives new upgraded skills, flight, invincibility, and incredible power for 50 seconds. Though this power is unmatched, it takes multiple stages to collect the emeralds, so it is important to use this power wisely. If you're playing with other Sonics, all of them will be able to transform into Super Sonic if everyone transforms within ten seconds of the first Sonic transforming.

# Compatible Mods
- CustomEmotesAPI
- BetterUI
- RiskOfOptions
- StandaloneAncientScepter (Upgrades utility skill)

# Special Thanks
- Presti (Lots of help with Super Sonic code)
- FORCED_REASSEMBLY (Rigging the mastery skin)
- Shader Forge (A tool that let me make the custom Chaos Emerald shaders without losing my mind https://github.com/CuteWaterBeary/ShaderForge)
- Sandwich (Writing the Sonic logbook)

# For unfinished builds
You can get the latest builds from over [here](/actions/workflows/cli.yml?query=branch%3Amaster)
In there you will see a list of builds that have been created select the latest one and you should see a Artifact called ``DLL``, that Artifact will upon you clicking on it download a zip file containing the Mod DLL.
If you want the full zip containing the mod manifest and Icon you will need to download the ``ds_gaming-SonicTheHedgehog`` Artifact instead.
