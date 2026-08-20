using EnemiesReturns.Components;
using HedgehogUtils.Forms.SuperForm;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using SonicTheHedgehog.Components;
using SonicTheHedgehog.SkillStates.Cyloop;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: HG.Reflection.SearchableAttribute.OptIn]
namespace SonicTheHedgehog.SkillStates.Cyloop
{
    public static class CyloopManager
    {
        public static ComponentPoolManager cyloopColliderControllerPool;
        public static ComponentPoolManager cyloopColliderPool;
        public static GameObject cyloopColliderControllerPrefab;
        public static GameObject cyloopColliderPrefab;

        public static SuperCyloopVFX superCyloopVFX = SuperCyloopVFX.Wind;

        internal static void Initialize()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            Stage.onServerStageBegin += RerollSuperCyloopVFX;
            cyloopColliderControllerPrefab = PrefabAPI.CreateEmptyPrefab("SonicCyloopColliderController", false);
            cyloopColliderControllerPrefab.AddComponent<CyloopColliderController>();
            cyloopColliderPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SonicCyloopCollider");
            cyloopColliderPrefab.AddComponent<CyloopCollider>();
            cyloopColliderPrefab.layer = LayerIndex.projectilePierce.intVal;
        }

        private static void OnSceneUnloaded(Scene arg0)
        {
            if (cyloopColliderControllerPool != null)
            {
                cyloopColliderControllerPool.ResetPools();
            }
            if (cyloopColliderPool != null)
            {
                cyloopColliderPool.ResetPools();
            }
        }
        private static void RerollSuperCyloopVFX(Stage stage)
        {
            new NetworkSuperCyloopVFX((CyloopManager.SuperCyloopVFX)UnityEngine.Random.RandomRangeInt(0, 3)).Send(NetworkDestination.Clients);
        }
        public static CyloopColliderController GetPooledCyloopColliderController(Cyloop cyloopState, Mesh[] meshes)
        {
            if (cyloopColliderControllerPool == null)
            {
                cyloopColliderControllerPool = new ComponentPoolManager(1, 1, false, true);
            }
            CyloopColliderController cyloop = (CyloopColliderController)cyloopColliderControllerPool.GetPooledObject(cyloopColliderControllerPrefab);
            cyloop.colliders = new CyloopCollider[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                cyloop.colliders[i] = GetPooledCyloopCollider(meshes[i], cyloop);
            }
            cyloop.overlapAttack = new OverlapAttack();
            cyloopState.PrepareAttack(ref cyloop.overlapAttack);
            cyloop.doubleOverlapAttack = new OverlapAttack();
            cyloop.doubleOverlapAttack.isCrit = cyloop.overlapAttack.isCrit;
            cyloopState.PrepareDoubleAttack(ref cyloop.doubleOverlapAttack);
            return cyloop;
        }
        public static CyloopCollider GetPooledCyloopCollider(Mesh mesh, CyloopColliderController owner)
        {
            if (cyloopColliderPool == null)
            {
                cyloopColliderPool = new ComponentPoolManager(1, 1, false, true);
            }
            CyloopCollider cyloopCollider = (CyloopCollider)cyloopColliderPool.GetPooledObject(cyloopColliderPrefab);
            cyloopCollider.collider.sharedMesh = mesh;
            cyloopCollider.owner = owner;
            return cyloopCollider;
        }

        [ConCommand(commandName = "cyloopvfx", flags = ConVarFlags.ExecuteOnServer, helpText = "Change the VFX style for Super Sonic's Cyloop skill.")]
        private static void SetSuperCyloopVFXCommand(ConCommandArgs args)
        {
            CyloopManager.superCyloopVFX = args.TryGetArgEnum<SuperCyloopVFX>(0).GetValueOrDefault();
        }
        public static GameObject GetSuperCyloopHitVFX()
        {
            switch (superCyloopVFX)
            {
                case SuperCyloopVFX.Chains:
                    return Modules.Assets.superCyloopHitChainsEffect;
                case SuperCyloopVFX.Spears:
                    return Modules.Assets.superCyloopHitWindEffect;
                default:
                case SuperCyloopVFX.Wind:
                    return Modules.Assets.superCyloopHitWindEffect;
            }
        }
        public static GameObject GetSuperCyloopDoubleHitVFX()
        {
            switch (superCyloopVFX)
            {
                case SuperCyloopVFX.Chains:
                    return Modules.Assets.superCyloopDoubleHitChainsEffect;
                case SuperCyloopVFX.Spears:
                    return Modules.Assets.superCyloopDoubleHitWindEffect;
                default:
                case SuperCyloopVFX.Wind:
                    return Modules.Assets.superCyloopDoubleHitWindEffect;
            }
        }
        public enum SuperCyloopVFX
        {
            Wind,
            Chains,
            Spears
        }
    }

    public class CyloopColliderController : ComponentPoolObject
    {
        public CyloopCollider[] colliders;
        public OverlapAttack overlapAttack;
        public OverlapAttack doubleOverlapAttack;

        private GameObject hitEffectPrefab;

        private List<HealthComponent> hitHealthComponents = new List<HealthComponent>();
        private List<OverlapAttack.OverlapInfo> hits = new List<OverlapAttack.OverlapInfo>();
        private List<HealthComponent> doubleHitHealthComponents = new List<HealthComponent>();
        private List<OverlapAttack.OverlapInfo> doubleHits = new List<OverlapAttack.OverlapInfo>();

        private const float DELAY = 0.15f;
        private const float RETURN_DELAY = 0.4f;
        private float timer;
        private bool attacked;

        private void OnEnable()
        {
            attacked = false;
            timer = 0;
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer > DELAY && !attacked)
            {
                attacked = true;

                hitEffectPrefab = overlapAttack.hitEffectPrefab;
                overlapAttack.hitEffectPrefab = null;

                if (hits != null && hits.Count > 0)
                {
                    overlapAttack.ProcessHits(hits);
                }
                if (hitEffectPrefab)
                {
                    for (int i = 0; i < hits.Count; i++)
                    {
                        EffectManager.SpawnEffect(hitEffectPrefab, new EffectData
                        {
                            origin = hits[i].hitPosition,
                            scale = Mathf.Min(hits[i].hurtBox.healthComponent.body.radius, 3f),
                            rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up)
                        }, true);
                    }
                }
                hitHealthComponents.Clear();
                hits.Clear();

                hitEffectPrefab = doubleOverlapAttack.hitEffectPrefab;
                doubleOverlapAttack.hitEffectPrefab = null;

                if (doubleHits != null && doubleHits.Count > 0)
                {
                    doubleOverlapAttack.ProcessHits(doubleHits);
                }
                if (hitEffectPrefab)
                {
                    for (int i = 0; i < doubleHits.Count; i++)
                    {
                        EffectManager.SpawnEffect(hitEffectPrefab, new EffectData
                        {
                            origin = doubleHits[i].hitPosition,
                            scale = Mathf.Min(doubleHits[i].hurtBox.healthComponent.body.radius, 3f),
                            rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up)
                        }, true);
                    }
                }
                doubleHitHealthComponents.Clear();
                doubleHits.Clear();
            }
            if (timer > RETURN_DELAY)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].PreReturnToPool();
                    colliders[i].ReturnToPool();
                }
                ReturnToPool();
            }
        }
        public void TryAddHit(HurtBox hurtBox)
        {
            if (!attacked)
            {
                if (Modules.Buffs.HasCyloopDebuff(hurtBox.healthComponent.body) && !doubleHitHealthComponents.Contains(hurtBox.healthComponent))
                {
                    doubleHitHealthComponents.Add(hurtBox.healthComponent);
                    doubleHits.Add(new OverlapAttack.OverlapInfo { hurtBox = hurtBox, hitPosition = hurtBox.transform.position, pushDirection = Vector3.zero });
                }
                else if (!hitHealthComponents.Contains(hurtBox.healthComponent))
                {
                    hitHealthComponents.Add(hurtBox.healthComponent);
                    hits.Add(new OverlapAttack.OverlapInfo { hurtBox = hurtBox, hitPosition = hurtBox.transform.position, pushDirection = Vector3.zero });
                }
            }
        }
    }

    [RequireComponent(typeof(MeshCollider))]
    public class CyloopCollider : ComponentPoolObject
    {
        public CyloopColliderController owner;
        public MeshCollider collider;
        private void Awake()
        {
            collider = GetComponent<MeshCollider>();
        }
        
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent<HurtBox>(out var hurtBox) && hurtBox.healthComponent && owner.overlapAttack.HurtBoxPassesFilter(hurtBox))
            {
                owner.TryAddHit(hurtBox);
            }
        }
        public override void PreReturnToPool()
        {
            Mesh.Destroy(collider.sharedMesh);
        }
    }
}
