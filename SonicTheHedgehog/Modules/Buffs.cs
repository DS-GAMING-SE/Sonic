using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SonicTheHedgehog.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef boostBuff;
        internal static BuffDef superBoostBuff;
        internal static BuffDef ballBuff;
        internal static BuffDef parryBuff;
        internal static BuffDef superParryDebuff;
        internal static BuffDef grandSlamJuggleDebuff;

        internal static void RegisterBuffs()
        {
            boostBuff = AddNewBuff("bdSonicBoost",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite, 
                new Color(0, 0.7f, 1), 
                false, 
                false);
            superBoostBuff = AddNewBuff("bdSuperSonicBoost",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                new Color(1f, 0.9f, 0),
                false,
                false);
            ballBuff = AddNewBuff("bdSonicBallArmor",
                //LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Assets.mainAssetBundle.LoadAsset<Sprite>("texBallBuffIcon"),
                new Color(0, 0.35f, 1),
                false,
                false);
            parryBuff = AddNewBuff("bdSonicParry",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/WarCryBuff").iconSprite,
                new Color(0, 0.7f, 1),
                false,
                false);
            superParryDebuff = AddNewBuff("bdSonicSuperParryDebuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Weak").iconSprite,
                new Color(0, 0.2f, 0.6f),
                false,
                true);
            grandSlamJuggleDebuff = AddNewBuff("bdSonicSuperParryDebuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Weak").iconSprite,
                new Color(0, 0.2f, 0.6f),
                false,
                true,
                true);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            return AddNewBuff(buffName, buffIcon, buffColor, canStack, isDebuff, false);
        }
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff, bool isHidden)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;
            buffDef.isHidden = isHidden;

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}