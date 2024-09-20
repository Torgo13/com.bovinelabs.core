﻿// <copyright file="SubSceneLoadAuthoring.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Core.Authoring.SubScenes
{
    using System;
    using BovineLabs.Core.SubScenes;
    using Unity.Entities;
    using Unity.Entities.Serialization;
    using UnityEditor;
    using UnityEngine;

    public class SubSceneLoadAuthoring : MonoBehaviour
    {
        [SerializeField]
        private Data[] subScenes = Array.Empty<Data>();

        private class Baker : Baker<SubSceneLoadAuthoring>
        {
            public override void Bake(SubSceneLoadAuthoring authoring)
            {
                var buffer = this.AddBuffer<SubSceneLoad>(this.GetEntity(TransformUsageFlags.None));

                foreach (var s in authoring.subScenes)
                {
                    if (s.SceneAsset == null)
                    {
                        continue;
                    }

                    if (!this.IncludeScene(s.TargetWorld))
                    {
                        continue;
                    }

                    // Depends on shouldn't be required as we don't care if the scene asset actually changes, only if our own references change
                    buffer.Add(new SubSceneLoad
                    {
                        Name = s.SceneAsset.name,
                        Scene = new EntitySceneReference(s.SceneAsset),
                        TargetWorld = SubSceneLoadUtil.ConvertFlags(s.TargetWorld),
                        LoadingMode = s.LoadMode,
                        IsRequired = s.IsRequired,
                    });
                }
            }

            private bool IncludeScene(SubSceneLoadFlags flags)
            {
#if UNITY_NETCODE
                if (this.IsClient())
                {
                    return flags != SubSceneLoadFlags.Server;
                }

                if (this.IsServer())
                {
                    return flags != SubSceneLoadFlags.Client && flags != SubSceneLoadFlags.ThinClient;
                }
#endif
                return true;
            }
        }

        [Serializable]
        private class Data
        {
            public SceneAsset? SceneAsset;

#if UNITY_NETCODE
            public SubSceneLoadFlags TargetWorld = SubSceneLoadFlags.ThinClient | SubSceneLoadFlags.Client | SubSceneLoadFlags.Server;
#else
            public SubSceneLoadFlags TargetWorld = SubSceneLoadFlags.Game;
#endif

            public SubSceneLoadMode LoadMode = SubSceneLoadMode.AutoLoad;
            public bool IsRequired = true;
        }
    }

}