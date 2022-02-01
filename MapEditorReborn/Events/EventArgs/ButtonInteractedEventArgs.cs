namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Components.ObjectComponents;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    public class ButtonInteractedEventArgs : EventArgs
    {
        public ButtonInteractedEventArgs(Pickup button, Player player, SchematicObjectComponent schematic)
        {
            Button = button;
            Player = player;
            Schematic = schematic;
        }

        public Pickup Button { get; }

        public Player Player { get; }

        public SchematicObjectComponent Schematic { get; }
    }
}
