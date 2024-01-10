﻿// <copyright file="UIDocumentManager.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_UI
namespace BovineLabs.Core.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;
    using UnityEngine.UIElements;

    [RequireComponent(typeof(UIDocument))]
    public class UIDocumentManager : UIAssetManagement, IUIDocumentManager
    {
        private const string RootClassName = "root";
        private const string PanelClassName = "panel";

        private readonly List<OrderedElement> elements = new();
        private readonly List<OrderedElement> rootElements = new();

        private VisualElement view = null!;

#if UNITY_EDITOR
        public event Action? EditorRebuild;
#endif

        public static IUIDocumentManager Instance { get; private set; } = new NullDocumentManager();

        public VisualElement Root { get; private set; } = null!;

        public void AddRoot(VisualElement visualElement, int priority = 0)
        {
            var e = new OrderedElement(visualElement, priority);

            this.rootElements.Add(e);
            this.rootElements.Sort();

            var index = this.rootElements.IndexOf(e);
            this.Root.Insert(index, visualElement);
        }

        public void RemoveRoot(VisualElement visualElement)
        {
            var index = this.rootElements.IndexOf(new OrderedElement(visualElement, 0));

            if (index < 0)
            {
                Debug.LogError($"Removing {visualElement} that isn't added.");
            }
            else
            {
                this.rootElements.RemoveAt(index);
                visualElement.RemoveFromHierarchy();
            }
        }

        public void AddPanel(int key, IBindingObject binding, int priority = 0)
        {
            // We use the same key as asset key as we don't allow multiple copies of same panel loaded at this time
            if (!this.TryLoadPanel(key, key, out var element))
            {
                return;
            }

            element.dataSource = binding;

            element.pickingMode = PickingMode.Ignore;
            element.AddToClassList(PanelClassName);

            var e = new OrderedElement(element, priority);

            this.elements.Add(e);
            this.elements.Sort();

            var index = this.elements.IndexOf(e);
            this.view.Insert(index, element);
        }

        public void RemovePanel(int key)
        {
            if (!this.TryUnloadPanel(key, out var ve))
            {
                return;
            }

            var index = this.elements.IndexOf(new OrderedElement(ve, 0));

            if (index < 0)
            {
                Debug.LogError($"Removing {ve} that isn't added.");
            }
            else
            {
                this.elements.RemoveAt(index);
                ve.RemoveFromHierarchy();
            }
        }

        private void Awake()
        {
            Instance = this;

            this.view = new VisualElement { pickingMode = PickingMode.Ignore };
            this.view.AddToClassList(RootClassName);

            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            this.AddRoot(this.view);

            this.LoadAllPanels();
        }

        private void OnEnable()
        {
            var document = this.GetComponent<UIDocument>();
            this.Root = this.GetRoot(document);
            this.Root.Add(this.view);
#if UNITY_EDITOR
            this.EditorRebuild?.Invoke();
#endif
        }

        private void OnDisable()
        {
            this.Root.Remove(this.view);
        }

        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local", Justification = "SystemAPI requirement")]
        private VisualElement GetRoot(UIDocument document)
        {
            const string rootName = "Root";

            if (document.rootVisualElement == null)
            {
                Debug.LogError($"{nameof(UIDocument)} root not found.");
                return new VisualElement();
            }

            var documentRoot = document.rootVisualElement.Q(rootName);
            if (documentRoot == null)
            {
                Debug.LogError($"{rootName} not found.");
                return new VisualElement();
            }

            return documentRoot;
        }

        private readonly struct OrderedElement : IComparable<OrderedElement>, IEquatable<OrderedElement>
        {
            private readonly VisualElement element;
            private readonly int priority;

            public OrderedElement(VisualElement visualElement, int priority)
            {
                this.priority = priority;
                this.element = visualElement;
            }

            public VisualElement Element => this.element;

            /// <inheritdoc />
            public int CompareTo(OrderedElement other)
            {
                return this.priority.CompareTo(other.priority);
            }

            /// <inheritdoc />
            public bool Equals(OrderedElement other)
            {
                return this.element.Equals(other.element);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return this.element.GetHashCode();
            }
        }

        private class NullDocumentManager : IUIDocumentManager
        {
            private VisualElement? root;

#if UNITY_EDITOR
#pragma warning disable CS0067
            public event Action? EditorRebuild;
#pragma warning restore CS0067
#endif

            public VisualElement Root => this.root ??= new VisualElement();

            public void AddRoot(VisualElement visualElement, int priority = 0)
            {
            }

            public void RemoveRoot(VisualElement visualElement)
            {
            }

            public void AddPanel(int key, IBindingObject bindingObject, int priority = 0)
            {
            }

            public void RemovePanel(int key)
            {
            }
        }
    }
}
#endif