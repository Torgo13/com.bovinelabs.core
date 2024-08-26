﻿// <copyright file="BakingSystemInternal.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITES

namespace BovineLabs.Core.Internal
{
    using Unity.Entities;

    public static class BakingSystemInternal
    {
        public static Hash128 SceneGUID(this BakingSystem bakingSystem)
        {
            return bakingSystem.BakingSettings.SceneGUID;
        }
    }
}

#endif // UNITY_ENTITES
