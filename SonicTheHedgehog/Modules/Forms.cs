using BepInEx.Configuration;
using EntityStates;
using IL.RoR2;
using RiskOfOptions.Options;
using RiskOfOptions.Utils;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using SonicTheHedgehog.Modules.Survivors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicTheHedgehog.Modules
{
    public static class Forms
    {
        public static FormDef superSonicDef;

        public static FormDef[] formsCatalog = Array.Empty<FormDef>();

        public static void Initialize()
        {
            Dictionary<string, RenderReplacements> superRenderDictionary = new Dictionary<string, RenderReplacements>
            {
                { SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME", new RenderReplacements { material = Materials.CreateHopooMaterial("matSuperSonic"), mesh = Assets.mainAssetBundle.LoadAsset<GameObject>("SuperSonicMesh").GetComponent<SkinnedMeshRenderer>().sharedMesh } },
                { SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME", new RenderReplacements { material = Materials.CreateHopooMaterial("matSuperMetalSonic"), mesh = null } }
            };
            superSonicDef = CreateFormDef(Buffs.superSonicBuff, StaticValues.superSonicDuration,
                new RoR2.ItemDef[] { Items.yellowEmerald, Items.redEmerald, Items.blueEmerald, Items.cyanEmerald, Items.grayEmerald, Items.greenEmerald, Items.purpleEmerald }, true, true,
                true, true, true, new SerializableEntityStateType(typeof(SkillStates.SuperSonic)), superRenderDictionary);

            CatalogAddFormDefs(new FormDef[]
            {
                superSonicDef
            });
        }

        public static FormDef CreateFormDef(RoR2.BuffDef buff, float duration, RoR2.ItemDef[] neededItems, bool shareItems, bool consumeItems, bool invincible, bool flight, bool superAnimations, SerializableEntityStateType formState, 
            Dictionary<string, RenderReplacements> renderDictionary)
        {
            FormDef form = ScriptableObject.CreateInstance<FormDef>();
            form.buff = buff;
            form.duration = duration;
            form.neededItems = neededItems;
            form.shareItems = shareItems;
            form.consumeItems = consumeItems;
            form.invincible = invincible;
            form.flight = flight;
            form.superAnimations = superAnimations;
            form.formState = formState;
            form.renderDictionary = renderDictionary;

            return form;
        }

        public static void CatalogAddFormDefs(FormDef[] forms)
        {
            int length = formsCatalog.Length;
            Array.Resize<FormDef>(ref formsCatalog, length + forms.Length);
            for (int i = 0; i < forms.Length; i++)
            {
                formsCatalog[length + i] = forms[i];
            }
        }
    }

    public class FormDef : ScriptableObject
    {
        [Tooltip("The buff given to you when you're transformed. Use this buff for applying whatever stat increases you want. This will be applied as a timed buff and will end the form when it goes away.")]
        public RoR2.BuffDef buff;

        [Tooltip("The duration of the transformation in seconds. The actual duration of the form may be longer to account for the transformation animation. If duration is <=0, a normal buff will be used instead of a timed buff")]
        public float duration;

        [Tooltip("The item or items that are needed to transform.")]
        public RoR2.ItemDef[] neededItems;

        [Tooltip("If needed items will be shared amongst all players. Any player will be able to transform if any other player or combination of players have the needed items.")]
        public bool shareItems;

        [Tooltip("If needed items will be removed when transforming.")]
        public bool consumeItems;

        [Tooltip("If you will be immune to all damage while transformed.")]
        public bool invincible;

        [Tooltip("If you will be able to fly while transformed.")]
        public bool flight;

        [Tooltip("If you will use Super Sonic's animations or stay with default animations. Super Sonic's animations include hovering in his idles, hovering when moving on the ground, replacing his \"falling\" animations with flying, and some animations made under the assumption that his quills are pointed up.")]
        public bool superAnimations;

        //EntityStateCatalog.InstantiateState()
        [Tooltip("The entity state used by the \"SonicForms\" entity state machine while transformed. Should be a subclass of SonicFormBase")]
        public SerializableEntityStateType formState;

        [Tooltip("Stores the material and mesh changes that will be applied when transforming. Key is the string token of the skin. Putting null for material or mesh will make them not change when transforming.")]
        public Dictionary<string, RenderReplacements> renderDictionary;

        [Tooltip("")]
        public Dictionary<SkillDef, SkillDef> skillOverrideDictionary;

    }

    public struct RenderReplacements
    {
        public Material material;
        public Mesh mesh;
    }
}