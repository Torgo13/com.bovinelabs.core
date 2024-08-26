// <copyright file="EntityInternals.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITES

namespace BovineLabs.Core.Internal
{
    using System;
    using Unity.Entities;

    public static class EntityInternals
    {
        public static Type CompanionGameObjectUpdateSystemType => typeof(CompanionGameObjectUpdateSystem);
    }
}

#endif // UNITY_ENTITES
