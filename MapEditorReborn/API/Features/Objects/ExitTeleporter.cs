namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="ExitTeleporter"/> used by the plugin to handle teleporters.
    /// </summary>
    public class ExitTeleporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportObject"/> class.
        /// </summary>
        public ExitTeleporter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportObject"/> class.
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
        /// Initializes a new instance of the <see cref="TeleportObject"/> class.
        /// </summary>
        /// <param name="position">The object's position.</param>
        /// <param name="scale">The object' scale.</param>
        /// <param name="roomType">The object's <see cref="Exiled.API.Enums.RoomType"/>.</param>
        /// <param name="chance">The value which determines the teleport probability on overlapping.</param>
        public ExitTeleporter(Vector3 position, Vector3 scale, RoomType roomType, float chance)
            : this(position, scale, roomType) => Chance = chance;

        /// <summary>
        /// Gets or sets the object's position.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the object' scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the object's <see cref="Exiled.API.Enums.RoomType"/>.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;

        /// <summary>
        /// Gets or sets a value which determines the teleport probability on overlapping.
        /// </summary>
        public float Chance { get; set; } = 100f;
    }
}
