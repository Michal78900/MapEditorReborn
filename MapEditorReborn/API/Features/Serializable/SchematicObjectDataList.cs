namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using UnityEngine;

    [Serializable]
    public class SchematicObjectDataList
    {
        public int RootObjectId { get; set; }

        public List<SchematicBlockData> Blocks { get; set; } = new List<SchematicBlockData>();

        public string Path;
    }

    public class SchematicBlockData
    {
        public string Name { get; set; }

        public int ObjectId { get; set; }

        public int ParentId { get; set; }

        public string AnimatorName { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public Vector3 Scale { get; set; }

        public BlockType BlockType { get; set; }

        public Dictionary<string, object> Properties { get; set; }
    }
}
