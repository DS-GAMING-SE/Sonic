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

[assembly: HG.Reflection.SearchableAttribute.OptIn]
namespace SonicTheHedgehog.Modules.Forms
{
    public static class Forms
    {
        public static FormDef superSonicDef;

        public static FormDef testFormDef;

        public static FormDef[] formsCatalog = Array.Empty<FormDef>();

        public static Dictionary<FormDef, GameObject> formToHandlerPrefab = new Dictionary<FormDef, GameObject>();

        public static Dictionary<FormDef, GameObject> formToHandlerObject = new Dictionary<FormDef, GameObject>();

        // Look at VarianceAPI for catalog coding
        // Stuff broke from SystemInitializer stuff but idk why
        public static void Initialize()
        {
            Dictionary<string, RenderReplacements> superRenderDictionary = new Dictionary<string, RenderReplacements>
            {
                { SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME", new RenderReplacements { material = Materials.CreateHopooMaterial("matSuperSonic"), mesh = Assets.mainAssetBundle.LoadAsset<GameObject>("SuperSonicMesh").GetComponent<SkinnedMeshRenderer>().sharedMesh } },
                { SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME", new RenderReplacements { material = Materials.CreateHopooMaterial("matSuperMetalSonic"), mesh = null } }
            };
            superSonicDef = CreateFormDef(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SUPER_FORM", Buffs.superSonicBuff, StaticValues.superSonicDuration, true, true, true,
                1, true, true, true, new SerializableEntityStateType(typeof(SkillStates.SuperSonic)), new SerializableEntityStateType(typeof(SkillStates.SuperSonicTransformation)), superRenderDictionary, SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PREFIX", 
                typeof(SuperSonicHandler), new AllowedBodyList { whitelist = false, bodyNames = Array.Empty<string>() });

            testFormDef = CreateFormDef(SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_TEST_FORM", Buffs.superSonicBuff, StaticValues.superSonicDuration, true, false, false,
                3, false, false, false, new SerializableEntityStateType(typeof(SkillStates.SonicFormBase)), new SerializableEntityStateType(typeof(SkillStates.SuperSonicTransformation)), new Dictionary<string, RenderReplacements>(), SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_SUPER_PREFIX",
                typeof(FormHandler), new AllowedBodyList { whitelist = false, bodyNames = Array.Empty<string>() });

            CatalogAddFormDefs(new FormDef[]
            {
                superSonicDef,
                testFormDef
            });
        }

        [SystemInitializer(typeof(ItemCatalog))]
        public static void InitializeFormItemRequirements()
        {
            superSonicDef.neededItems = new NeededItem[] { Items.yellowEmerald, Items.redEmerald, Items.blueEmerald, Items.cyanEmerald, Items.grayEmerald, Items.greenEmerald, Items.purpleEmerald };
            testFormDef.neededItems = new NeededItem[] { new NeededItem { item = RoR2Content.Items.Mushroom, count = 10 } };
        }

        // Look at the tooltips in the FormDef class for more information on what all of these parameters mean
        public static FormDef CreateFormDef(string name, BuffDef buff, float duration, bool requiresItems, bool shareItems, bool consumeItems, int maxTransforms, bool invincible, bool flight, bool superAnimations, SerializableEntityStateType formState, SerializableEntityStateType transformState, 
            Dictionary<string, RenderReplacements> renderDictionary, string transformationNameToken, Type handlerComponent, AllowedBodyList allowedBodyList)
        {
            FormDef form = ScriptableObject.CreateInstance<FormDef>();
            form.name = name;
            form.buff = buff;
            form.duration = duration;
            form.requiresItems = requiresItems;
            form.shareItems = shareItems;
            form.consumeItems = consumeItems;
            form.maxTransforms = maxTransforms;
            form.invincible = invincible;
            form.flight = flight;
            form.superAnimations = superAnimations;
            form.formState = formState;
            form.transformState = transformState;
            form.renderDictionary = renderDictionary;
            form.transformationNameToken = transformationNameToken;
            if (!(handlerComponent.IsSubclassOf(typeof(FormHandler)) || handlerComponent == typeof(FormHandler)))
            { 
                Debug.LogWarningFormat("handlerComponent of type {0} is not a subclass of FormHandler.", new object[] { handlerComponent.Name }); 
            };
            form.handlerComponent = handlerComponent;
            form.allowedBodyList = allowedBodyList;

            return form;
        }

        public static void CatalogAddFormDefs(FormDef[] forms)
        {
            Debug.LogFormat("Adding new FormDef(s) to catalog.\n {0}", new object[] { string.Concat(forms.Select(x => x.ToString() + "\n")) });
            int length = formsCatalog.Length;
            Array.Resize(ref formsCatalog, length + forms.Length);
            for (int i = 0; i < forms.Length; i++)
            {
                // Adding form to catalog
                formsCatalog[length + i] = forms[i];

                // Creating handler prefab
                GameObject handlerPrefab = PrefabAPI.InstantiateClone(Assets.mainAssetBundle.LoadAsset<GameObject>("SuperSonicHandler"), forms[i].name + " " + forms[i].handlerComponent.Name);
                FormHandler handlerComponent = (FormHandler) handlerPrefab.AddComponent(forms[i].handlerComponent);
                handlerComponent.form = forms[i];
                if (forms[i].requiresItems)
                {
                    handlerPrefab.AddComponent(forms[i].shareItems ? typeof(SyncedItemTracker) : typeof(UnsyncedItemTracker));
                }
                //PrefabAPI.RegisterNetworkPrefab(handlerPrefab);
                formToHandlerPrefab.Add(forms[i], handlerPrefab);
                Debug.LogFormat("FormDef {0} added to catalog. Created new {1} prefab", new object[] { forms[i].name, forms[i].handlerComponent.Name });

            }

            formsCatalog = formsCatalog.OrderBy(form => form.name).ToArray();

            Debug.LogFormat("FormDef(s) added to formCatalog. formCatalog now contains:\n {0}", new object[] { string.Concat(formsCatalog.Select(x => x.ToString() + "\n")) });


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
                    return component.activeForm == form;
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

        [SystemInitializer(typeof(BodyCatalog))]
        public static void SuperSonicComponentsForEveryone()
        {
            foreach (GameObject body in BodyCatalog.allBodyPrefabs)
            {
                foreach (FormDef form in formsCatalog)
                {
                    if (form.allowedBodyList.BodyIsAllowed(BodyCatalog.FindBodyIndex(body)))
                    {
                        body.AddComponent<SuperSonicComponent>();

                        EntityStateMachine superSonicState = body.AddComponent<EntityStateMachine>();
                        superSonicState.customName = "SonicForms";
                        superSonicState.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.BaseSonic));

                        NetworkStateMachine network = body.GetComponent<NetworkStateMachine>();
                        if (network)
                        {
                            Helpers.Append(ref network.stateMachines, new List<EntityStateMachine> { superSonicState });
                        }
                        break;
                    }
                }
            }
        }
    }

    public class FormDef : ScriptableObject
    {
        //Name should be the name token for the transformation. Eg. "DS_GAMING_SUPER_FORM". The actual name of the form should be handled using LanguageAPI. See Tokens.cs for an example of how that works

        [Tooltip("The buff given to you when you're transformed. This should be a buff unique to the form you're making.\nUse this buff for applying whatever stat increases you want.\nThis will be applied as a timed buff and will end the form when it goes away.")]
        public BuffDef buff;

        [Tooltip("The duration of the transformation in seconds. The actual duration of the form may be slightly longer to account for the transformation animation. If duration is <=0, a normal buff will be used instead of a timed buff")]
        public float duration;

        [Tooltip("Whether or not the form requires having certain items in order to transform. The specific items needed to transform are defined in neededItems once the ItemsCatalog is done.")]
        public bool requiresItems;

        [Tooltip("The item or items that are needed to transform. NeededItem struct stores a RoR2.ItemDef and a uint for how many of that item is needed. You can also just use ItemDefs here if you won't need multiple of the same item, there is an implicit cast")]
        public NeededItem[] neededItems;

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

        [Tooltip("The entity state used by the \"Body\" entity state machine for the transformation animation that will transition you into the form. Should be a subclass of TransformationBase")]
        public SerializableEntityStateType transformState;

        [Tooltip("Stores the material and mesh changes that will be applied when transforming based on what skin you're using.\nKey is the string token of the skin. RenderReplacements is a struct containing a material and mesh.\nPutting null for material or mesh will make them not change when transforming.")]
        public Dictionary<string, RenderReplacements> renderDictionary;

        [Tooltip("The name prefix/suffix that will be added when transformed, similar to the names of elites. {0} will be replaced with whatever the character's normal name is.\nIf the string is left empty, like \"\", there will be no name change.\nEg. \"Super {0}\" would be Super Sonic")]
        public string transformationNameToken;

        [Tooltip("The component that will track information about your form, such as whether all necessary items have been collected. This component will be put on a gameObject that will be created at the beginning of every stage and will stay for the duration of the stage.\nIf you're unsure what to put here, use typeof(FormHandler).\nYou can create a subclass of FormHandler and put it here if you want to add code, such as an extra requirement for transforming.")]
        public Type handlerComponent;

        [Tooltip("Contains information on what characters are allowed to transform.\nIf whitelist, any body name listed under bodyNames will be allowed. If not whitelist, any body name not listed under bodyNames will be allowed.\nBody name refers to the name that survivors and enemies use internally. If you're unsure about what body name means, look into RoR2 BodyCatalog related stuff")]
        public AllowedBodyList allowedBodyList;

        public FormIndex formIndex { get { return (FormIndex)Array.IndexOf(Forms.formsCatalog, this); } }

        public override string ToString()
        {
            return Language.GetString(this.name, Language.currentLanguageName);
        }
    }

    public struct NeededItem
    {
        public ItemDef item;

        public uint count;

        public static implicit operator ItemDef(NeededItem x) => x.item;

        public static implicit operator NeededItem(ItemDef x) => new NeededItem { item = x, count = 1 };

        public override string ToString()
        {
            return this.item.nameToken + " (" + count + ")\n";
        }
    }

    public struct AllowedBodyList
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