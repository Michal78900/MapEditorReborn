using System.Collections.Generic;

public class LightSourceObject
{
    public string Color { get; set; } = "white";
    public float Intensity { get; set; } = 1f;
    public float Range { get; set; } = 1f;
    public bool Shadows { get; set; } = true;
    public SerializableVector3 Position { get; set; }
    // public List<SerializableAnimationFrame> AnimationFrames { get; set; }
}