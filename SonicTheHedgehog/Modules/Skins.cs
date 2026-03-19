using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SonicTheHedgehog.Modules
{
    internal static class SkinAddressables
    {
        internal static AssetReferenceT<Material> metalMaterial = new AssetReferenceT<Material>("0aec3cb6fc371554892b3463bedd454c");
        internal static AssetReferenceT<Material> superMetalMaterial = new AssetReferenceT<Material>("21ba9e605dea3a04b995f75206c6a3b8");
    }
    internal static class Skins
    {
        // If you're making a skin or something for Sonic, use 0.14 scale no convert units on the import settings. Don't ask why, I don't have an answer
        internal static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] defaultRendererInfos, GameObject root, UnlockableDef unlockableDef = null)
        {
            SkinDefInfo skinDefInfo = new SkinDefInfo
            {
                BaseSkins = Array.Empty<SkinDef>(),
                GameObjectActivations = new SkinDef.GameObjectActivation[0],
                Icon = skinIcon,
                MeshReplacements = new SkinDef.MeshReplacement[0],
                MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0],
                Name = skinName,
                NameToken = skinName,
                ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0],
                RendererInfos = new CharacterModel.RendererInfo[defaultRendererInfos.Length],
                RootObject = root,
                UnlockableDef = unlockableDef
            };

            On.RoR2.SkinDef.Awake += DoNothing;

            SkinDef skinDef = ScriptableObject.CreateInstance<RoR2.SkinDef>();
            skinDef.baseSkins = skinDefInfo.BaseSkins;
            skinDef.icon = skinDefInfo.Icon;
            skinDef.unlockableDef = skinDefInfo.UnlockableDef;
            skinDef.rootObject = skinDefInfo.RootObject;
            defaultRendererInfos.CopyTo(skinDefInfo.RendererInfos, 0);
            skinDef.rendererInfos = skinDefInfo.RendererInfos;
            skinDef.gameObjectActivations = skinDefInfo.GameObjectActivations;
            skinDef.meshReplacements = skinDefInfo.MeshReplacements;
            skinDef.projectileGhostReplacements = skinDefInfo.ProjectileGhostReplacements;
            skinDef.minionSkinReplacements = skinDefInfo.MinionSkinReplacements;
            skinDef.nameToken = skinDefInfo.NameToken;
            skinDef.name = skinDefInfo.Name;

            On.RoR2.SkinDef.Awake -= DoNothing;

            return skinDef;
        }

        private static void DoNothing(On.RoR2.SkinDef.orig_Awake orig, RoR2.SkinDef self)
        {
        }

        internal struct SkinDefInfo
        {
            internal SkinDef[] BaseSkins;
            internal Sprite Icon;
            internal string NameToken;
            internal UnlockableDef UnlockableDef;
            internal GameObject RootObject;
            internal CharacterModel.RendererInfo[] RendererInfos;
            internal SkinDef.MeshReplacement[] MeshReplacements;
            internal SkinDef.GameObjectActivation[] GameObjectActivations;
            internal SkinDef.ProjectileGhostReplacement[] ProjectileGhostReplacements;
            internal SkinDef.MinionSkinReplacement[] MinionSkinReplacements;
            internal string Name;
        }

        // I don't know how to import JUST meshes I keep importing fbx and don't know how to separate the mesh
        internal static SkinDef.MeshReplacement[] GetMeshReplacementsFromObject(CharacterModel.RendererInfo[] defaultRendererInfos, params string[] meshes)
        {

            List<SkinDef.MeshReplacement> meshReplacements = new List<SkinDef.MeshReplacement>();

            for (int i = 0; i < defaultRendererInfos.Length; i++)
            {
                if (string.IsNullOrEmpty(meshes[i]))
                    continue;

                meshReplacements.Add(
                new SkinDef.MeshReplacement
                {
                    renderer = defaultRendererInfos[i].renderer,
                    mesh = Assets.mainAssetBundle.LoadAsset<GameObject>(meshes[i]).GetComponent<SkinnedMeshRenderer>().sharedMesh
            });
            }

            return meshReplacements.ToArray();
        }
        internal static SkinDefParams.MeshReplacement[] GetParamMeshReplacementsFromObject(CharacterModel.RendererInfo[] defaultRendererInfos, params string[] meshes)
        {

            List<SkinDefParams.MeshReplacement> meshReplacements = new List<SkinDefParams.MeshReplacement>();

            for (int i = 0; i < defaultRendererInfos.Length; i++)
            {
                if (string.IsNullOrEmpty(meshes[i]))
                    continue;

                meshReplacements.Add(
                new SkinDefParams.MeshReplacement
                {
                    renderer = defaultRendererInfos[i].renderer,
                    mesh = Assets.mainAssetBundle.LoadAsset<GameObject>(meshes[i]).GetComponent<SkinnedMeshRenderer>().sharedMesh
                });
            }

            return meshReplacements.ToArray();
        }
    }
}