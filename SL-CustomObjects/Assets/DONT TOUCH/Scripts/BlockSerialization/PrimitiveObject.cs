using UnityEngine;

public class PrimitiveObject
{
    public PrimitiveType Type { get; set; }
    public string Color { get; set; }
    public SerializableVector3 Position { get; set; }
    public SerializableVector3 Rotation { get; set; }
    public SerializableVector3 Scale { get; set; }
}