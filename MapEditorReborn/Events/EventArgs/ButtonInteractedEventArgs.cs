// -----------------------------------------------------------------------
// <copyright file="ButtonInteractedEventArgs.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using MapEditorReborn.Exiled.Features.Pickups;
using PluginAPI.Core;

namespace MapEditorReborn.Events.EventArgs;

using System;
using API.Features.Objects;

public class ButtonInteractedEventArgs : EventArgs
{
    public ButtonInteractedEventArgs(Pickup button, Player player, SchematicObject schematic)
    {
        Button = button;
        Player = player;
        Schematic = schematic;
    }

    public Pickup Button { get; }

    public Player Player { get; }

    public SchematicObject Schematic { get; }
}