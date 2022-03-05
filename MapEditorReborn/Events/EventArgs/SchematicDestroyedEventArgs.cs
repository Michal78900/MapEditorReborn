namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Components.ObjectComponents;

    public class SchematicDestroyedEventArgs : EventArgs
    {
        public SchematicDestroyedEventArgs(SchematicObjectComponent schematic, string name)
        {
            Schematic = schematic;
            Name = name;
        }

        public SchematicObjectComponent Schematic { get; }

        public string Name { get; }
    }
}
