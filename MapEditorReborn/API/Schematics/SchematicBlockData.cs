namespace MapEditorReborn.API
{
    using UnityEngine;

    public class SchematicBlockData
    {
        public ObjectType ObjectType { get; set; }

        public ItemType ItemType { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public Vector3 Scale { get; set; }
    }
}
