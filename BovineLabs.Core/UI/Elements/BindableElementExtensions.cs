﻿// <copyright file="BindableElementExtensions.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Core.UI
{
    using System.Runtime.CompilerServices;
    using UnityEngine.UIElements;

    public static class BindableElementExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Notify(this BindableElement bindableElement, [CallerMemberName] string property = "")
        {
            bindableElement.NotifyPropertyChanged(property);
        }
    }
}
