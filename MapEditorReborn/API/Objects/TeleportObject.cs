namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    [Serializable]
    public class TeleportObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportObject"/> class.
        /// </summary>
        public TeleportObject()
        {
        }

        public Vector3 EntranceTeleporterPosition { get; set; } = Vector3.zero;

        public RoomType EntranceTeleporterRoomType { get; set; } = RoomType.Unknown;

        public Vector3 ExitTeleporterPosition { get; set; } = Vector3.zero;

        public RoomType ExitTeleporterRoomType { get; set; } = RoomType.Unknown;

        public float TeleportCooldown { get; set; } = 10f;

        public bool BothWayMode { get; set; } = false;

        public bool IsVisible { get; set; } = true;
    }
}
