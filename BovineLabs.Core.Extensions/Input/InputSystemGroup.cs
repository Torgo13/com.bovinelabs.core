﻿// <copyright file="InputSystemGroup.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_INPUT
namespace BovineLabs.Core.Input
{
    using BovineLabs.Core.Groups;
    using Unity.Entities;

    [WorldSystemFilter(WorldSystemFilterFlags.Presentation, WorldSystemFilterFlags.Presentation)]
#if UNITY_NETCODE
    [UpdateInGroup(typeof(Unity.NetCode.GhostInputSystemGroup))]
#else
    [UpdateInGroup(typeof(BeginSimulationSystemGroup))]
#endif
    public partial class InputSystemGroup : ComponentSystemGroup
    {
    }
}
#endif
