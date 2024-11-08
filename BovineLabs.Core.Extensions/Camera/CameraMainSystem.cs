﻿// <copyright file="CameraMainSystem.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_CAMERA
namespace BovineLabs.Core.Camera
{
    using BovineLabs.Core;
    using BovineLabs.Core.Groups;
    using BovineLabs.Core.Input;
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    [WorldSystemFilter(WorldSystemFilterFlags.Presentation)]
    [UpdateInGroup(typeof(BeginSimulationSystemGroup))]
    public partial class CameraMainSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<CameraMain>();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            var cameraQuery = SystemAPI.QueryBuilder()
                .WithAllRW<LocalTransform>()
                .WithAll<CameraMain, Camera, Transform>()
                .Build();

            var noCameraQuery = SystemAPI.QueryBuilder()
                .WithAllRW<LocalTransform>()
                .WithAll<CameraMain>()
                .WithNone<Camera, Transform>()
                .Build();

            Entity entity;

            if (noCameraQuery.IsEmptyIgnoreFilter)
            {
                entity = cameraQuery.GetSingletonEntity();
            }
            else
            {
                entity = noCameraQuery.GetSingletonEntity();

                var cam = Camera.main;
                if (cam == null)
                {
                    SystemAPI.GetSingleton<BLDebug>().Error("No main camera found");
                    return;
                }

                var componentTypeSet = new ComponentTypeSet(ComponentType.ReadOnly<Transform>(), ComponentType.ReadOnly<Camera>());
                this.EntityManager.AddComponent(entity, componentTypeSet);
                this.EntityManager.AddComponentObject(entity, cam);
                this.EntityManager.AddComponentObject(entity, cam.transform);
            }

            var camera = cameraQuery.GetSingleton<Camera>();
            var tr = camera.transform;
            this.EntityManager.SetComponentData(entity, LocalTransform.FromPositionRotation(tr.position, tr.rotation));
        }
    }
}
#endif
