﻿// -----------------------------------------------------------------------
// <copyright file="SchematicSpawnedEventArgs.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Objects;
    using Exiled.Events.EventArgs.Interfaces;

    public class SchematicSpawnedEventArgs : IExiledEvent
    {
        public SchematicSpawnedEventArgs(SchematicObject schematic, string name)
        {
            Schematic = schematic;
            Name = name;
        }

        public SchematicObject Schematic { get; }

        public string Name { get; }
    }
}
