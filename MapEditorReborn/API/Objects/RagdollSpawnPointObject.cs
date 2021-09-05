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

        /// <inheritdoc cref="RagdollSpawnPointObject()"/>
        public RagdollSpawnPointObject(string name, RoleType roleType, string damageType, Vector3 position, Vector3 rotation, RoomType roomType)
        {
            Name = name;
            RoleType = roleType;
            DamageType = damageType;
            Position = position;
            Rotation = rotation;
            RoomType = roomType;
        }

        /// <summary>
        /// Gets the name of the ragdoll that will be spawned. If this is empty, a random name will be choosen from <see cref="MapSchematic.RagdollRoleNames"/>.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the RoleType of the ragdoll that will be spawned.
        /// </summary>
        public RoleType RoleType { get; private set; } = RoleType.ClassD;

        /// <summary>
        /// Gets the death reason of the ragdoll that will be spawned.
        /// </summary>
        public string DamageType { get; private set; } = "None";

        /// <summary>
        /// Gets the RagdollSpawnPoint's position.
        /// </summary>
        public Vector3 Position { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the RagdollSpawnPoint's rotation.
        /// </summary>
        public Vector3 Rotation { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; private set; } = RoomType.Unknown;
    }
}
