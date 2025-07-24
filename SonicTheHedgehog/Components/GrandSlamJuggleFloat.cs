using EmotesAPI;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.Components
{
    public class GrandSlamJuggleFloat : MonoBehaviour
    {
        public CharacterBody body;
        
        public void Start()
        {
            body = GetComponent<CharacterBody>();
            if (body.characterMotor)
            {
                body.characterMotor.velocity.x = 0;
                body.characterMotor.velocity.z = 0;
            }
        }

        public void FixedUpdate()
        {
            if (body && body.characterMotor && body.HasBuff(Modules.Buffs.grandSlamJuggleDebuff))
            {
                body.characterMotor.velocity.y = Mathf.Clamp(body.characterMotor.velocity.y, -2, 2);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}