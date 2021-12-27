namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A tool to spawn and save RagdollSpawnPoints to a file.
    /// </summary>
    [Serializable]
    public class RagdollSpawnPointObject
    {
        /// <summary>
        /// Gets or sets the name of the ragdoll to spawned.
        /// <para>If this is empty, a random name will be choosen from <see cref="MapSchematic.RagdollRoleNames"/>.</para>
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the RoleType of the ragdoll to spawn.
        /// </summary>
        public RoleType RoleType { get; set; } = RoleType.ClassD;

        /// <summary>
        /// Gets or sets the death reason of the ragdoll to spawn.
        /// </summary>
        public string DeathReason { get; set; } = "None";

        /// <summary>
        /// Gets or sets the spawn chance of the ragdoll.
        /// </summary>
        public int SpawnChance { get; set; } = 100;

        /// <summary>
        /// Gets or sets the <see cref="RagdollSpawnPointObject"/>'s position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RagdollSpawnPointObject"/>'s rotation.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the RagdollSpawnPoint.
        /// </summary>
        public RoomType RoomType { get; set; }
    }
}
