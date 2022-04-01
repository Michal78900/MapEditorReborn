namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using UnityEngine;

    /// <summary>
    /// A tool used to easily handle primitives.
    /// </summary>
    [Serializable]
    public class PrimitiveSerializable : SerializableObject
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
        public PrimitiveSerializable(PrimitiveType primitiveType, string color, float health)
        {
            PrimitiveType = primitiveType;
            Color = color;
            Health = health;
        }

        /// <summary>
        /// Gets or sets the <see cref="UnityEngine.PrimitiveType"/>.
        /// </summary>
        public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Cube;

        /// <summary>
        /// Gets or sets the <see cref="PrimitiveSerializable"/>'s color.
        /// </summary>
        public string Color { get; set; } = "red";

        public float Health { get; set; } = -1f;
    }
}
