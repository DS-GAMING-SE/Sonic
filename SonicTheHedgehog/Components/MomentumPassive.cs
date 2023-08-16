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
        // Stupid passive skill def whatever
        // Needs interaction for landing on a slope
        // Needs special interaction with flying. Flying in a straight line builds speed?

        private CharacterBody body;
        private EntityStateMachine bodyStateMachine;
        private GenericSkill passiveSkill;
        private ICharacterFlightParameterProvider flight;

        public bool momentumEquipped=false;

        public float momentum=0; // from -1 to 1

        private float desiredMomentum=0;
        private bool calced;
        private Vector3 prevVelocity = Vector3.zero;

        private int frame= 0;
        private static int framesBetweenRecalc = 20;

        public static float speedMultiplier=1f;

        private static float aerialDecay = 0.2f;
        private static float groundDecay = 0.5f;
        private static float fastDecay = 0.8f;
        private static float slowDecay = 1.35f;

        private static float flyingAccuracy = 0.9f;

        private void Start()
        {
            this.body = GetComponent<CharacterBody>();
            this.bodyStateMachine = EntityStateMachine.FindByCustomName(body.gameObject, "Body");
            this.passiveSkill = body.skillLocator.FindSkillByFamilyName("SonicTheHedgehogMiscFamily");
            this.flight = body.GetComponent<ICharacterFlightParameterProvider>();
            if (this.passiveSkill)
            {
                MomentumEquipped();
                this.passiveSkill.onSkillChanged += OnSkillChanged;
            }
        }

        private void FixedUpdate()
        {
            if (this.momentumEquipped)
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
            // Flying
            /*if (flight!=null && flight.isFlying)
            {
                if (body.characterMotor.velocity != Vector3.zero && (bodyStateMachine.state.GetType() == typeof(SonicEntityState) || bodyStateMachine.state.GetType() == typeof(Boost)))
                {
                    calced = false;
                    Vector3 velocity = body.characterMotor.velocity;
                    Vector3 lastVelocity = prevVelocity;
                    if (body.characterMotor.isGrounded)
                    {
                        velocity = VelocityOnGround(ref velocity);
                        lastVelocity = VelocityOnGround(ref prevVelocity);
                    }
                    float dot = Vector3.Dot(velocity, lastVelocity);
                    float accuracy = dot * Time.fixedDeltaTime * framesBetweenRecalc;
                    if (dot > flyingAccuracy)
                    {
                        desiredMomentum = 1f;
                    }
                    else
                    {
                        desiredMomentum = 0f;
                    }

                    if (Mathf.Abs(desiredMomentum - momentum) > 0.1f)
                    {
                        if (desiredMomentum > momentum)
                        {
                            momentum = Mathf.Clamp(momentum + ((desiredMomentum - momentum) * Time.fixedDeltaTime * 1 * framesBetweenRecalc), -0.5f, desiredMomentum);
                        }
                        else
                        {
                            momentum = Mathf.Clamp(momentum + ((desiredMomentum - momentum) * Time.fixedDeltaTime * 1 * framesBetweenRecalc), desiredMomentum, 1f);
                        }
                        body.MarkAllStatsDirty();
                        prevVelocity = body.characterMotor.velocity;
                        Chat.AddMessage("mom " + momentum.ToString() + " des " + desiredMomentum.ToString() + " dot " + dot.ToString());
                    }
                    else
                    {
                        momentum = desiredMomentum;
                        if (!calced)
                        {
                            calced = true;
                            body.MarkAllStatsDirty();
                            prevVelocity = body.characterMotor.velocity;
                        }
                    }
                }
                else
                {
                    momentum = 0;
                    if (!calced)
                    {
                        calced = true;
                        body.MarkAllStatsDirty();
                        prevVelocity = body.characterMotor.velocity;
                        //body.RecalculateStats();
                    }
                }
                return;
            }
            */

            // Not Flying
            if (body.characterMotor.velocity != Vector3.zero && body.characterMotor.isGrounded && (bodyStateMachine.state.GetType()==typeof(SonicEntityState) || bodyStateMachine.state.GetType() == typeof(Boost)))
            {
                calced = false;
                Vector3 forward = VelocityOnGround(ref body.characterMotor.velocity); //body.characterMotor.moveDirection.normalized;
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
                    body.MarkAllStatsDirty();
                    //body.RecalculateStats();
                    //Chat.AddMessage("mom " + momentum.ToString() + " des " + desiredMomentum.ToString());
                }
                else
                {
                    momentum = desiredMomentum;
                    if (!calced)
                    {
                        calced = true;
                        body.MarkAllStatsDirty();
                        //body.RecalculateStats();
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
                    body.MarkAllStatsDirty();
                    //body.RecalculateStats();
                    //Chat.AddMessage("mom " + momentum.ToString());
                }
                else
                {
                    momentum = 0;
                    if (!calced)
                    {
                        calced = true;
                        body.MarkAllStatsDirty();
                        //body.RecalculateStats();
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
            this.momentumEquipped = this.passiveSkill.skillDef == Modules.Survivors.SonicTheHedgehogCharacter.momentumPassiveDef;
        }

        private Vector3 VelocityOnGround(ref Vector3 velocity)
        {
            velocity.y = 0;
            return Vector3.ProjectOnPlane(velocity, body.characterMotor.estimatedGroundNormal).normalized;
        }
    }
}