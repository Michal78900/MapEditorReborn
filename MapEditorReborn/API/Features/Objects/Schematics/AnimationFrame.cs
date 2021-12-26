namespace MapEditorReborn.API.Features.Objects.Schematics
{
    using System;
    using UnityEngine;

    [Serializable]
    public class AnimationFrame
    {
        public float Delay { get; set; }

        public Vector3 PositionAdded { get; set; }

        public float PositionRate { get; set; }

        public Vector3 RotationAdded { get; set; }

        public float RotationRate { get; set; }

        public float FrameLength { get; set; }
    }
}
