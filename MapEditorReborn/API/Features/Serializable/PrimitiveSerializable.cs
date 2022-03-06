namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A tool used to easily handle primitives.
    /// </summary>
    [Serializable]
    public class PrimitiveSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveSerializable"/> class.
        /// </summary>
        public PrimitiveSerializable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveSerializable"/> class.
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <param name="color"></param>
        public PrimitiveSerializable(PrimitiveType primitiveType, string color)
        {
            PrimitiveType = primitiveType;
            Color = color;
        }

        /// <summary>
        /// Gets or sets the <see cref="UnityEngine.PrimitiveType"/>.
        /// </summary>
        public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Cube;

        /// <summary>
        /// Gets or sets the <see cref="PrimitiveSerializable"/>'s color.
        /// </summary>
        public string Color { get; set; } = "red";

        /// <summary>
        /// Gets or sets the <see cref="PrimitiveSerializable"/>'s position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PrimitiveSerializable"/>'s rotation.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PrimitiveSerializable"/>' scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the <see cref="PrimitiveSerializable"/>.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
