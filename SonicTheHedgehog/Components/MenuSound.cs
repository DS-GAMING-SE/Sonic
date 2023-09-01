using RoR2;
using UnityEngine;

namespace SonicTheHedgehog.Components
{
    public class MenuSound : MonoBehaviour
    {
        private uint playID;
        private void OnEnable()
        {
            this.Invoke("PlaySound", 0.05f);
        }
        
        private void PlaySound()
        {
            this.playID = Util.PlaySound("Play_brake", base.gameObject);
        }

        private void OnDestroy()
        {
            if (this.playID != 0) AkSoundEngine.StopPlayingID(this.playID);
        }
    }
}