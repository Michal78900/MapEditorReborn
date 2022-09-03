// -----------------------------------------------------------------------
// <copyright file="WorkstationSerializable.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Serializable
{
    using System;

    /// <summary>
    /// A tool used to spawn and save Workstations to a file.
    /// </summary>
    [Serializable]
    public class WorkstationSerializable : SerializableObject
    {
        public WorkstationSerializable()
        {
        }

        public WorkstationSerializable(bool isInteractable)
        {
            IsInteractable = isInteractable;
        }

        public WorkstationSerializable(SchematicBlockData block)
        {
            IsInteractable = block.Properties.ContainsKey("IsInteractable");
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player can interact with the <see cref="WorkstationSerializable"/>.
        /// </summary>
        public bool IsInteractable { get; set; } = true;
    }
}
