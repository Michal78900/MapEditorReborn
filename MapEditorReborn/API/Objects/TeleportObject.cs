namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="Handler.TeleporterObj"/> used by the plugin to spawn and save LightControllers to a file.
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

        public Vector3 EntranceTeleporterScale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType EntranceTeleporterRoomType { get; set; } = RoomType.Unknown;

        /// <summary>
        /// Gets or sets the exit teleport's position.
        /// </summary>
        public Vector3 ExitTeleporterPosition { get; set; } = Vector3.zero;

        public Vector3 ExitTeleporterScale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType ExitTeleporterRoomType { get; set; } = RoomType.Unknown;

        /// <summary>
        /// Gets or sets the teleport's teleport cooldown.
        /// </summary>
        public float TeleportCooldown { get; set; } = 10f;

        /// <summary>
        /// Gets or sets a value indicating whether the teleport can teleport in both ways (exit will behave like the entrance).
        /// </summary>
        public bool BothWayMode { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the teleport is visible by the players.
        /// </summary>
        public bool IsVisible { get; set; } = true;
    }
}
