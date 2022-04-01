namespace MapEditorReborn.API.Features.Serializable
{
    using Exiled.API.Enums;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A tool used to handle teleporters.
    /// </summary>
    public class ExitTeleporterSerializable : SerializableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitTeleporterSerializable"/> class.
        /// </summary>
        public ExitTeleporterSerializable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitTeleporterSerializable"/> class.
        /// </summary>
        /// <param name="position">The object's position.</param>
        /// <param name="scale">The object' scale.</param>
        /// <param name="roomType">The object's <see cref="Exiled.API.Enums.RoomType"/>.</param>
        public ExitTeleporterSerializable(Vector3 position, Vector3 scale, RoomType roomType)
        {
            Position = position;
            Scale = scale;
            RoomType = roomType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitTeleporterSerializable"/> class.
        /// </summary>
        /// <param name="position">The object's position.</param>
        /// <param name="scale">The object' scale.</param>
        /// <param name="roomType">The object's <see cref="Exiled.API.Enums.RoomType"/>.</param>
        /// <param name="chance">The value which determines the teleport probability on overlapping.</param>
        public ExitTeleporterSerializable(Vector3 position, Vector3 scale, RoomType roomType, float chance)
            : this(position, scale, roomType) => Chance = chance;

        /// <summary>
        /// Gets or sets a value which determines the teleport probability on overlapping.
        /// </summary>
        public float Chance { get; set; } = 100f;

        [YamlIgnore]
        public override Vector3 Rotation { get; set; }
    }
}
