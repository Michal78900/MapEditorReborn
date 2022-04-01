namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Objects;

    public class SchematicSpawnedEventArgs : EventArgs
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
