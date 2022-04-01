﻿// <copyright file="PoolAllocator.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Core.Memory
{
    using System;
    using System.Diagnostics;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

        public unsafe struct PoolAllocator<T> : IDisposable
        where T : unmanaged
    {
        private readonly int maxItems;
        private readonly Allocator allocator;

        private Ptr buffer;
        private UnsafeHashSet<Ptr> freeIndex;

        public PoolAllocator(int maxItems, Unity.Collections.Allocator allocator)
        {
            ValidateSize(maxItems);

            this.maxItems = maxItems;
            this.allocator = allocator;
            this.freeIndex = new UnsafeHashSet<Ptr>(maxItems, allocator);

            this.buffer = UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * maxItems, UnsafeUtility.AlignOf<T>(), allocator);

            for (int i = 0; i < maxItems; i++)
            {
                this.freeIndex.Add((T*)this.buffer.Value + i);
            }
        }

        public bool IsCreated => this.buffer.Value != null;

        public T* Alloc()
        {
            // ReSharper disable once UseMethodAny.2
            if (this.freeIndex.Count() == 0)
            {
                return null;
            }

            using var e = this.freeIndex.GetEnumerator();
            if (!e.MoveNext())
            {
                return null;
            }

            var ptr = e.Current;
            this.freeIndex.Remove(ptr);
            return (T*)ptr.Value;
        }

        public void Free(T* p)
        {
            this.ValidatePtr(p);
            this.freeIndex.Add(p);
        }

        public void Dispose()
        {
            UnsafeUtility.Free(this.buffer.Value, this.allocator);
            this.freeIndex.Dispose();
            this.buffer = Ptr.Zero;
            this.freeIndex = default;
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void ValidateSize(int maxItems)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (maxItems <= 0)
            {
                throw new ArgumentException("Null pointer");
            }
#endif
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void ValidatePtr(T* p)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (p == null)
            {
                throw new ArgumentException("Null pointer");
            }

            if (p < this.buffer || p >= ((T*)this.buffer + this.maxItems))
            {
                throw new ArgumentException("Ptr not from this allocator");
            }

            if (this.freeIndex.Contains(p))
            {
                throw new ArgumentException("Ptr already returned");
            }

            // This shouldn't be possible due to above checks
            if (this.freeIndex.Count() == this.maxItems)
            {
                throw new ArgumentException("More free than in buffer");
            }
#endif
        }
    }
}
