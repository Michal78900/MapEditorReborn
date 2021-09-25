namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="Handler.PlayerSpawnPointObj"/> used by the plugin to spawn and save PlayerSpawnPoints to a file.
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

        /// <summary>
        /// Gets or sets the role which will spawn on the spawnpoint.
        /// </summary>
        public RoleType RoleType { get; set; } = RoleType.Scp173;

        /// <summary>
        /// Gets or sets the PlayerSpawnPoint's position.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
