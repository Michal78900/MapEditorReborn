// -----------------------------------------------------------------------
// <copyright file="ButtonInteractedEventArgs.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Objects;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;

    public class ButtonInteractedEventArgs : EventArgs, IExiledEvent
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
}
