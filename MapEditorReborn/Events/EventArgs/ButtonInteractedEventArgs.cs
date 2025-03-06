// -----------------------------------------------------------------------
// <copyright file="ButtonInteractedEventArgs.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.EventArgs
{
    using API.Features.Objects;
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;

    public class ButtonInteractedEventArgs : IPlayerEvent, IPickupEvent
    {
        public ButtonInteractedEventArgs(Pickup button, Player player, SchematicObject schematic)
        {
            Button = button;
            Player = player;
            Schematic = schematic;
        }

        public Pickup Button { get; }

        public Pickup Pickup => Button;

        public Player Player { get; }

        public SchematicObject Schematic { get; }
    }
}
