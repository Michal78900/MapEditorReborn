using System.Collections.Generic;

public class SchematicBlockData
{
    public string Name { get; set; }

    public int ObjectId { get; set; }
    public int ParentId { get; set; }

    public string AnimatorName { get; set; }

    public SerializableVector3 Position { get; set; }

    public SerializableVector3 Rotation { get; set; }

    public SerializableVector3 Scale { get; set; }

    public BlockType BlockType { get; set; }

    public Dictionary<string, object> Properties { get; set; }
}
