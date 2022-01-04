namespace MapEditorReborn.API.Features.Objects
{
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A tool used to handle teleporters.
    /// </summary>
    public class ExitTeleporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitTeleporter"/> class.
        /// </summary>
        public ExitTeleporter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitTeleporter"/> class.
        /// </summary>
        /// <param name="position">The object's position.</param>
        /// <param name="scale">The object' scale.</param>
        /// <param name="roomType">The object's <see cref="Exiled.API.Enums.RoomType"/>.</param>
        public ExitTeleporter(Vector3 position, Vector3 scale, RoomType roomType)
        {
            Position = position;
            Scale = scale;
            RoomType = roomType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitTeleporter"/> class.
        /// </summary>
        /// <param name="position">The object's position.</param>
        /// <param name="scale">The object' scale.</param>
        /// <param name="roomType">The object's <see cref="Exiled.API.Enums.RoomType"/>.</param>
        /// <param name="chance">The value which determines the teleport probability on overlapping.</param>
        public ExitTeleporter(Vector3 position, Vector3 scale, RoomType roomType, float chance)
            : this(position, scale, roomType) => Chance = chance;

        /// <summary>
        /// Gets or sets the <see cref="ExitTeleporter"/>'s position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ExitTeleporter"/>' scale.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the <see cref="ExitTeleporter"/>.
        /// </summary>
        public RoomType RoomType { get; set; }

        /// <summary>
        /// Gets or sets a value which determines the teleport probability on overlapping.
        /// </summary>
        public float Chance { get; set; } = 100f;
    }
}
