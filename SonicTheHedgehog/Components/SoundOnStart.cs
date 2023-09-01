using EmotesAPI;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.Components
{
    public class SoundOnStart : MonoBehaviour
    {
        public string soundString;
        private void Start()
        {
            Util.PlaySound(soundString, base.gameObject);
        }
    }
}