// <copyright file="EntitiesCacheUtility.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITIES

namespace BovineLabs.Core.Editor.Internal
{
    public static class EntitiesCacheUtility
    {
        public static void UpdateEntitySceneGlobalDependency()
        {
            Unity.Scenes.EntitiesCacheUtility.UpdateEntitySceneGlobalDependency();
        }
    }
}

#endif // UNITY_ENTITES
