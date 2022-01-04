namespace MapEditorReborn.API.Features.Objects.Schematics
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Used to animate schematics.
    /// </summary>
    [Serializable]
    public class AnimationFrame
    {
        /// <summary>
        /// Gets the frame delay.
        /// </summary>
        public float Delay { get;  private set; }

        /// <summary>
        /// Gets the position added during this frame.
        /// </summary>
        public Vector3 PositionAdded { get; private set; }

        /// <summary>
        /// Gets the rate of adding position during this frame.
        /// </summary>
        public float PositionRate { get; private set; }

        /// <summary>
        /// Gets the rotation added during this frame.
        /// </summary>
        public Vector3 RotationAdded { get; private set; }

        /// <summary>
        /// Gets the rate of adding rotation during this frame.
        /// </summary>
        public float RotationRate { get; private set; }

        /// <summary>
        /// Gets the refresh rate.
        /// </summary>
        public float FrameLength { get; private set; }
    }
}
