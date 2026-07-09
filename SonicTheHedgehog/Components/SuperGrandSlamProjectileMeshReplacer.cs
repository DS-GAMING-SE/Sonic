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
            if (projectileController.ghost && projectileController.owner && UniqueSkinEffect.TryGetSkinEffects(projectileController.owner, out var skinEffect))
            {
                ReplaceMesh(skinEffect.Value);
            }
        }
        public void ReplaceMesh(UniqueSkinEffect.SkinEffects effect)
        {
            if (effect.superGrandSlamMeshReplacement == null) return;
            Mesh mesh = effect.superGrandSlamMeshReplacement.WaitForCompletion();
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
