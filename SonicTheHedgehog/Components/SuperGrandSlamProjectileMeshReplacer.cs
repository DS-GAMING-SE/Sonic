using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SonicTheHedgehog.Components
{
    public class SuperGrandSlamProjectileMeshReplacer : MonoBehaviour
    {
        public ProjectileController projectileController;

        public Mesh mesh;

        private void Awake()
        {
            projectileController = GetComponent<ProjectileController>();
        }
        public void Start()
        {
            if (projectileController.ghost && projectileController.owner && projectileController.owner.TryGetComponent<UniqueSkinEffect>(out var skinEffect) && skinEffect.skinController)
            {
                ReplaceMesh(skinEffect.skinController.currentSkinIndex);
            }
        }
        public void ReplaceMesh(int index)
        {
            if (!UniqueSkinEffect.skinEffects[index].HasValue) return;
            UniqueSkinEffect.SuperGrandSlamMeshReplacements meshReplacement = UniqueSkinEffect.skinEffects[index].Value.superGrandSlamMeshReplacement;
            if (meshReplacement.meshAddress != null && meshReplacement.meshAddress.RuntimeKeyIsValid())
            {
                mesh = AssetAsyncReferenceManager<Mesh>.LoadAsset(meshReplacement.meshAddress).WaitForCompletion();
            }
            else if (meshReplacement.mesh)
            {
                mesh = meshReplacement.mesh;
            }
            else
            {
                return;
            }
            Transform effects = projectileController.ghost.transform.GetChild(0);
            if (effects)
            {
                Transform sonics = effects.Find("Sonics");
                if (sonics)
                {
                    sonics.GetComponent<ParticleSystemRenderer>().mesh = mesh;
                }
            }
        }
    }
}
