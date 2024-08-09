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

        private CharacterBody body;
        private EntityStateMachine bodyStateMachine;
        private GenericSkill passiveSkill;
        private ICharacterFlightParameterProvider flight;

        public bool momentumEquipped=false;

        public float momentum=0; // from -1 to 1

        private float desiredMomentum=0;
        private bool calced = false;

        private Vector3 prevVelocity = Vector3.zero;
        private static float cutoffAngle = 30f;

        private int frame= 0;
        private static int framesBetweenRecalc = 20;

        public static float speedMultiplier=1f;

        private static float aerialDecay = 0.2f;
        private static float groundDecay = 0.5f;
        private static float fastDecay = 0.8f;
        private static float slowDecay = 1.35f;

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
            else if (!calced)
            {
                calced = true;
                momentum = 0;
                body.MarkAllStatsDirty();
            }
        }

        private void CalculateMomentum()
        {
            // Flying (Thank you Starstorm 2 Back Thrusters for inspiration)
            if (flight!=null && flight.isFlying)
            {
                if (body.characterMotor.velocity != Vector3.zero && (bodyStateMachine.state.GetType() == typeof(SonicEntityState) || typeof(Boost).IsAssignableFrom(bodyStateMachine.state.GetType())))
                {
                    this.calced = false;
                    Vector3 velocity = Vector3.Normalize(body.characterMotor.velocity);
                    Vector3 prevVelocity = Vector3.Normalize(this.prevVelocity);
                    if (body.characterMotor.isGrounded)
                    {
                        velocity.y = 0;
                        prevVelocity.y = 0;
                        Vector3.Normalize(velocity);
                        Vector3.Normalize(prevVelocity);
                    }
                    if (body.inputBank.moveVector!=Vector3.zero)
                    {
                        desiredMomentum = Mathf.Lerp(1, -0.8f, Vector3.Angle(velocity, prevVelocity)/cutoffAngle);
                    }
                    else
                    {
                        desiredMomentum = 0f;
                    }
                    //Chat.AddMessage((Vector3.Angle(velocity, prevVelocity) / cutoffAngle).ToString());
                    MomentumCalculation(1.2f, 0.4f);
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
                this.prevVelocity = body.characterMotor.velocity;
                return;
            }
            


            // Not Flying
            if (body.characterMotor.velocity != Vector3.zero && body.characterMotor.isGrounded && (bodyStateMachine.state.GetType()==typeof(SonicEntityState) || typeof(Boost).IsAssignableFrom(bodyStateMachine.state.GetType())))
            {
                calced = false;
                Vector3 forward = VelocityOnGround(body.characterMotor.velocity); //body.characterMotor.moveDirection.normalized;
                float dot = Vector3.Dot(forward, Vector3.down);
                desiredMomentum = Mathf.Clamp(dot * 2f, -1f, 1f);
                MomentumCalculation(slowDecay, fastDecay);
            }
            else
            {
                desiredMomentum = 0;
                float momentumDecay = body.characterMotor.isGrounded ? groundDecay : aerialDecay;
                MomentumCalculation(momentumDecay, momentumDecay);
            }
        }

        private void OnSkillChanged(GenericSkill skill)
        {
            MomentumEquipped();
        }

        private void MomentumEquipped()
        {
            this.momentumEquipped = this.passiveSkill.skillDef == Modules.Survivors.SonicTheHedgehogCharacter.momentumPassiveDef;
            this.calced = false;
        }

        private Vector3 VelocityOnGround(Vector3 velocity)
        {
            velocity.y = 0;
            return Vector3.ProjectOnPlane(velocity, body.characterMotor.estimatedGroundNormal).normalized;
        }

        private void MomentumCalculation(float slowDecay, float fastDecay)
        {
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
                //Chat.AddMessage("mom " + momentum.ToString() + " des " + desiredMomentum.ToString() + " dot " + dot.ToString());
            }
            else
            {
                momentum = desiredMomentum;
                if (!calced)
                {
                    calced = true;
                    body.MarkAllStatsDirty();
                }
            }
        }
    }
}