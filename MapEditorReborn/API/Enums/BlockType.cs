// -----------------------------------------------------------------------
// <copyright file="BlockType.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Enums
{
    /// <summary>
    /// All available blocks for schematic object.
    /// </summary>
    public enum BlockType
    {
        /// <summary>
        /// Represents an empty transform.
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Represents a primitive.
        /// </summary>
        Primitive = 1,

        /// <summary>
        /// Represents a light.
        /// </summary>
        Light = 2,

        /// <summary>
        /// Represents a pickup.
        /// </summary>
        Pickup = 3,

        /// <summary>
        /// Represents a workstation.
        /// </summary>
        Workstation = 4,

        /// <summary>
        /// Represents a sub-schematic.
        /// </summary>
        Schematic = 5,

        /// <summary>
        /// Represents a teleporter.
        /// </summary>
        Teleport = 6,

        /// <summary>
        /// Represents a locker.
        /// </summary>
        Locker = 7,
    }
}
