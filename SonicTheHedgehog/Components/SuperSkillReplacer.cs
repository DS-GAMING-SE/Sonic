using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using HedgehogUtils.Forms;
using RoR2.Skills;
using SonicTheHedgehog.Modules.Survivors;
using SonicTheHedgehog.SkillStates;

namespace SonicTheHedgehog.Components
{
    public class SuperSkillReplacer : MonoBehaviour
    {
        private FormComponent formComponent;
        private CharacterBody body;
        private HomingTracker homingTracker;

        public static Modules.SkillDefs.RequiresFormMeleeSkillDef melee;

        public static SkillDefs.RequiresFormSkillDef sonicBoom;

        public static SkillDefs.RequiresFormSkillDef parry;
        public static SkillDefs.RequiresFormSkillDef afterIDWAttack;

        public static HedgehogUtils.Boost.SkillDefs.RequiresFormBoostSkillDef boost;

        public static SkillDefs.RequiresFormSkillDef grandSlam;

        public void Start()
        {
            formComponent = base.GetComponent<FormComponent>();
            if (formComponent)
            {
                body = base.GetComponent<CharacterBody>();
                homingTracker = base.GetComponent<HomingTracker>();
                formComponent.OnFormChanged += FormChanged;
            }
            else
            {
                Destroy(this);
            }
        }

        public void OnDestroy()
        {
            formComponent.OnFormChanged -= FormChanged;
        }

        public void FormChanged(FormDef previous, FormDef active)
        {
            if (!Util.HasEffectiveAuthority(base.gameObject)) { return; }
            if (active == HedgehogUtils.Forms.SuperForm.SuperFormDef.superFormDef)
            {
                SkillOverrides();
                homingTracker.SetColors(SonicTheHedgehogCharacter.superSonicColor, SonicTheHedgehogCharacter.superSonicColor2);
            }
            // Removal of Super Upgrade skills is built into the HedgehogUtils.Forms.SkillDefs.RequiresFormSkillDef type of SkillDef that all the super upgrades are, so it doesn't need to be done manually

            else if (previous == HedgehogUtils.Forms.SuperForm.SuperFormDef.superFormDef)
            {
                homingTracker.SetColors(SonicTheHedgehogCharacter.sonicColor2, SonicTheHedgehogCharacter.sonicColor);
            }
        }

        public void SkillOverrides()
        {
            if (!body.skillLocator) { return; }
            SkillHelper(this, body.skillLocator.primary, SonicTheHedgehogCharacter.primarySkillDef, melee);
            if (!SkillHelper(this, body.skillLocator.secondary, SonicTheHedgehogCharacter.sonicBoomSkillDef, sonicBoom))
            {
                SkillHelper(this, body.skillLocator.secondary, SonicTheHedgehogCharacter.parrySkillDef, parry);

            }
            SkillHelper(this, body.skillLocator.utility, SonicTheHedgehogCharacter.boostSkillDef, boost);
            SkillHelper(this, body.skillLocator.special, SonicTheHedgehogCharacter.grandSlamSkillDef, grandSlam);
        }

        public void IDWAttackActivated()
        {
            body.skillLocator.secondary.SetSkillOverride(this, afterIDWAttack, GenericSkill.SkillOverridePriority.Contextual);
        }


        public static bool SkillHelper(object source, GenericSkill slot, SkillDef original, SkillDef upgrade)
        {
            return SkillHelper(source, slot, original, upgrade, true);
        }

        public static bool SkillHelper(object source, GenericSkill slot, SkillDef original, SkillDef upgrade, bool set)
        {
            if (slot)
            {
                if (slot.baseSkill == original)
                {
                    if (set)
                    {
                        slot.SetSkillOverride(source, upgrade, GenericSkill.SkillOverridePriority.Upgrade);
                        return true;
                    }
                    else
                    {
                        slot.UnsetSkillOverride(source, upgrade, GenericSkill.SkillOverridePriority.Upgrade);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
