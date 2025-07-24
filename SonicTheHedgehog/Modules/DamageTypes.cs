
using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SonicTheHedgehog.Modules
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType grandSlamJuggle;
        public static void Initialize()
        {
            grandSlamJuggle = DamageAPI.ReserveDamageType();
        }
    }
}