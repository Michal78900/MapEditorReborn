namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Extensions;
    using Features.Objects;
    using UnityEngine;

    using static API;

    /// <summary>
    /// Component added to a spawned PlayerSpawnPoint object. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class PlayerSpawnPointComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="PlayerSpawnPointComponent"/>.
        /// </summary>
        /// <param name="playerSpawnPointObject">The <see cref="PlayerSpawnPointObject"/> used for instantiating the object.</param>
        /// <returns>Instance of this compoment.</returns>
        public PlayerSpawnPointComponent Init(PlayerSpawnPointObject playerSpawnPointObject)
        {
            Base = playerSpawnPointObject;

            ForcedRoomType = playerSpawnPointObject.RoomType != RoomType.Unknown ? playerSpawnPointObject.RoomType : FindRoom().Type;
            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public PlayerSpawnPointObject Base;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            SpawnpointPositions[tag.ConvertToRoleType()].Remove(gameObject);
            tag = Base.RoleType.ConvertToSpawnPointTag();
            SpawnpointPositions[Base.RoleType].Add(gameObject);
        }

        private void OnDestroy()
        {
            SpawnpointPositions[Base.RoleType].Remove(gameObject);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the vanilla spawnpoints are disabled.
        /// </summary>
        public static bool VanillaSpawnPointsDisabled
        {
            get => (bool)CurrentLoadedMap?.RemoveDefaultSpawnPoints;

            set
            {
                if (value)
                {
                    foreach (List<GameObject> list in SpawnpointPositions.Values)
                    {
                        foreach (GameObject spawnPoint in list.ToList())
                        {
                            if (VanillaSpawnPoints.Contains(spawnPoint) && SpawnedObjects.FirstOrDefault(x => x is PlayerSpawnPointComponent spawnPointComponent && spawnPointComponent.Base.RoleType == spawnPoint.tag.ConvertToRoleType()) != null)
                                SpawnpointPositions[spawnPoint.tag.ConvertToRoleType()].Remove(spawnPoint);
                        }
                    }
                }
                else
                {
                    foreach (GameObject spawnPoint in VanillaSpawnPoints)
                    {
                        if (!SpawnpointPositions[spawnPoint.tag.ConvertToRoleType()].Contains(spawnPoint))
                            SpawnpointPositions[spawnPoint.tag.ConvertToRoleType()].Add(spawnPoint);
                    }
                }
            }
        }

        public static Dictionary<RoleType, List<GameObject>> SpawnpointPositions = new Dictionary<RoleType, List<GameObject>>();

        /// <summary>
        /// The list of vanilla spawn points.
        /// </summary>
        public static List<GameObject> VanillaSpawnPoints = new List<GameObject>();

        /// <summary>
        /// The list of tag names used by vanilla spawnpoints.
        /// </summary>
        public static readonly List<string> SpawnPointTags = new List<string>()
        {
            "SP_049",
            "SP_079",
            "SCP_096",
            "SP_106",
            "SP_173",
            "SCP_939",
            "SP_CDP",
            "SP_RSC",
            "SP_GUARD",
            "SP_MTF",
            "SP_CI",
        };

        /// <summary>
        /// Registers the vanilla spawnpoints.
        /// </summary>
        internal static void RegisterSpawnPoints()
        {
            SpawnpointPositions.Clear();
            foreach (string tag in SpawnPointTags)
            {
                SpawnpointPositions.Add(tag.ConvertToRoleType(), new List<GameObject>(GameObject.FindGameObjectsWithTag(tag)));
            }

            SpawnpointPositions.Add(RoleType.Tutorial, new List<GameObject>() { GameObject.Find("TUT Spawn") });

            VanillaSpawnPoints.Clear();
            foreach (List<GameObject> list in SpawnpointPositions.Values)
            {
                foreach (GameObject spawnPoint in list)
                {
                    VanillaSpawnPoints.Add(spawnPoint);
                }
            }
        }
    }
}
