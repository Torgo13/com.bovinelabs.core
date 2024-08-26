// <copyright file="WorldUnmanagedInternal.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITES

namespace BovineLabs.Core.Internal
{
    using Unity.Entities;

    public static class WorldUnmanagedInternal
    {
        public static readonly ComponentType SystemInstanceComponentType = ComponentType.ReadWrite<SystemInstance>();
    }
}

#endif // UNITY_ENTITES
