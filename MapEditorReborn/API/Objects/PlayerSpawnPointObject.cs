namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="Handler.PlayerSpawnPointObj"/> used by the plugin to spawn and save PlayerSpawnPoint to a file.
    /// </summary>
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

        /// <summary>
        /// Gets or sets the role which will spawn on the spawnpoint.
        /// </summary>
        public RoleType RoleType { get; set; } = RoleType.Tutorial;

        /// <summary>
        /// Gets or sets the ItemSpawnPoint's position.
        /// </summary>
        // public Vector3 Position { get; set; } = Vector3.zero;
        public SerializableVector3 Position { get; set; } = SerializableVector3.Zero;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
