using System.Collections.Generic;
using UnityEngine;

public class PrimitiveObject
{
    public PrimitiveType PrimitiveType { get; set; }
    public string Color { get; set; }
    public SerializableVector3 Position { get; set; }
    public SerializableVector3 Rotation { get; set; }
    public SerializableVector3 Scale { get; set; }
    public List<SerializableAnimationFrame> AnimationFrames { get; set; }
    public AnimationEndAction AnimationEndAction { get; set; }
}