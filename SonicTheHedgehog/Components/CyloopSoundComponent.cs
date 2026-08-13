using HedgehogUtils.Voicelines;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SonicTheHedgehog.Components
{
    public class CyloopSoundComponent : MonoBehaviour
    {
        public string soundString;
        private EffectManagerHelper emh;

        public static Dictionary<string, List<Vector3>> soundStringToSoundPosition = new Dictionary<string, List<Vector3>>();

        private void OnEnable()
        {
            if (!emh)
            {
                emh = GetComponent<EffectManagerHelper>();
                if (emh)
                {
                    emh.OnEffectActivated += Sound;
                }
                else
                {
                    Sound();
                }
            }
        }
        private void Sound()
        {
            if (soundStringToSoundPosition.ContainsKey(soundString))
            {
                soundStringToSoundPosition.GetValueOrDefault(soundString)?.Add(transform.position);
            }
            else
            {
                StartCoroutine(CreateNewSound(soundString, transform.position));
            }
        }
        private static IEnumerator CreateNewSound(string soundString, Vector3 position)
        {
            soundStringToSoundPosition.Add(soundString, new List<Vector3> { position });
            yield return null;
            AkGameObj emitter = RoR2.Audio.PointSoundManager.RequestEmitter();
            List<Vector3> soundSources = soundStringToSoundPosition.GetValueOrDefault(soundString);
            if (soundSources != null)
            {
                AkPositionArray positions = new AkPositionArray((uint)soundSources.Count);
                for (int i = 0; i < soundSources.Count; i++)
                {
                    positions.Add(soundSources[i], Vector3.forward, Vector3.up);
                }
                AkSoundEngine.SetMultiplePositions(AkSoundEngine.GetAkGameObjectID(emitter.gameObject), positions, (ushort)soundSources.Count, AkMultiPositionType.MultiPositionType_MultiDirections, AkSetPositionFlags.AkSetPositionFlags_Emitter);
                AkSoundEngine.PostEvent(soundString, emitter.gameObject, 1u, OnSoundEnd, emitter);
                soundStringToSoundPosition.Remove(soundString);
            }
        }
        private static void OnSoundEnd(object in_cookie, AkCallbackType in_type, object in_info)
        {
            if (in_type == AkCallbackType.AK_EndOfEvent)
            {
                AkGameObj obj = (AkGameObj)in_cookie;
                AkSoundEngine.SetObjectPosition(AkSoundEngine.GetAkGameObjectID(obj.gameObject), Vector3.zero, Vector3.forward, Vector3.up);
                RoR2.Audio.PointSoundManager.FreeEmitter(obj);
            }
        }
    }
}
