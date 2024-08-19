using EntityStates;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.Modules.Forms;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SonicTheHedgehog.SkillStates
{
    public class SuperSonicTransformation : TransformationBase
    {
        protected override float duration => 2.4f;

        protected override string transformSoundString => "Play_super_transform";

        private static float cameraDistance = -7;
        private CharacterCameraParamsData cameraParams = new CharacterCameraParamsData
        {
            maxPitch = 70f,
            minPitch = -70f,
            pivotVerticalOffset = 0.5f,
            idealLocalCameraPos = new Vector3(0f, 0f, cameraDistance),
            wallCushion = 0.1f
        };
        private CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;


        public override void OnEnter()
        {
            base.OnEnter();
            if (!fromTeamSuper)
            {
                Util.PlaySound("Play_emerald", base.gameObject);
                if (base.isAuthority)
                {
                    EffectManager.SimpleEffect(Modules.Assets.transformationEmeraldSwirl, base.gameObject.transform.position, base.gameObject.transform.rotation, true);
                }
            }

            this.camOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = this.cameraParams,
                priority = 1f
            }, duration / 2f);

        }

        public override void Transform()
        {
            base.Transform();
            base.cameraTargetParams.RemoveParamsOverride(this.camOverrideHandle, 0.2f);
        }
    }
}