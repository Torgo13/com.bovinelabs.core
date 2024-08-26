﻿// <copyright file="ContextGetter.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITES

namespace BovineLabs.Core.Editor.Inspectors
{
    using System;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Entities;
    using Unity.Entities.Editor;
    using Unity.Entities.UI;

    internal interface IContextGetter
    {
        public InspectionContext Context { get; }

        public Entity Entity { get; }

        public World World { get; }

        public EntityManager EntityManager { get; }

        public bool IsReadOnly { get; }
    }

    internal static class ContextGetter
    {
        public static IContextGetter Create<T>(object inspector)
        {
            if (inspector is not IInspector)
            {
                throw new ArgumentException($"Inspector is not {nameof(IInspector)}", nameof(inspector));
            }

            if (!UnsafeUtility.IsUnmanaged<T>())
            {
                throw new ArgumentException($"{typeof(T)} is not unmanaged", nameof(T));
            }

            if (typeof(IBufferElementData).IsAssignableFrom(typeof(T)))
            {
                var db = typeof(DynamicBuffer<>).MakeGenericType(typeof(T));
                var context = typeof(ContextGetter<>).MakeGenericType(db);
                return (IContextGetter)Activator.CreateInstance(context, inspector);
            }

            if (typeof(IAspect).IsAssignableFrom(typeof(T)))
            {
                var context = typeof(AspectGetter<>).MakeGenericType(typeof(T));
                return (IContextGetter)Activator.CreateInstance(context, inspector);
            }

            {
                var context = typeof(ContextGetter<>).MakeGenericType(typeof(T));
                return (IContextGetter)Activator.CreateInstance(context, inspector);
            }
        }
    }

    internal class ContextGetter<T> : IContextGetter
    {
        private readonly EntityInspectorContext context;

        public ContextGetter(object inspector)
        {
            if (inspector is not InspectorBase<T> propertyInspector)
            {
                throw new ArgumentException($"Inspector is not {nameof(InspectorBase<T>)}", nameof(inspector));
            }

            this.context = propertyInspector.GetContext<EntityInspectorContext>();
        }

        public InspectionContext Context => this.context;

        public Entity Entity => this.context.Entity;

        public World World => this.context.World;

        public EntityManager EntityManager => this.context.EntityManager;

        public bool IsReadOnly => this.context.EntityContainer.IsReadOnly;
    }

    internal class AspectGetter<T> : IContextGetter
        where T : unmanaged, IAspect, IAspectCreate<T>
    {
        private readonly AspectInspectionContext context;
        private readonly EntityAspectContainer<T> container;

        public AspectGetter(object inspector)
        {
            if (inspector is not InspectorBase<EntityAspectContainer<T>> propertyInspector)
            {
                throw new ArgumentException($"Inspector is not {nameof(InspectorBase<T>)}", nameof(inspector));
            }

            this.context = propertyInspector.GetContext<AspectInspectionContext>();
            this.container = this.context.Root.GetTarget<EntityAspectContainer<T>>();
        }

        public InspectionContext Context => this.context;

        public Entity Entity => this.container.Entity;

        public World World => this.container.World;

        public EntityManager EntityManager => this.container.World.EntityManager;

        public bool IsReadOnly => true;
    }
}

#endif // UNITY_ENTITES

