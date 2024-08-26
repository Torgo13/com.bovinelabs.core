// <copyright file="EditorSettings.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Core.Editor.Settings
{
    using System;
    using System.Linq;
    using BovineLabs.Core.Authoring.Settings;
    using BovineLabs.Core.PropertyDrawers;
    using BovineLabs.Core.Settings;
    using UnityEditor;
    using UnityEngine;

    [SettingsGroup("Core")]
    public class EditorSettings : ScriptableObject, ISettings
    {
        public const string SettingsKey = "settings";
        public const string SettingsResourceKey = "settings.resource";
        public const string DefaultSettingsDirectory = "Assets/Settings/Settings";
        public const string DefaultSettingsResourceDirectory = "Assets/Settings/";

        [SerializeField]
        private KeyPath[] paths = Array.Empty<KeyPath>();

#if UNITY_ENTITES

        [SerializeField]
        private SettingsAuthoring? defaultSettingsAuthoring;

        [SerializeField]
        private KeyAuthoring[] settingAuthoring = { new() { World = "service" }, new() { World = "shared" } };

        public SettingsAuthoring? DefaultSettingsAuthoring => this.defaultSettingsAuthoring;

#endif // UNITY_ENTITES

        public void GetOrAddPath(string key, ref string path)
        {
            var result = this.paths.FirstOrDefault(k => k.Key.ToLower() == key);
            if (result == null)
            {
                var serializedObject = new SerializedObject(this);
                serializedObject.Update();

                var serializedProperty = serializedObject.FindProperty("paths");

                var index = serializedProperty.arraySize;
                serializedProperty.InsertArrayElementAtIndex(index);
                var keyPath = serializedProperty.GetArrayElementAtIndex(index);
                keyPath.FindPropertyRelative("Key").stringValue = key;
                keyPath.FindPropertyRelative("Path").stringValue = path;

                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssetIfDirty(this);
                return;
            }

            path = result.Path;
        }

#if UNITY_ENTITES

        public bool TryGetAuthoring(string world, out SettingsAuthoring? authoring)
        {
            world = world.ToLower();

            authoring = this.settingAuthoring.FirstOrDefault(k => k.World.ToLower() == world)?.Authoring;
            return authoring != null;
        }

#endif // UNITY_ENTITES

        [Serializable]
        public class KeyPath
        {
            [InspectorReadOnly]
            public string Key = string.Empty;

            public string Path = string.Empty;
        }

#if UNITY_ENTITES

        [Serializable]
        public class KeyAuthoring
        {
            public string World = string.Empty;

            public SettingsAuthoring? Authoring;
        }

#endif // UNITY_ENTITES
    }
}
