using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class AnimationFrame
{
    public float Delay = 0f;
    public Vector3 PositionAdded = Vector3.zero;
    public float PositionRate = 1f;
    public Vector3 RotationAdded = Vector3.zero;
    public float RotationRate = 1f;
    public float FrameLength = 1f;
}

[Serializable]
public class SerializableAnimationFrame
{
    public SerializableAnimationFrame()
    {
    }

    public SerializableAnimationFrame(AnimationFrame animationFrame)
    {
        Delay = animationFrame.Delay;
        PositionAdded = animationFrame.PositionAdded;
        RotationAdded = animationFrame.RotationAdded;
        RotationRate = animationFrame.RotationRate;
        FrameLength = animationFrame.FrameLength;
        PositionRate = animationFrame.PositionRate;
    }

    public float Delay { get; set; }
    public SerializableVector3 PositionAdded { get; set; }
    public float PositionRate { get; set; }
    public SerializableVector3 RotationAdded { get; set; }
    public float RotationRate { get; set; }
    public float FrameLength { get; set; }
}
