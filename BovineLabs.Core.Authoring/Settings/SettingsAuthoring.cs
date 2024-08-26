// <copyright file="SettingsAuthoring.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_ENTITES

namespace BovineLabs.Core.Authoring.Settings
{
    using System;
    using System.Linq;
    using Unity.Entities;
    using UnityEngine;

    public class SettingsAuthoring : MonoBehaviour
    {
        [SerializeField]
        private SettingsBase[] settings = Array.Empty<SettingsBase>();

        private class Baker : Baker<SettingsAuthoring>
        {
            public override void Bake(SettingsAuthoring authoring)
            {
                foreach (var setting in authoring.settings.Distinct())
                {
                    if (setting == null)
                    {
                        Debug.LogWarning($"Setting is not set on {authoring.gameObject} in {authoring.gameObject.scene.name}");
                        continue;
                    }

                    this.DependsOn(setting);
                    setting.Bake(this);
                }
            }
        }
    }
}

#endif // UNITY_ENTITES
