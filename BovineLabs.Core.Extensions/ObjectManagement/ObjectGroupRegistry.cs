﻿// <copyright file="ObjectGroupRegistry.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_OBJECT_DEFINITION
namespace BovineLabs.Core.ObjectManagement
{
    using BovineLabs.Core.Iterators;
    using Unity.Entities;

    [InternalBufferCapacity(0)]
    public struct ObjectGroupRegistry : IDynamicMultiHashMap<GroupId, ObjectId>
    {
        byte IDynamicMultiHashMap<GroupId, ObjectId>.Value { get; }
    }

    public static class ObjectGroupRegistryExtensions
    {
        internal static DynamicBuffer<ObjectGroupRegistry> Initialize(this DynamicBuffer<ObjectGroupRegistry> buffer)
        {
            return buffer.InitializeMultiHashMap<ObjectGroupRegistry, GroupId, ObjectId>();
        }

        public static DynamicMultiHashMap<GroupId, ObjectId> AsMap(this DynamicBuffer<ObjectGroupRegistry> buffer)
        {
            return buffer.AsMultiHashMap<ObjectGroupRegistry, GroupId, ObjectId>();
        }
    }
}
#endif
