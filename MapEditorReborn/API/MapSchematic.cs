namespace MapEditorReborn.API
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The MapSchematic class which is used to save and load maps.
    /// </summary>
    [Serializable]
    public class MapSchematic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapSchematic"/> class.
        /// </summary>
        public MapSchematic()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapSchematic"/> class.
        /// </summary>
        /// <param name="name">The name of the map.</param>
        public MapSchematic(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the <see cref="MapSchematic"/> name.
        /// </summary>
        public string Name = "None";

        /// <summary>
        /// Gets a value indicating whether the default spawnpoints should be disabled.
        /// </summary>
        [Description("Whether the default player spawnpoints should be removed. Keep in mind, that given role spawnpoint will be removed only if there is at least one custom spawn point of that type.")]
        public bool RemoveDefaultSpawnPoints { get; internal set; } = false;

        /// <summary>
        /// Gets possible role names for a ragdolls.
        /// </summary>
        [Description("List of possible names for ragdolls spawned by RagdollSpawnPoints.")]
        public Dictionary<RoleType, List<string>> RagdollRoleNames { get; internal set; } = new Dictionary<RoleType, List<string>>()
        {
            { RoleType.ClassD, new List<string>() { "D-9341" } },
        };

        /// <summary>
        /// Gets the list of <see cref="DoorObject"/>.
        /// </summary>
        public List<DoorObject> Doors { get; private set; } = new List<DoorObject>();

        /// <summary>
        /// Gets the list of <see cref="WorkStationObject"/>.
        /// </summary>
        public List<WorkStationObject> WorkStations { get; private set; } = new List<WorkStationObject>();

        /// <summary>
        /// Gets the list of <see cref="ItemSpawnPointObject"/>.
        /// </summary>
        public List<ItemSpawnPointObject> ItemSpawnPoints { get; private set; } = new List<ItemSpawnPointObject>();

        /// <summary>
        /// Gets the list of <see cref="PlayerSpawnPointObject"/>.
        /// </summary>
        public List<PlayerSpawnPointObject> PlayerSpawnPoints { get; private set; } = new List<PlayerSpawnPointObject>();

        /// <summary>
        /// Gets the list of <see cref="RagdollSpawnPointObject"/>.
        /// </summary>
        public List<RagdollSpawnPointObject> RagdollSpawnPoints { get; private set; } = new List<RagdollSpawnPointObject>();

        /// <summary>
        /// Gets the list of <see cref="ShootingTargetObject"/>.
        /// </summary>
        public List<ShootingTargetObject> ShootingTargetObjects { get; private set; } = new List<ShootingTargetObject>();

        /// <summary>
        /// Gets the list of <see cref="PrimitiveObject"/>.
        /// </summary>
        public List<PrimitiveObject> PrimitiveObjects { get; private set; } = new List<PrimitiveObject>();

        /// <summary>
        /// Gets the list of <see cref="LightSourceObject"/>.
        /// </summary>
        public List<LightSourceObject> LightSourceObjects { get; private set; } = new List<LightSourceObject>();

        /// <summary>
        /// Gets the list of <see cref="RoomLightObject"/>.
        /// </summary>
        public List<RoomLightObject> RoomLightObjects { get; private set; } = new List<RoomLightObject>();

        /// <summary>
        /// Gets the list of <see cref="TeleportObject"/>".
        /// </summary>
        public List<TeleportObject> TeleportObjects { get; private set; } = new List<TeleportObject>();

        /// <summary>
        /// Gets the list of <see cref="TeleportObject"/>/.
        /// </summary>
        public List<SchematicObject> SchematicObjects { get; private set; } = new List<SchematicObject>();

        /// <summary>
        /// Removes every currently saved object from all objects' lists.
        /// </summary>
        public void CleanupAll()
        {
            Doors.Clear();
            WorkStations.Clear();
            WorkStations.Clear();
            ItemSpawnPoints.Clear();
            PlayerSpawnPoints.Clear();
            RagdollSpawnPoints.Clear();
            ShootingTargetObjects.Clear();
            PrimitiveObjects.Clear();
            LightSourceObjects.Clear();
            RoomLightObjects.Clear();
            TeleportObjects.Clear();
            SchematicObjects.Clear();
        }
    }
}
