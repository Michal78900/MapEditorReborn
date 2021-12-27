namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using Exiled.API.Enums;
    using Schematics;
    using UnityEngine;

    /// <summary>
    /// A tool used to easily handle primitives.
    /// </summary>
    [Serializable]
    public class PrimitiveObject
    {
        /// <summary>
        /// Gets or sets the <see cref="UnityEngine.PrimitiveType"/>.
        /// </summary>
        public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Cube;

        /// <summary>
        /// Gets or sets the <see cref="PrimitiveObject"/>'s color.
        /// </summary>
        public string Color { get; set; } = "red";

        /// <summary>
        /// Gets or sets the <see cref="PrimitiveObject"/>'s position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PrimitiveObject"/>'s rotation.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PrimitiveObject"/>' scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the <see cref="PrimitiveObject"/>.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;

        /// <summary>
        /// Gets or sets the a <see cref="List{T}"/> of <see cref="AnimationFrame"/> containing all <see cref="PrimitiveObject"/>'s animation frames.
        /// </summary>
        public List<AnimationFrame> AnimationFrames { get; set; } = new List<AnimationFrame>();

        /// <summary>
        /// Gets or sets the <see cref="Enums.AnimationEndAction"/>.
        /// </summary>
        public AnimationEndAction AnimationEndAction { get; set; }
    }
}
