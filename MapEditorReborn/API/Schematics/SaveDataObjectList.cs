namespace MapEditorReborn.API
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class SaveDataObjectList
    {
        public List<PrimitiveSchematicObject> Primitives = new List<PrimitiveSchematicObject>();
        public List<ItemSchematicObject> Items = new List<ItemSchematicObject>();
        public List<WorkStationSchematicObject> WorkStations = new List<WorkStationSchematicObject>();
    }

    public class ItemSchematicObject
    {
        public ItemType ItemType { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public Vector3 Scale { get; set; }
    }

    public class WorkStationSchematicObject
    {
        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public Vector3 Scale { get; set; }
    }

    public class PrimitiveSchematicObject
    {
        public PrimitiveType Type { get; set; }

        public string Color { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public Vector3 Scale { get; set; }
    }
}
