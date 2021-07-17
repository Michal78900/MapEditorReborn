namespace MapEditorReborn.API
{
    using Exiled.API.Enums;
    using System;
    using UnityEngine;

    [Serializable]
    public class PlayerSpawnPointObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSpawnPointObject"/> class.
        /// </summary>
        public PlayerSpawnPointObject()
        {
        }

        /// <inheritdoc cref="PlayerSpawnPointObject()"/>
        public PlayerSpawnPointObject(RoleType roleType, Vector3 position, RoomType roomType)
        {
            RoleType = roleType;
            Position = position;
            RoomType = roomType;
        }

        public RoleType RoleType { get; set; } = RoleType.Tutorial;

        /// <summary>
        /// Gets or sets the ItemSpawnPoint's position.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
