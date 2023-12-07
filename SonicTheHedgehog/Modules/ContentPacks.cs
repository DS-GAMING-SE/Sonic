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
            args.ReportProgress(1f);
            yield break;
        }

        private void SceneDirectorOnStart(SceneDirector.orig_Start orig, RoR2.SceneDirector self)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "intro") return;

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
                test.transform.GetChild(test.transform.childCount - 1).gameObject.AddComponent<EntityLocator>().entity = test;

                purchaseInteraction.name = "Silly man";
                purchaseInteraction.displayNameToken = SonicTheHedgehogPlugin.DEVELOPER_PREFIX + "_SONIC_THE_HEDGEHOG_BODY_EMERALD_TEMPLE";
                purchaseInteraction.available = true;
                purchaseInteraction.cost = 5050;
                purchaseInteraction.costType = CostTypeIndex.Money;
                PurchaseEvent purchaseEvent = new PurchaseEvent();
                
                // Neither of these seem to work. Gotta find a solution.
                /*purchaseEvent.AddListener(x =>
                {
                    Debug.Log("Bought this shit dunno.");
                    
                });

                PurchaseInteraction.onItemSpentOnPurchase += (e, x) =>
                {
                    Debug.Log("test");
                };*/
                
                // possible code to add a emerald -> Run.instance.GetUserMaster(x).inventory.GiveItem(Items.cyanEmerald);
                
                purchaseInteraction.onPurchase = purchaseEvent;
                
                spawnCard.prefab = test;
                spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
                spawnCard.sendOverNetwork = true;

                for (int i = 0; i < 7; i++)
                {
                    DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule,
                        Run.instance.stageRng));
                }
            }
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