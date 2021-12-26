namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="TeleportObject"/> used by the plugin to spawn and save LightControllers to a file.
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

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="ExitTeleporter"/>.
        /// </summary>
        public List<ExitTeleporter> ExitTeleporters { get; set; } = new List<ExitTeleporter>()
        {
            new ExitTeleporter(),
        };

        /// <summary>
        /// Gets or sets the teleport's teleport cooldown.
        /// </summary>
        public float TeleportCooldown { get; set; } = 10f;

        /// <summary>
        /// Gets or sets a value indicating whether the teleport can teleport in both ways (exit will behave like the entrance).
        /// </summary>
        public bool BothWayMode { get; set; } = false;
    }
}
