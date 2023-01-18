// -----------------------------------------------------------------------
// <copyright file="SchematicDestroyedEventArgs.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Objects;
    using Exiled.Events.EventArgs.Interfaces;

    public class SchematicDestroyedEventArgs : EventArgs, IExiledEvent
    {
        public SchematicDestroyedEventArgs(SchematicObject schematic, string name)
        {
            Schematic = schematic;
            Name = name;
        }

        public SchematicObject Schematic { get; }

        public string Name { get; }
    }
}
