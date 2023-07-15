using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SonicTheHedgehog.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef boostBuff;
        internal static BuffDef superSonicBuff;
        internal static BuffDef ballBuff;

        internal static void RegisterBuffs()
        {
            boostBuff = AddNewBuff("SonicBoostBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite, 
                new Color(0, 0.7f, 1), 
                false, 
                false);
            superSonicBuff = AddNewBuff("SuperSonic",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.yellow,
                false,
                false);
            ballBuff = AddNewBuff("SonicBallArmor",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                new Color(0, 0.35f, 1),
                false,
                false);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}