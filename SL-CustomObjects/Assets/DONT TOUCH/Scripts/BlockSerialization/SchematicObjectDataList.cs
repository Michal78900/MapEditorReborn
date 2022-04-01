using System;
using System.Collections.Generic;

[Serializable]
public class SchematicObjectDataList
{
    public int RootObjectId { get; set; }

    public List<SchematicBlockData> Blocks { get; set; } = new List<SchematicBlockData>();
}
