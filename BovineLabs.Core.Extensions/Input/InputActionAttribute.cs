﻿// <copyright file="InputActionAttribute.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_INPUT
namespace BovineLabs.Core.Input
{
    using System;

    public class InputActionAttribute : Attribute
    {
    }

    public class InputActionDeltaAttribute : Attribute
    {
    }

    public class InputActionDownAttribute : Attribute
    {
    }

    public class InputActionUpAttribute : Attribute
    {
    }
}
#endif
