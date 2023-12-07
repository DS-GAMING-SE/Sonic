using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneDirector = On.RoR2.SceneDirector;

namespace SonicTheHedgehog.Modules
{
    internal class ContentPacks : IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();
        public string identifier => SonicTheHedgehogPlugin.MODUID;

        public static List<GameObject> bodyPrefabs = new List<GameObject>();
        public static List<GameObject> masterPrefabs = new List<GameObject>();
        public static List<GameObject> projectilePrefabs = new List<GameObject>();

        public static List<SurvivorDef> survivorDefs = new List<SurvivorDef>();
        public static List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();

        public static List<SkillFamily> skillFamilies = new List<SkillFamily>();
        public static List<SkillDef> skillDefs = new List<SkillDef>();
        public static List<Type> entityStates = new List<Type>();

        public static List<BuffDef> buffDefs = new List<BuffDef>();
        public static List<EffectDef> effectDefs = new List<EffectDef>();
        public static List<ItemDef> itemDefs = new List<ItemDef>();

        public static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(
            ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public System.Collections.IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            this.contentPack.identifier = this.identifier;

            contentPack.bodyPrefabs.Add(bodyPrefabs.ToArray());
            contentPack.masterPrefabs.Add(masterPrefabs.ToArray());
            contentPack.projectilePrefabs.Add(projectilePrefabs.ToArray());

            contentPack.survivorDefs.Add(survivorDefs.ToArray());
            contentPack.unlockableDefs.Add(unlockableDefs.ToArray());

            contentPack.skillDefs.Add(skillDefs.ToArray());
            contentPack.skillFamilies.Add(skillFamilies.ToArray());
            contentPack.entityStateTypes.Add(entityStates.ToArray());

            contentPack.buffDefs.Add(buffDefs.ToArray());
            contentPack.effectDefs.Add(effectDefs.ToArray());

            contentPack.itemDefs.Add(itemDefs.ToArray());

            contentPack.networkSoundEventDefs.Add(networkSoundEventDefs.ToArray());

            On.RoR2.SceneDirector.Start += SceneDirectorOnStart;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteractionOnOnInteractionBegin;
            args.ReportProgress(1f);
            yield break;
        }

        // Thanks to Nuxlar for helping and given a lot of advice!
        private void PurchaseInteractionOnOnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig,
            PurchaseInteraction self, Interactor activator)
        {
            string prefix = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_EMERALD_TEMPLE_";
            if (self.displayNameToken.StartsWith(prefix))
            {
                int id = int.Parse(self.displayNameToken.Substring(prefix.Length));

                Inventory inventory = activator.GetComponent<CharacterBody>().inventory;

                switch (id)
                {
                    case 1:
                        inventory.GiveItem(Items.blueEmerald);
                        break;
                    case 2:
                        inventory.GiveItem(Items.redEmerald);
                        break;
                    case 3:
                        inventory.GiveItem(Items.grayEmerald);
                        break;
                    case 4:
                        inventory.GiveItem(Items.greenEmerald);
                        break;
                    case 5:
                        inventory.GiveItem(Items.cyanEmerald);
                        break;
                    case 6:
                        inventory.GiveItem(Items.purpleEmerald);
                        break;
                    default:
                        inventory.GiveItem(Items.yellowEmerald);
                        break;
                }

                Debug.Log("Bought this shit dunno, " + id);
                self.available = false;
            }
            
            orig(self, activator);
        }

        // Thanks to Nuxlar for helping and given a lot of advice!
        private void SceneDirectorOnStart(SceneDirector.orig_Start orig, RoR2.SceneDirector self)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "intro")
            {
                orig(self);
                return;
            }

            if (sceneName == "title")
            {
                // TODO:: create prefab of super sonic floating in the air silly style.
                Vector3 vector = new Vector3(38, 23, 36);
            }
            else
            {
                DirectorPlacementRule placementRule = new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Random
                };

                SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();

                GameObject test = Assets.mainAssetBundle.LoadAsset<GameObject>("BuyThingy");

                PurchaseInteraction purchaseInteraction = test.AddComponent<PurchaseInteraction>();
                test.GetComponent<Highlight>().targetRenderer = test.transform.GetChild(2).GetComponent<MeshRenderer>();
                test.transform.GetChild(test.transform.childCount - 1).gameObject.AddComponent<EntityLocator>().entity =
                    test;

                purchaseInteraction.name = "Silly man";
                purchaseInteraction.available = true;
                purchaseInteraction.cost = 1;
                purchaseInteraction.costType = CostTypeIndex.Money;
                purchaseInteraction.contextToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +
                                                   "_SONIC_THE_HEDGEHOG_BODY_EMERALD_TEMPLE_CONTEXT";

                spawnCard.prefab = test;
                spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
                spawnCard.sendOverNetwork = true;

                for (int i = 0; i < 7; i++)
                {
                    purchaseInteraction.displayNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX +
                                                           "_SONIC_THE_HEDGEHOG_BODY_EMERALD_TEMPLE_" + i;
                    DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule,
                        Run.instance.stageRng));
                }
            }
            
            orig(self);
        }

        public System.Collections.IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(this.contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }

    internal class Content
    {
        public static void AddCharacterBodyPrefab(GameObject bprefab)
        {
            ContentPacks.bodyPrefabs.Add(bprefab);
        }

        public static void AddMasterPrefab(GameObject prefab)
        {
            ContentPacks.masterPrefabs.Add(prefab);
        }

        public static void AddProjectilePrefab(GameObject prefab)
        {
            ContentPacks.projectilePrefabs.Add(prefab);
        }

        public static void AddSurvivorDef(SurvivorDef survivorDef)
        {
            ContentPacks.survivorDefs.Add(survivorDef);
        }

        public static void AddUnlockableDef(UnlockableDef unlockableDef)
        {
            ContentPacks.unlockableDefs.Add(unlockableDef);
        }

        public static void AddSkillDef(SkillDef skillDef)
        {
            ContentPacks.skillDefs.Add(skillDef);
        }

        public static void AddSkillFamily(SkillFamily skillFamily)
        {
            ContentPacks.skillFamilies.Add(skillFamily);
        }

        public static void AddEntityState(Type entityState)
        {
            ContentPacks.entityStates.Add(entityState);
        }

        public static void AddBuffDef(BuffDef buffDef)
        {
            ContentPacks.buffDefs.Add(buffDef);
        }

        public static void AddEffectDef(EffectDef effectDef)
        {
            ContentPacks.effectDefs.Add(effectDef);
        }

        public static void AddItemDef(ItemDef itemDef)
        {
            ContentPacks.itemDefs.Add(itemDef);
        }

        public static void AddNetworkSoundEventDef(NetworkSoundEventDef networkSoundEventDef)
        {
            ContentPacks.networkSoundEventDefs.Add(networkSoundEventDef);
        }
    }
}