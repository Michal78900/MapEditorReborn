namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="Handler.RagdollSpawnPointObj"/> used by the plugin to spawn and save RagdollSpawnPoints to a file.
    /// </summary>
    [Serializable]
    public class RagdollSpawnPointObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RagdollSpawnPointObject"/> class.
        /// </summary>
        public RagdollSpawnPointObject()
        {
        }

        /// <summary>
        /// Gets or sets the name of the ragdoll that will be spawned. If this is empty, a random name will be choosen from <see cref="MapSchematic.RagdollRoleNames"/>.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the RoleType of the ragdoll that will be spawned.
        /// </summary>
        public RoleType RoleType { get; set; } = RoleType.ClassD;

        /// <summary>
        /// Gets or sets the death reason of the ragdoll that will be spawned.
        /// </summary>
        public string DamageType { get; set; } = "None";

        /// <summary>
        /// Gets or sets the RagdollSpawnPoint's position.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the RagdollSpawnPoint's rotation.
        /// </summary>
        public Vector3 Rotation { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
