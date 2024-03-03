﻿// // <copyright file="DynamicHashMapListElement.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Core.Editor.Inspectors
{
    using System;
    using System.Collections.Generic;
    using BovineLabs.Core.Iterators;
    using JetBrains.Annotations;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public class DynamicHashMapListElement<TBuffer, TKey, TValue> : DynamicListElement<TBuffer, DynamicHashMapListElement<TBuffer, TKey, TValue>.KVP>
        where TBuffer : unmanaged, IDynamicHashMap<TKey, TValue>
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged
    {
        public DynamicHashMapListElement(object inspector, int refreshRate = 250)
            : base(inspector, refreshRate)
        {
        }

        private DynamicHashMap<TKey, TValue> GetMap =>
             this.Context.EntityManager.GetBuffer<TBuffer>(this.Context.Entity).AsHashMap<TBuffer, TKey, TValue>();

        protected override void PopulateList(List<KVP> list)
        {
            var map = this.GetMap;

            using var e = map.GetEnumerator();
            while (e.MoveNext())
            {
                list.Add(new KVP(e.Current));
            }
        }

        protected override void OnValueChanged(NativeArray<KVP> newValues)
        {
            var keys = newValues.Slice().SliceWithStride<TKey>();
            var values = newValues.Slice().SliceWithStride<TValue>(UnsafeUtility.SizeOf<TKey>());

            var map = this.GetMap;
            map.Clear();
            map.AddBatchUnsafe(keys, values);
        }

        public struct KVP
        {
            [UsedImplicitly]
            public TKey Key;

            [UsedImplicitly]
            public TValue Value;

            public KVP(Iterators.KVPair<TKey, TValue> kvp)
            {
                this.Key = kvp.Key;
                this.Value = kvp.Value;
            }
        }
    }
}
