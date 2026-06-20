using HedgehogUtils.Forms;
using HedgehogUtils.Forms.SuperForm;
using HedgehogUtils.Voicelines;
using HG;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace SonicTheHedgehog.Components
{
    internal class SonicVoicelineComponent : VoicelineComponent
    {
        public static bool stageRankingModFound = false;

        public FormComponent formComponent;
        public Run.FixedTimeStamp fastBossKillTimer;
        public const float timeForFastBossKill = 15f;

        #region Voicelines
        public static NetworkSoundEventDef lobby1;
        public static NetworkSoundEventDef lobby2;
        public static NetworkSoundEventDef lobby3;
        public static NetworkSoundEventDef lobby4;
        public static NetworkSoundEventDef lobby5;

        public static NetworkSoundEventDef bossDefeat1;
        public static NetworkSoundEventDef bossDefeat2;
        public static NetworkSoundEventDef bossDefeat3;
        public static NetworkSoundEventDef bossDefeat4;
        public static NetworkSoundEventDef bossDefeat5;
        public static NetworkSoundEventDef bossDefeat6;
        public static NetworkSoundEventDef[] bossDefeats;
        public static NetworkSoundEventDef bossDefeatFast;

        public static NetworkSoundEventDef transform1;
        public static NetworkSoundEventDef transform2;
        public static NetworkSoundEventDef[] transforms;
        #region Final Bosses
        public static NetworkSoundEventDef finalBossStartGeneric;
        public static NetworkSoundEventDef finalBossPhase;

        public static NetworkSoundEventDef mithrixHammer;
        public static NetworkSoundEventDef mithrixWhoIAm;

        public static NetworkSoundEventDef voidlingTitan;
        public static NetworkSoundEventDef voidlingFreedom;

        public static NetworkSoundEventDef falseSonStart;
        public static NetworkSoundEventDef falseSonLightning;

        public static NetworkSoundEventDef solusWingStart1;
        public static NetworkSoundEventDef solusWingStart2;
        public static NetworkSoundEventDef[] solusWingStarts;

        public static NetworkSoundEventDef neuralSanctumEnter;
        public static NetworkSoundEventDef solusHeartStart;

        public static NetworkSoundEventDef doppelgangerStart1;
        public static NetworkSoundEventDef doppelgangerStart2;
        public static NetworkSoundEventDef doppelgangerStart3;
        public static NetworkSoundEventDef doppelgangerStart4;
        public static NetworkSoundEventDef[] doppelgangerStarts;

        public static NetworkSoundEventDef finalBossDefeat1;
        public static NetworkSoundEventDef finalBossDefeat2;
        public static NetworkSoundEventDef finalBossDefeat3;
        public static NetworkSoundEventDef[] finalBossDefeats;
        #endregion
        #endregion
        public void Initialize()
        {
            stageRankingModFound = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(StageRanking.StageRankingPlugin.PluginGUID);
            lobby1 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_lobby_1");
            lobby2 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_lobby_2");
            lobby3 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_lobby_3");
            lobby4 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_lobby_4");

            bossDefeat1 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_boss_defeat_1");
            bossDefeat2 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_boss_defeat_2");
            bossDefeat3 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_boss_defeat_3");
            bossDefeat4 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_boss_defeat_4");
            bossDefeat5 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_boss_defeat_5");
            bossDefeat6 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_boss_defeat_6");
            bossDefeats = new[] { bossDefeat1, bossDefeat2, bossDefeat3, bossDefeat4, bossDefeat5, bossDefeat6 };
            bossDefeatFast = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_boss_defeat_fast");

            transform1 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_now_ill_show_you");
            transform2 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_chaos_emeralds");
            transforms = new[] { transform1, transform2 };

            finalBossStartGeneric = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_final_boss_start");
            finalBossPhase = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_final_boss_phase_defeat");

            mithrixHammer = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_mithrix_start");
            mithrixWhoIAm = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_mithrix_who_i_am");

            voidlingTitan = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_voidling_start");
            voidlingFreedom = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_voidling_freedom");

            falseSonStart = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_false_son_start");
            falseSonLightning = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_false_son_lightning");

            solusWingStart1 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_soluswing_1");
            solusWingStart2 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_soluswing_2");
            solusWingStarts = new[] { solusWingStart1, solusWingStart2 };

            neuralSanctumEnter = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_neural_sanctum");
            solusHeartStart = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_solus_heart_start");

            doppelgangerStart1 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_doppelganger_1");
            doppelgangerStart2 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_doppelganger_2");
            doppelgangerStart3 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_doppelganger_3");
            doppelgangerStart4 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_doppelganger_4");
            doppelgangerStarts = new[] { doppelgangerStart1, doppelgangerStart2, doppelgangerStart3, doppelgangerStart4 };

            finalBossDefeat1 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_final_boss_defeat_1");
            finalBossDefeat2 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_final_boss_defeat_2");
            finalBossDefeat3 = Modules.Assets.CreateNetworkSoundEventDef("Play_sonicthehedgehog_voiceline_final_boss_defeat_3");
            finalBossDefeats = new[] { finalBossDefeat1, finalBossDefeat2, finalBossDefeat3 };
        }
        public override void SubscribeEvents()
        {
            formComponent = base.GetComponent<FormComponent>();
            HedgehogUtils.Forms.EntityStates.GenericTransformationBase.OnGenericTransform += OnTransform;
            VoicelineManager.OnStageStart += OnStageStart;
            VoicelineManager.OnBossStart += OnBossStart;
            VoicelineManager.OnBossDefeated += OnBossDefeated;
            VoicelineManager.OnFinalBossStart += OnFinalBossStart;
            VoicelineManager.OnFinalBossDefeated += OnFinalBossDefeated;
            characterBody.onJump += new CharacterBody.JumpDelegate(OnJump);
            GlobalEventManager.onClientDamageNotified += TakeMajorDamage;
            if (stageRankingModFound && Util.HasEffectiveAuthority(gameObject)) SubscribeStageRanking();
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SubscribeStageRanking()
        {
            StageRanking.StageRankingPanel.OnStageRankingPanelEnd += OnStageRanking;
        }
        public override void UnsubscribeEvents()
        {
            HedgehogUtils.Forms.EntityStates.GenericTransformationBase.OnGenericTransform -= OnTransform;
            VoicelineManager.OnStageStart -= OnStageStart;
            VoicelineManager.OnBossStart -= OnBossStart;
            VoicelineManager.OnBossDefeated -= OnBossDefeated;
            VoicelineManager.OnFinalBossStart -= OnFinalBossStart;
            VoicelineManager.OnFinalBossDefeated -= OnFinalBossDefeated;
            characterBody.onJump -= new CharacterBody.JumpDelegate(OnJump);
            GlobalEventManager.onClientDamageNotified -= TakeMajorDamage;
            if (stageRankingModFound && Util.HasEffectiveAuthority(gameObject)) UnsubscribeStageRanking();
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void UnsubscribeStageRanking()
        {
            StageRanking.StageRankingPanel.OnStageRankingPanelEnd -= OnStageRanking;
        }
        private void TakeMajorDamage(DamageDealtMessage damage)
        {
            if (damage.victim && damage.victim == base.gameObject && !damage.isSilent && damage.damage >= characterBody.maxHealth * 0.34f)
            {
                PlayVoiceline("Play_sonicthehedgehog_voiceline_damage", VoicelinePriority.Skill);
            }
        }
        public override void OnDeathStart()
        {
            base.OnDeathStart();
            PlayVoiceline("Play_sonicthehedgehog_voiceline_death", VoicelinePriority.Dialogue);
        }
        private void OnJump()
        {
            PlayVoiceline("Play_sonicthehedgehog_voiceline_jump", VoicelinePriority.Any);
        }
        private void OnTransform(FormComponent formComponent, FormDef form)
        {
            if (Util.HasEffectiveAuthority(gameObject) && formComponent == this.formComponent && form == SuperFormDef.superFormDef)
            {
                // if during final boss, say now i'll show you, otherwise, chaos emerald power?
                PlayNetworkedVoiceline(transforms.GetRandom().index, VoicelinePriority.Dialogue);
            }
        }
        private void OnStageStart(Stage stage, List<NetworkedVoiceline> networkedVoicelines)
        {
            if (Stage.instance.sceneDef.cachedName == "solusweb")
            {
                networkedVoicelines.Add(new NetworkedVoiceline(this, neuralSanctumEnter.index, VoicelinePriority.PriorityDialogue));
                return;
            }
        }
        private void OnBossStart(BodyIndex boss, List<NetworkedVoiceline> networkedVoicelines)
        {
            if (boss == characterBody.bodyIndex)
            {
                networkedVoicelines.Add(new NetworkedVoiceline(this, doppelgangerStarts.GetRandom().index, VoicelinePriority.PriorityDialogue));
            }
            fastBossKillTimer = Run.FixedTimeStamp.now + timeForFastBossKill;
        }
        private void OnBossDefeated(BodyIndex boss, List<NetworkedVoiceline> networkedVoicelines)
        {
            if (!fastBossKillTimer.hasPassed)
            {
                networkedVoicelines.Add(new NetworkedVoiceline(this, bossDefeatFast.index, VoicelinePriority.PriorityDialogue));
            }
            else
            {
                networkedVoicelines.Add(new NetworkedVoiceline(this, bossDefeats.GetRandom().index, VoicelinePriority.PriorityDialogue));
            }
        }

        private void OnFinalBossStart(FinalBoss finalBoss, List<NetworkedVoiceline> networkedVoicelines)
        {
            switch (finalBoss)
            {
                case FinalBoss.Mithrix1:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, mithrixHammer.index, VoicelinePriority.PriorityDialogue));
                    return;
                case FinalBoss.Voidling1:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, voidlingTitan.index, VoicelinePriority.PriorityDialogue));
                    return;
                case FinalBoss.Voidling2:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, voidlingFreedom.index, VoicelinePriority.PriorityDialogue));
                    return;
                case FinalBoss.FalseSon1:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, falseSonStart.index, VoicelinePriority.PriorityDialogue));
                    return;
                case FinalBoss.SolusWing:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, solusWingStarts.GetRandom().index, VoicelinePriority.PriorityDialogue));
                    return;
                case FinalBoss.SolusHeart1:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, solusHeartStart.index, VoicelinePriority.PriorityDialogue));
                    return;
            }
            if (finalBoss == FinalBoss.Mithrix3 || finalBoss == FinalBoss.FalseSon3 || finalBoss == FinalBoss.Arraign2 || finalBoss == FinalBoss.SolusHeart3)
            {
                networkedVoicelines.Add(new NetworkedVoiceline(this, finalBossPhase.index, VoicelinePriority.PriorityDialogue));
                return;
            }
            if (finalBoss == FinalBoss.LunarScavenger || finalBoss == FinalBoss.Arraign1)
            {
                networkedVoicelines.Add(new NetworkedVoiceline(this, finalBossStartGeneric.index, VoicelinePriority.PriorityDialogue));
                return;
            }

        }
        private void OnFinalBossDefeated(FinalBoss finalBoss, List<NetworkedVoiceline> networkedVoicelines)
        {
            switch (finalBoss)
            {
                case FinalBoss.Mithrix1:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, mithrixWhoIAm.index, VoicelinePriority.PriorityDialogue));
                    return;
                case FinalBoss.FalseSon1:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, falseSonLightning.index, VoicelinePriority.PriorityDialogue));
                    return;
                case FinalBoss.Voidling2:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, finalBossPhase.index, VoicelinePriority.PriorityDialogue));
                    return;
            }
            if (finalBoss == FinalBoss.Mithrix4 || finalBoss == FinalBoss.Voidling3 || finalBoss == FinalBoss.FalseSon3 || finalBoss == FinalBoss.SolusWing || finalBoss == FinalBoss.SolusHeart3 || finalBoss == FinalBoss.LunarScavenger || finalBoss == FinalBoss.Arraign2)
            {
                networkedVoicelines.Add(new NetworkedVoiceline(this, finalBossDefeats.GetRandom().index, VoicelinePriority.PriorityDialogue));
                return;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void OnStageRanking(StageRanking.Ranking ranking)
        {
            switch (ranking)
            {
                case StageRanking.Ranking.S:
                    PlayVoiceline("Play_sonicthehedgehog_voiceline_ranking_s", VoicelinePriority.Dialogue);
                    break;
                case StageRanking.Ranking.A:
                    PlayVoiceline("Play_sonicthehedgehog_voiceline_ranking_a", VoicelinePriority.Dialogue);
                    break;
                case StageRanking.Ranking.B:
                    PlayVoiceline("Play_sonicthehedgehog_voiceline_ranking_b", VoicelinePriority.Dialogue);
                    break;
                case StageRanking.Ranking.C:
                    PlayVoiceline("Play_sonicthehedgehog_voiceline_ranking_c", VoicelinePriority.Dialogue);
                    break;
                case StageRanking.Ranking.D:
                    PlayVoiceline("Play_sonicthehedgehog_voiceline_ranking_d", VoicelinePriority.Dialogue);
                    break;
            }
        }
    }
}
