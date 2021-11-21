namespace MapEditorReborn.API
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using UnityEngine;

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
            if (playerSpawnPointObject != null)
            {
                Base = playerSpawnPointObject;

                ForcedRoomType = playerSpawnPointObject.RoomType != RoomType.Unknown ? playerSpawnPointObject.RoomType : FindRoom().Type;
                UpdateObject();
            }
            else
            {
                PrevTag = tag;
            }

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public PlayerSpawnPointObject Base;

        /// <summary>
        /// The previous (default) tag of this spawnpoint. This is only used by vanilla spawnpoints.
        /// </summary>
        public string PrevTag;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject() => tag = Base.RoleType.ConvertToSpawnPointTag();

        /// <summary>
        /// Registers the vanilla spawnpoints.
        /// </summary>
        internal static void RegisterVanillaSpawnPoints()
        {
            VanillaSpawnPoints.Clear();

            foreach (string tag in SpawnPointTags)
            {
                foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(tag))
                {
                    VanillaSpawnPoints.Add(gameObject.AddComponent<PlayerSpawnPointComponent>().Init(null));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the vanilla spawnpoints are disabled.
        /// </summary>
        public static bool VanillaSpawnPointsDisabled
        {
            get => (bool)Handler.CurrentLoadedMap?.RemoveDefaultSpawnPoints;

            set
            {
                if (value)
                {
                    foreach (PlayerSpawnPointComponent vanillaSpawnPoint in VanillaSpawnPoints)
                    {
                        if (Handler.SpawnedObjects.FirstOrDefault(x => x is PlayerSpawnPointComponent playerSpawnPoint && playerSpawnPoint.tag == vanillaSpawnPoint.tag) == null)
                            continue;

                        vanillaSpawnPoint.tag = "Untagged";
                    }
                }
                else
                {
                    foreach (PlayerSpawnPointComponent vanillaSpawnPoint in VanillaSpawnPoints)
                    {
                        vanillaSpawnPoint.tag = vanillaSpawnPoint.PrevTag;
                    }
                }
            }
        }

        /// <summary>
        /// The list of tag names used by vanilla spawnpoints.
        /// </summary>
        public static readonly List<string> SpawnPointTags = new List<string>()
        {
            "SP_049",
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
        /// The list of vanilla spawn points.
        /// </summary>
        public static List<PlayerSpawnPointComponent> VanillaSpawnPoints = new List<PlayerSpawnPointComponent>();
    }
}
