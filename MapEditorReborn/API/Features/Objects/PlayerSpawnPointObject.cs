// -----------------------------------------------------------------------
// <copyright file="PlayerSpawnPointObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using System.Collections.Generic;
    using Enums;
    using Exiled.API.Enums;
    using Exiled.Events.EventArgs.Player;
    using PlayerRoles;
    using Serializable;
    using static API;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Component added to a spawned PlayerSpawnPoint object. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class PlayerSpawnPointObject : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="PlayerSpawnPointObject"/>.
        /// </summary>
        /// <param name="playerSpawnPointSerializable">The <see cref="PlayerSpawnPointSerializable"/> used for instantiating the object.</param>
        /// <returns>Instance of this component.</returns>
        public PlayerSpawnPointObject Init(PlayerSpawnPointSerializable? playerSpawnPointSerializable)
        {
            _prevSpawnableTeam = playerSpawnPointSerializable.SpawnableTeam;
            Base = playerSpawnPointSerializable;

            ForcedRoomType = playerSpawnPointSerializable.RoomType != RoomType.Unknown ? playerSpawnPointSerializable.RoomType : FindRoom().Type;
            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public PlayerSpawnPointSerializable Base;

        /// <inheritdoc cref="MapEditorObject.IsRotatable"/>
        public override bool IsRotatable => false;

        /// <inheritdoc cref="MapEditorObject.IsScalable"/>
        public override bool IsScalable => false;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            Spawnpoints[_prevSpawnableTeam].Remove(this);
            Spawnpoints[Base.SpawnableTeam].Add(this);
            _prevSpawnableTeam = Base.SpawnableTeam;
        }

        private void OnDestroy() => Spawnpoints[_prevSpawnableTeam].Remove(this);

        private SpawnableTeam _prevSpawnableTeam;

        /// <summary>
        /// Gets all spawnpoints' positions.
        /// </summary>
        public static readonly Dictionary<SpawnableTeam, List<PlayerSpawnPointObject>> Spawnpoints = new();

        /// <summary>
        /// Registers the vanilla spawnpoints.
        /// </summary>
        internal static void ResetSpawnpoints()
        {
            Spawnpoints.Clear();
            foreach (SpawnableTeam spawnableTeam in EnumUtils<SpawnableTeam>.Values)
                Spawnpoints.Add(spawnableTeam, new List<PlayerSpawnPointObject>());
        }

        internal static void OnPlayerSpawning(SpawningEventArgs ev)
        {
            SpawnableTeam spawnableTeam = ev.Player.Role.Type switch
            {
                RoleTypeId.Scp049 => SpawnableTeam.Scp049,
                RoleTypeId.Scp0492 => SpawnableTeam.Scp0492,
                RoleTypeId.Scp079 => SpawnableTeam.Scp079,
                RoleTypeId.Scp096 => SpawnableTeam.Scp096,
                RoleTypeId.Scp106 => SpawnableTeam.Scp106,
                RoleTypeId.Scp173 => SpawnableTeam.Scp173,
                RoleTypeId.Scp939 => SpawnableTeam.Scp939,
                RoleTypeId.ClassD => SpawnableTeam.ClassD,
                RoleTypeId.Scientist => SpawnableTeam.Scientist,
                RoleTypeId.FacilityGuard => SpawnableTeam.FacilityGuard,
                RoleTypeId.NtfPrivate => SpawnableTeam.MTF,
                RoleTypeId.NtfSergeant => SpawnableTeam.MTF,
                RoleTypeId.NtfSpecialist => SpawnableTeam.MTF,
                RoleTypeId.NtfCaptain => SpawnableTeam.MTF,
                RoleTypeId.ChaosRifleman => SpawnableTeam.Chaos,
                RoleTypeId.ChaosConscript => SpawnableTeam.Chaos,
                RoleTypeId.ChaosMarauder => SpawnableTeam.Chaos,
                RoleTypeId.ChaosRepressor => SpawnableTeam.Chaos,
                RoleTypeId.Tutorial => SpawnableTeam.Tutorial,
                _ => SpawnableTeam.None,
            };

            if (spawnableTeam == SpawnableTeam.None)
                return;

            if (!Spawnpoints.ContainsKey(spawnableTeam) || Spawnpoints[spawnableTeam].Count == 0)
                return;

            if (CurrentLoadedMap is not null && !CurrentLoadedMap.RemoveDefaultSpawnPoints && Random.Range(0, Spawnpoints[spawnableTeam].Count + 1) == 0)
                return;

            PlayerSpawnPointObject spawnpoint = Spawnpoints[spawnableTeam][Random.Range(0, Spawnpoints[spawnableTeam].Count)];
            ev.Position = spawnpoint.Position;
            ev.HorizontalRotation = spawnpoint.EulerAngles.y;
        }
    }
}