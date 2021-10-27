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
        [Description("The name of the map.")]
        public string Name { get; private set; } = "None";

        /// <summary>
        /// Gets a value indicating whether the default spawnpoints should be removed.
        /// </summary>
        [Description("Whether the default player spawnpoints should be removed.")]
        public bool RemoveDefaultSpawnPoints { get; private set; } = false;

        /// <summary>
        /// Gets possible role names for a ragdolls.
        /// </summary>
        [Description("List of possible names for ragdolls spawned by RagdollSpawnPoints.")]
        public Dictionary<RoleType, List<string>> RagdollRoleNames { get; private set; } = new Dictionary<RoleType, List<string>>()
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
        /// Gets the list of <see cref="LightControllerObject"/>.
        /// </summary>
        public List<LightControllerObject> LightControllerObjects { get; private set; } = new List<LightControllerObject>();

        /// <summary>
        /// Gets the of <see cref="TeleportObjects"/>".
        /// </summary>
        public List<TeleportObject> TeleportObjects { get; private set; } = new List<TeleportObject>();

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
            LightControllerObjects.Clear();
            TeleportObjects.Clear();
            SchematicObjects.Clear();
        }
    }
}
