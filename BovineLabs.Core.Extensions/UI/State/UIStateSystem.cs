﻿// <copyright file="UIStateSystem.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_UI
namespace BovineLabs.Core.UI
{
    using BovineLabs.Core.States;
    using Unity.Burst;
    using Unity.Entities;

    [UpdateInGroup(typeof(UISystemGroup), OrderFirst = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.Presentation | Worlds.Service)]
    public partial struct UIStateSystem : ISystem, ISystemStartStop
    {
        private StateFlagModelWithHistory impl;

        /// <inheritdoc />
        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            this.impl = new StateFlagModelWithHistory(
                ref state,
                ComponentType.ReadWrite<UIState>(),
                ComponentType.ReadWrite<UIStatePrevious>(),
                ComponentType.ReadWrite<UIStateBack>(),
                ComponentType.ReadWrite<UIStateForward>(),
                64);
        }

        /// <inheritdoc />
        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            this.impl.Dispose(ref state);
        }

        /// <inheritdoc />
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            this.impl.Run(ref state, ecb);
            ecb.Playback(state.EntityManager);
        }
    }
}
#endif
