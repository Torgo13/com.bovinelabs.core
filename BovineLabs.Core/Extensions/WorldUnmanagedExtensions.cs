﻿// <copyright file="WorldUnmanagedExtensions.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITES

namespace BovineLabs.Core.Extensions
{
    using Unity.Entities;

    public static class WorldUnmanagedExtensions
    {
        public static bool SystemExists<T>(this WorldUnmanaged world)
        {
            var typeIndex = TypeManager.GetSystemTypeIndex<T>();
            return world.GetExistingUnmanagedSystem(typeIndex) != SystemHandle.Null;
        }
    }
}

#endif // UNITY_ENTITES
