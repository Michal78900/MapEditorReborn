namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Enums;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A tool used to save and load maps.
    /// </summary>
    [Serializable]
    public sealed class MapSchematic
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
        public MapSchematic(string name) => Name = name;

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
        /// Gets the list of <see cref="DoorSerializable"/>.
        /// </summary>
        public List<DoorSerializable> Doors { get; private set; } = new List<DoorSerializable>();

        /// <summary>
        /// Gets the list of <see cref="WorkstationSerializable"/>.
        /// </summary>
        public List<WorkstationSerializable> WorkStations { get; private set; } = new List<WorkstationSerializable>();

        /// <summary>
        /// Gets the list of <see cref="ItemSpawnPointSerializable"/>.
        /// </summary>
        public List<ItemSpawnPointSerializable> ItemSpawnPoints { get; private set; } = new List<ItemSpawnPointSerializable>();

        /// <summary>
        /// Gets the list of <see cref="PlayerSpawnPointSerializable"/>.
        /// </summary>
        public List<PlayerSpawnPointSerializable> PlayerSpawnPoints { get; private set; } = new List<PlayerSpawnPointSerializable>();

        /// <summary>
        /// Gets the list of <see cref="RagdollSpawnPointSerializable"/>.
        /// </summary>
        public List<RagdollSpawnPointSerializable> RagdollSpawnPoints { get; private set; } = new List<RagdollSpawnPointSerializable>();

        /// <summary>
        /// Gets the list of <see cref="ShootingTargetSerializable"/>.
        /// </summary>
        public List<ShootingTargetSerializable> ShootingTargets { get; private set; } = new List<ShootingTargetSerializable>();

        /// <summary>
        /// Gets the list of <see cref="PrimitiveSerializable"/>.
        /// </summary>
        public List<PrimitiveSerializable> Primitives { get; private set; } = new List<PrimitiveSerializable>();

        /// <summary>
        /// Gets the list of <see cref="LightSourceSerializable"/>.
        /// </summary>
        public List<LightSourceSerializable> LightSources { get; private set; } = new List<LightSourceSerializable>();

        /// <summary>
        /// Gets the list of <see cref="RoomLightSerializable"/>.
        /// </summary>
        public List<RoomLightSerializable> RoomLights { get; private set; } = new List<RoomLightSerializable>();

        /// <summary>
        /// Gets the list of <see cref="TeleportSerializable"/>".
        /// </summary>
        public List<TeleportSerializable> Teleports { get; private set; } = new List<TeleportSerializable>();

        /// <summary>
        /// Gets the list of <see cref="LockerSerializable"/>.
        /// </summary>
        public List<LockerSerializable> Lockers { get; private set; } = new List<LockerSerializable>();

        /// <summary>
        /// Gets the list of <see cref="SchematicSerializable"/>/.
        /// </summary>
        public List<SchematicSerializable> Schematics { get; private set; } = new List<SchematicSerializable>();

        /// <summary>
        /// Removes every currently saved object from all objects' lists.
        /// </summary>
        public void CleanupAll()
        {
            Doors.Clear();
            WorkStations.Clear();
            ItemSpawnPoints.Clear();
            PlayerSpawnPoints.Clear();
            RagdollSpawnPoints.Clear();
            ShootingTargets.Clear();
            Primitives.Clear();
            LightSources.Clear();
            RoomLights.Clear();
            Teleports.Clear();
            Lockers.Clear();
            Schematics.Clear();
        }

        [YamlIgnore]
        public bool IsValid
        {
            get
            {
                if (_isValid != null)
                    return _isValid.Value;

                List<RoomType> roomTypes = NorthwoodLib.Pools.ListPool<RoomType>.Shared.Rent(Doors.Count + WorkStations.Count + ItemSpawnPoints.Count + PlayerSpawnPoints.Count + RagdollSpawnPoints.Count + ShootingTargets.Count + Primitives.Count + LightSources.Count + RoomLights.Count + Teleports.Count + Lockers.Count + Schematics.Count);

                roomTypes.AddRange(Doors.Select(x => x.RoomType));
                roomTypes.AddRange(WorkStations.Select(x => x.RoomType));
                roomTypes.AddRange(ItemSpawnPoints.Select(x => x.RoomType));
                roomTypes.AddRange(PlayerSpawnPoints.Select(x => x.RoomType));
                roomTypes.AddRange(RagdollSpawnPoints.Select(x => x.RoomType));
                roomTypes.AddRange(ShootingTargets.Select(x => x.RoomType));
                roomTypes.AddRange(Primitives.Select(x => x.RoomType));
                roomTypes.AddRange(LightSources.Select(x => x.RoomType));
                roomTypes.AddRange(RoomLights.Select(x => x.RoomType));
                roomTypes.AddRange(Teleports.Select(x => x.RoomType));
                roomTypes.AddRange(Schematics.Select(x => x.RoomType));

                foreach (RoomType roomType in roomTypes)
                {
                    if (!API.SpawnedRoomTypes.Contains(roomType))
                    {
                        NorthwoodLib.Pools.ListPool<RoomType>.Shared.Return(roomTypes);
                        _isValid = false;
                        return false;
                    }
                }

                NorthwoodLib.Pools.ListPool<RoomType>.Shared.Return(roomTypes);
                _isValid = true;
                return true;
            }
        }

        private bool? _isValid;
    }
}
