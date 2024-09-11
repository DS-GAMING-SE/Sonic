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
    public abstract class TransformationBase : BaseSkillState
    {
        public FormDef form;

        protected abstract float duration
        {
            get;
        }

        protected bool effectFired = false;

        protected SuperSonicComponent superSonic;

        protected abstract string transformSoundString
        {
            get;
        }

        public bool fromTeamSuper;

        public override void OnEnter()
        {
            base.OnEnter();
            this.superSonic= base.GetComponent<SuperSonicComponent>();
            this.superSonic.superSonicState.SetNextStateToMain();
            this.form = superSonic.targetedForm;
            if (form != superSonic.activeForm)
            {
                if (duration > 0)
                {
                    if (BodyCatalog.GetBodyName(base.characterBody.bodyIndex) == "SonicTheHedgehog")
                    {
                        base.PlayAnimation("FullBody, Override", "Transform", "Roll.playbackRate", this.duration);
                    }
                    if (NetworkServer.active)
                    {
                        base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, duration, 1);
                    }
                }
            }
            else
            {
                effectFired = true;
                this.outer.SetNextStateToMain();
                return;
            }

        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= this.duration / 2 && !effectFired && this.superSonic)
            {
                Transform();
            }
           
            
            if (fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
            }
        }

        public virtual void Transform()
        {
            effectFired = true;
            Util.PlaySound(this.transformSoundString, base.gameObject);
            if (base.isAuthority)
            {
                //this.superSonic.superSonicState.SetNextState(new SuperSonic { form = Forms.superSonicDef });
                this.superSonic.SetNextForm(this.form);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Vehicle;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(fromTeamSuper);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            fromTeamSuper = reader.ReadBoolean();
        }
    }
}