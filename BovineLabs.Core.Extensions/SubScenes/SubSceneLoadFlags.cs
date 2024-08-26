﻿// <copyright file="SubSceneLoadFlags.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_SUBSCENE
namespace BovineLabs.Core.SubScenes
{
    using System;

    [Flags]
    public enum SubSceneLoadFlags : byte
    {
#if UNITY_NETCODE
        Server = 1 << 0,
        Client = 1 << 1,
        ThinClient = 1 << 2,
#else
        Game = 1 << 3,
#endif
        Service = 1 << 4,
    }
}
#endif
