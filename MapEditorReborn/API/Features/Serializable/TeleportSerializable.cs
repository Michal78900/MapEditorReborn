namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A tool used to spawn and save Teleports to a file.
    /// </summary>
    [Serializable]
    public class TeleportSerializable
    {
        /// <summary>
        /// Gets or sets the entrance <see cref="TeleportSerializable"/>'s position.
        /// </summary>
        public Vector3 EntranceTeleporterPosition { get; set; }

        /// <summary>
        /// Gets or sets the entrance <see cref="TeleportSerializable"/>'s scale.
        /// </summary>
        public Vector3 EntranceTeleporterScale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType EntranceTeleporterRoomType { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="ExitTeleporterSerializable"/>.
        /// </summary>
        public List<ExitTeleporterSerializable> ExitTeleporters { get; set; } = new List<ExitTeleporterSerializable>()
        {
            new ExitTeleporterSerializable(),
        };

        /// <summary>
        /// Gets or sets the <see cref="TeleportSerializable"/>'s teleport cooldown.
        /// </summary>
        public float TeleportCooldown { get; set; } = 10f;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="TeleportSerializable"/> can teleport in both ways (exit will behave like the entrance).
        /// </summary>
        public bool BothWayMode { get; set; } = false;

        public TeleportFlags TeleportFlags { get; set; } = TeleportFlags.Player;

        public LockOnEvent LockOnEvent { get; set; } = LockOnEvent.None;
    }
}
