namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="Methods.TeleporterObj"/> used by the plugin to spawn and save LightControllers to a file.
    /// </summary>
    [Serializable]
    public class TeleportObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportObject"/> class.
        /// </summary>
        public TeleportObject()
        {
        }

        /// <summary>
        /// Gets or sets the entrance teleport's position.
        /// </summary>
        public Vector3 EntranceTeleporterPosition { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the entrance teleport's scale.
        /// </summary>
        public Vector3 EntranceTeleporterScale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType EntranceTeleporterRoomType { get; set; } = RoomType.Unknown;

        public List<ExitTeleporter> ExitTeleporters { get; set; } = new List<ExitTeleporter>()
        {
            new ExitTeleporter(),
        };

        /*
        /// <summary>
        /// Gets or sets the exit teleport's position.
        /// </summary>
        public Vector3 ExitTeleporterPosition { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the entrance teleport's scale.
        /// </summary>
        public Vector3 ExitTeleporterScale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType ExitTeleporterRoomType { get; set; } = RoomType.Unknown;
        */

        /// <summary>
        /// Gets or sets the teleport's teleport cooldown.
        /// </summary>
        public float TeleportCooldown { get; set; } = 10f;

        /// <summary>
        /// Gets or sets a value indicating whether the teleport can teleport in both ways (exit will behave like the entrance).
        /// </summary>
        public bool BothWayMode { get; set; } = false;
    }

    public class ExitTeleporter
    {
        public ExitTeleporter()
        {
        }

        public ExitTeleporter(Vector3 position, Vector3 scale, RoomType roomType)
        {
            Position = position;
            Scale = scale;
            RoomType = roomType;
        }

        public Vector3 Position { get; set; } = Vector3.zero;

        public Vector3 Scale { get; set; } = Vector3.one;

        public RoomType RoomType { get; set; } = RoomType.Unknown;

        public float Chance { get; set; } = 100f;
    }
}
