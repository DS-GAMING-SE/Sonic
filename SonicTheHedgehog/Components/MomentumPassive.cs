using EntityStates;
using R2API;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.SkillStates;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SonicTheHedgehog.Components
{
    public class MomentumPassive : MonoBehaviour
    {
        // Needs interaction for landing on a slope
        // Needs special interaction with flying. Flying in a straight line builds speed?

        private CharacterBody body;
        private EntityStateMachine bodyStateMachine;

        public bool momentumEquipped=true;

        public float momentum=0; // from -1 to 1

        private float desiredMomentum=0;
        private bool calced;

        private int frame= 0;
        private static int framesBetweenRecalc = 20;

        public static float speedMultiplier=1f;

        private static float aerialDecay = 0.2f;
        private static float groundDecay = 0.5f;
        private static float fastDecay = 0.8f;
        private static float slowDecay = 1.35f;

        private void Start()
        {
            body = GetComponent<CharacterBody>();
            bodyStateMachine = EntityStateMachine.FindByCustomName(body.gameObject, "Body");
            //MomentumEquipped();
            //body.skillLocator.FindSkillByFamilyName("Misc").onSkillChanged += OnSkillChanged;
        }

        private void FixedUpdate()
        {
            if (momentumEquipped)
            {
                frame = (frame + 1) % framesBetweenRecalc;
                if (frame == 0)
                {
                    CalculateMomentum();
                }
            }
        }

        private void CalculateMomentum()
        {
            if (body.characterMotor.velocity != Vector3.zero && body.characterMotor.isGrounded && (bodyStateMachine.state.GetType()==typeof(SonicEntityState) || bodyStateMachine.state.GetType() == typeof(Boost)))
            {
                calced = false;
                Vector3 velocity = body.characterMotor.velocity;
                velocity.y = 0;
                Vector3 forward = Vector3.ProjectOnPlane(velocity, body.characterMotor.estimatedGroundNormal).normalized; //body.characterMotor.moveDirection.normalized;
                float dot = Vector3.Dot(forward, Vector3.down);
                desiredMomentum = Mathf.Clamp(dot * 2f, -1f, 1f);
                if (Mathf.Abs(desiredMomentum - momentum) > 0.1f)
                {
                    if (desiredMomentum > momentum)
                    {
                        momentum = Mathf.Clamp(momentum + ((desiredMomentum - momentum) * Time.fixedDeltaTime * slowDecay * framesBetweenRecalc), -1f, desiredMomentum);
                    }
                    else
                    {
                        momentum = Mathf.Clamp(momentum + ((desiredMomentum - momentum) * Time.fixedDeltaTime * fastDecay * framesBetweenRecalc), desiredMomentum, 1f);
                    }
                    body.RecalculateStats();
                    //Chat.AddMessage("mom " + momentum.ToString() + " des " + desiredMomentum.ToString());
                }
                else
                {
                    momentum = desiredMomentum;
                    if (!calced)
                    {
                        calced = true;
                        body.RecalculateStats();
                    }
                }
            }
            else
            {
                if (Mathf.Abs(momentum) > 0.1f)
                {
                    float momentumDecay = body.characterMotor.isGrounded ? groundDecay : aerialDecay;
                    calced = false;
                    if (momentum > 0)
                    {
                        momentum = Mathf.Clamp(momentum - (momentumDecay * Time.fixedDeltaTime * framesBetweenRecalc), 0, 1);
                    }
                    else
                    {
                        momentum = Mathf.Clamp(momentum + (momentumDecay * Time.fixedDeltaTime * framesBetweenRecalc), -1, 0);
                    }
                    body.RecalculateStats();
                    //Chat.AddMessage("mom " + momentum.ToString());
                }
                else
                {
                    momentum = 0;
                    if (!calced)
                    {
                        calced = true;
                        body.RecalculateStats();
                    }
                }
            }
        }

        private void OnSkillChanged(GenericSkill skill)
        {
            MomentumEquipped();
        }

        private void MomentumEquipped()
        {
            momentumEquipped = body.skillLocator.FindSkillByFamilyName("Misc").skillNameToken == "DS_GAMING_SONIC_THE_HEDGEHOG_BODY_MOMENTUM_PASSIVE_NAME";
        }
    }
}