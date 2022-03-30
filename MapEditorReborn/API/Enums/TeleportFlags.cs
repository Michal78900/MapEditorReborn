// -----------------------------------------------------------------------
// <copyright file="TeleportFlags.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Enums
{
    using System;

    [Flags]
    public enum TeleportFlags
    {
        None = 0,
        Player = 1,
        Pickup = 2,
        ActiveGrenade = 4,
    }
}
