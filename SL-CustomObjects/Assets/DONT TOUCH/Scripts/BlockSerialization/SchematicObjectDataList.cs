using System;
using System.Collections.Generic;

[Serializable]
public class SchematicObjectDataList
{
    public SchematicObjectDataList()
    {
    }

    public SchematicObjectDataList(int rootObjectId)
    {
        RootObjectId = rootObjectId;
    }

    public int RootObjectId { get; set; }

    public List<SchematicBlockData> Blocks { get; set; } = new List<SchematicBlockData>();
}
