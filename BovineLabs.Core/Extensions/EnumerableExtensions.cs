﻿// <copyright file="EnumerableExtensions.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using Unity.Mathematics;
    using UnityEngine;

    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> e, T value)
            where T : IEquatable<T>
        {
            int index = 0;

            foreach (var t in e)
            {
                if (t.Equals(value))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> e, Func<T, bool> predicate)
        {
            int index = 0;

            foreach (var t in e)
            {
                if (predicate.Invoke(t))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }
    }
}
