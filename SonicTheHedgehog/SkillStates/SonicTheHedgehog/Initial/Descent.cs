using EntityStates;
using HedgehogUtils.Voicelines;
using RoR2;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates.Initial
{
    public class Descent : BaseState
    {
        public const float duration = 3f;
        public const float screamDurationPercent = 0.5f;
        private bool voicelinePlayed;
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(DLC3Content.Buffs.Untargetable);
                base.characterBody.AddBuff(RoR2Content.Buffs.Intangible);
            }    
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = true;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration * screamDurationPercent && !voicelinePlayed)
            {
                voicelinePlayed = true;
                VoicelineComponent.TryPlayVoiceline(gameObject, "Play_sonicthehedgehog_voiceline_stage_intro", VoicelinePriority.Dialogue);
            }
            if (base.isAuthority && fixedAge >= duration)
            {
                this.outer.SetNextState(new Landed());
            }
        }
        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(DLC3Content.Buffs.Untargetable);
                base.characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
            }
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
            }
            base.OnExit();
        }
    }
}
