using EntityStates;
using HG;
using R2API;
using UnityEngine.Networking;
using SonicTheHedgehog.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BepInEx;
using RoR2;
using HarmonyLib;
using SonicTheHedgehog.Components;

namespace SonicTheHedgehog.Modules.Forms
{
    public static class Forms
    {
        public static FormDef superSonicDef;

        public static FormDef[] formsCatalog = Array.Empty<FormDef>();

        public static Dictionary<FormDef, Type> formToHandlerType = new Dictionary<FormDef, Type>();

        public static Dictionary<Type, GameObject> handlerTypeToPrefab = new Dictionary<Type, GameObject>();

        // Look at VarianceAPI for catalog coding
        public static void Initialize()
        {
            Dictionary<string, RenderReplacements> superRenderDictionary = new Dictionary<string, RenderReplacements>
            {
                { SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME", new RenderReplacements { material = Materials.CreateHopooMaterial("matSuperSonic"), mesh = Assets.mainAssetBundle.LoadAsset<GameObject>("SuperSonicMesh").GetComponent<SkinnedMeshRenderer>().sharedMesh } },
                { SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME", new RenderReplacements { material = Materials.CreateHopooMaterial("matSuperMetalSonic"), mesh = null } }
            };
            superSonicDef = CreateFormDef(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SUPER_FORM", Buffs.superSonicBuff, StaticValues.superSonicDuration,
                new RoR2.ItemDef[] { Items.yellowEmerald, Items.redEmerald, Items.blueEmerald, Items.cyanEmerald, Items.grayEmerald, Items.greenEmerald, Items.purpleEmerald }, true, true,
                1, true, true, true, new SerializableEntityStateType(typeof(SkillStates.SuperSonic)), superRenderDictionary, typeof(SuperSonicHandler),
                new AllowedBodyList { whitelist = true, bodyNames = new string[] { "SonicTheHedgehog" } });

            CatalogAddFormDefs(new FormDef[]
            {
                superSonicDef
            });
        }

        public static FormDef CreateFormDef(string name, RoR2.BuffDef buff, float duration, RoR2.ItemDef[] neededItems, bool shareItems, bool consumeItems, int maxTransforms, bool invincible, bool flight, bool superAnimations, SerializableEntityStateType formState, 
            Dictionary<string, RenderReplacements> renderDictionary, Type handlerComponent, AllowedBodyList allowedBodyList)
        {
            FormDef form = ScriptableObject.CreateInstance<FormDef>();
            form.name = name;
            form.buff = buff;
            form.duration = duration;
            form.neededItems = neededItems;
            form.shareItems = shareItems;
            form.consumeItems = consumeItems;
            form.maxTransforms = maxTransforms;
            form.invincible = invincible;
            form.flight = flight;
            form.superAnimations = superAnimations;
            form.formState = formState;
            form.renderDictionary = renderDictionary;
            if (!handlerComponent.IsAssignableFrom(typeof(SuperSonicHandler)))
            { 
                Debug.LogWarningFormat("handlerComponent of type {0} is not assignable from SuperSonicHandler. It's recommended that handlerComponent inherits from SuperSonicHandler", new object[] { handlerComponent.Name }); 
            };
            form.handlerComponent = handlerComponent;
            form.allowedBodyList = allowedBodyList;

            return form;
        }

        public static void CatalogAddFormDefs(FormDef[] forms)
        {
            int length = formsCatalog.Length;
            Array.Resize(ref formsCatalog, length + forms.Length);
            for (int i = 0; i < forms.Length; i++)
            {
                // Adding form to catalog
                formsCatalog[length + i] = forms[i];
                /*
                // Creating or reusing handler object prefab
                if (formToHandlerType.ContainsValue(forms[i].handlerComponent))
                {
                    formToHandlerType.Add(forms[i], forms[i].handlerComponent);
                    Debug.LogFormat("FormDef {0} added to catalog. Reuses {1} prefab", new object[] { forms[i].name, forms[i].handlerComponent.Name });
                }
                else
                {
                    GameObject handlerPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SuperSonicHandler");
                    handlerPrefab.AddComponent(forms[i].handlerComponent);
                    handlerPrefab.AddComponent<NetworkIdentity>();
                    handlerPrefab.name = forms[i].handlerComponent.Name;
                    PrefabAPI.RegisterNetworkPrefab(handlerPrefab);
                    handlerTypeToPrefab.Add(forms[i].handlerComponent, handlerPrefab);
                    Debug.LogFormat("FormDef {0} added to catalog. Created new {1} prefab", new object[] { forms[i].name, forms[i].handlerComponent.Name });
                }
                */
            }

            formsCatalog = formsCatalog.OrderBy(form => form.name).ToArray();

            Debug.LogFormat("FormDef(s) added to formCatalog. formCatalog now contains: {0}", new object[] { formsCatalog.ToString() });


        }

        public static FormDef GetFormDef(FormIndex formIndex)
        {
            return ArrayUtils.GetSafe(formsCatalog, (int)formIndex);
        }

        public static bool GetIsInForm(GameObject gameObject, FormDef form)
        {
            if (gameObject)
            {
                SuperSonicComponent component = gameObject.GetComponent<SuperSonicComponent>();
                if (component)
                {
                    return component.form == form;
                }
            }
            return false;
        }

        public static bool GetIsInForm(CharacterBody body, FormDef form)
        {
            if (body)
            {
                return GetIsInForm(body.gameObject, form);
            }
            return false;
        }
    }

    public class FormDef : ScriptableObject
    {
        //Name can't be seen by the player, only used for internal stuff

        [Tooltip("The buff given to you when you're transformed. This should be a buff unique to the form you're making.\nUse this buff for applying whatever stat increases you want.\nThis will be applied as a timed buff and will end the form when it goes away.")]
        public RoR2.BuffDef buff;

        [Tooltip("The duration of the transformation in seconds. The actual duration of the form may be slightly longer to account for the transformation animation. If duration is <=0, a normal buff will be used instead of a timed buff")]
        public float duration;

        [Tooltip("The item or items that are needed to transform.")]
        public RoR2.ItemDef[] neededItems;

        [Tooltip("If needed items will be shared amongst all players. Any player will be able to transform if any other player or combination of players have the needed items.")]
        public bool shareItems;

        [Tooltip("If needed items will be removed when transforming.")]
        public bool consumeItems;

        [Tooltip("The maximum number of times the same player is allowed to transform into this form every stage. If number is <=0, there will be no limit.")]
        public int maxTransforms;

        [Tooltip("If you will be immune to all damage while transformed.")]
        public bool invincible;

        [Tooltip("If you will be able to fly while transformed.")]
        public bool flight;

        [Tooltip("If you will use Super Sonic's animations or stay with default animations.\nSuper Sonic's animations include hovering in his idles, hovering when moving on the ground, replacing his \"falling\" animations with flying, and some animations made under the assumption that his quills are pointed up.")]
        public bool superAnimations;

        //EntityStateCatalog.InstantiateState()
        [Tooltip("The entity state used by the \"SonicForms\" entity state machine while transformed. Should be a subclass of SonicFormBase")]
        public SerializableEntityStateType formState;

        [Tooltip("Stores the material and mesh changes that will be applied when transforming based on what skin you're using.\nKey is the string token of the skin. RenderReplacements is a struct containing a material and mesh.\nPutting null for material or mesh will make them not change when transforming.")]
        public Dictionary<string, RenderReplacements> renderDictionary;

        [Tooltip("The component that will track information about your form, such as whether all necessary items have been collected. This component will be put on a gameObject that will always exist and persist between stages.\nIf you're unsure what to put here, use typeof(SuperSonicHandler).\nYou can create a subclass of SuperSonicHandler and put it here if you want to add code, such as an extra requirement for transforming.")]
        public Type handlerComponent;

        [Tooltip("Contains information on what characters are allowed to transform.\nIf whitelist, any body name listed under bodyNames will be allowed. If not whitelist, any body name not listed under bodyNames will be allowed.\nIf you're unsure about what body name means, look into BodyCatalog related stuff")]
        public AllowedBodyList allowedBodyList;

        public FormIndex formIndex { get { return (FormIndex)Array.IndexOf(Forms.formsCatalog, this); } }
    }

    public class AllowedBodyList
    {
        public bool whitelist;

        public string[] bodyNames;

        public bool BodyIsAllowed(string bodyName)
        {
            return !(whitelist ^ bodyNames.Contains(bodyName));
        }

        public bool BodyIsAllowed(BodyIndex bodyIndex)
        {
            return BodyIsAllowed(BodyCatalog.GetBodyName(bodyIndex));
        }
    }

    public struct RenderReplacements
    {
        public Material material;
        public Mesh mesh;
    }

    // Do not set this value. This value is set automatically at runtime
    public enum FormIndex
    {
        None = -1
    }
}