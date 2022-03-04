namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Components.ObjectComponents;

    public class SchematicSpawnedEventArgs : EventArgs
    {
        public SchematicSpawnedEventArgs(SchematicObjectComponent schematic, string name)
        {
            Schematic = schematic;
            Name = name;
        }

        public SchematicObjectComponent Schematic { get; }

        public string Name { get; }
    }
}
