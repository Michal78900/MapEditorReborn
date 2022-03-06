namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using Enums;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A tool used to spawn and save schematics.
    /// </summary>
    [Serializable]
    public class SchematicSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchematicSerializable"/> class.
        /// </summary>
        public SchematicSerializable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchematicSerializable"/> class.
        /// </summary>
        /// <param name="schematicName">The schematic's name.</param>
        public SchematicSerializable(string schematicName) => SchematicName = schematicName;

        /// <summary>
        /// Gets or sets the <see cref="SchematicSerializable"/>'s name.
        /// </summary>
        public string SchematicName { get; set; } = "None";

        public CullingType CullingType { get; set; } = CullingType.None;

        /// <summary>
        /// Gets or sets a value indicating whether the schematic can be picked up via the Gravity Gun.
        /// </summary>
        public bool IsPickable { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SchematicSerializable"/>'s position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SchematicSerializable"/>'s rotation.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SchematicSerializable"/>' scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the <see cref="SchematicSerializable"/>.
        /// </summary>
        public RoomType RoomType { get; set; }
    }
}
