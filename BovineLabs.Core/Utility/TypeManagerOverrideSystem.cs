﻿// <copyright file="TypeManagerOverrideSystem.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITES

#if BL_TYPEMANAGER_OVERRIDE
#if UNITY_6000_0_OR_NEWER
namespace BovineLabs.Core.Utility
{
    using Unity.Entities;
    using Unity.Entities.Hybrid.Baking;

    // Baking initialization
    [CreateBefore(typeof(LinkedEntityGroupBakingCleanUp))]
    [UpdateInGroup(typeof(PreBakingSystemGroup), OrderFirst = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    public partial struct TypeManagerOverrideSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            TypeManagerOverrides.Initialize();
        }
    }
}
#endif
#endif

#endif // UNITY_ENTITES
