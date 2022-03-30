// -----------------------------------------------------------------------
// <copyright file="LockOnEvent.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Enums
{
    using System;

    [Flags]
    public enum LockOnEvent
    {
        None = 0,
        LightDecontaminated = 1,
        WarheadDetonated = 2,
    }
}
