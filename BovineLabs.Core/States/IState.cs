// <copyright file="IState.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITES

namespace BovineLabs.Core.States
{
    using BovineLabs.Core.Collections;
    using Unity.Entities;

    public interface IState<T> : IComponentData
        where T : unmanaged, IBitArray<T>
    {
        T Value { get; set; }
    }
}

#endif // UNITY_ENTITES
