// <copyright file="BakerExtensions.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITES

namespace BovineLabs.Core.Authoring
{
    using Unity.Entities;

    public static class BakerExtensions
    {
        public static void AddEnabledComponent<T>(this IBaker baker, Entity entity, bool initialState)
            where T : unmanaged, IComponentData, IEnableableComponent
        {
            baker.AddComponent<T>(entity);
            baker.SetComponentEnabled<T>(entity, initialState);
        }

        public static void AddEnabledComponent<T>(this IBaker baker, Entity entity, T defaultValue, bool initialState)
            where T : unmanaged, IComponentData, IEnableableComponent
        {
            baker.AddComponent(entity, defaultValue);
            baker.SetComponentEnabled<T>(entity, initialState);
        }

        public static DynamicBuffer<T> AddEnabledBuffer<T>(this IBaker baker, Entity entity, bool initialState)
            where T : unmanaged, IBufferElementData, IEnableableComponent
        {
            var buffer = baker.AddBuffer<T>(entity);
            baker.SetComponentEnabled<T>(entity, initialState);
            return buffer;
        }
    }
}

#endif // UNITY_ENTITES
