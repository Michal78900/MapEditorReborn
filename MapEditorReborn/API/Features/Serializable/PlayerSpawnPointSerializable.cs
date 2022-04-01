namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using Enums;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A tool used to spawn and save PlayerSpawnPoints to a file.
    /// </summary>
    [Serializable]
    public class PlayerSpawnPointSerializable : SerializableObject
    {
        /// <summary>
        /// Gets or sets the <see cref="PlayerSpawnPointSerializable"/>'s <see cref="Enums.SpawnableTeam"/>.
        /// </summary>
        public SpawnableTeam SpawnableTeam { get; set; }

        [YamlIgnore]
        public override Vector3 Rotation { get; set; }

        [YamlIgnore]
        public override Vector3 Scale { get; set; }

    }
}
