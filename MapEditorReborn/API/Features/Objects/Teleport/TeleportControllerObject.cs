namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Serializable;
    using UnityEngine;

    using static API;

    /// <summary>
    /// Component added to spawned TeleportController. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class TeleportControllerObject : MapEditorObject
    {
        /// <summary>
        /// Instantiates <see cref="TeleportControllerObject"/>.
        /// </summary>
        /// <param name="teleportObject">The <see cref="TeleportSerializable"/> used for instantiating the object.</param>
        /// <returns>Instance of this compoment.</returns>
        public TeleportControllerObject Init(TeleportSerializable teleportObject)
        {
            Base = teleportObject;
            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public TeleportSerializable Base;

        /// <summary>
        /// Gets the EntranceTeleport object of the <see cref="TeleportControllerObject"/>.
        /// </summary>
        public TeleportObject EntranceTeleport { get; private set; }

        /// <summary>
        /// Gets the <see cref="List{T}"/> of all <see cref="TeleportObject"/> exits.
        /// </summary>
        public List<TeleportObject> ExitTeleports { get; private set; }

        /// <summary>
        /// Gets the time when the teleport was last used.
        /// </summary>
        public DateTime LastUsed { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the object can be rotated.
        /// </summary>
        public override bool IsRotatable => false;

        /// <summary>
        /// Gets a value indicating whether can be scaled.
        /// </summary>
        public override bool IsScalable => false;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            EntranceTeleport?.Destroy();

            if (ExitTeleports != null)
            {
                foreach (TeleportObject exitTeleport in ExitTeleports)
                {
                    exitTeleport?.Destroy();
                }
            }

            if (Base.Position != Vector3.zero)
            {
                EntranceTeleport = CreateTeleporter(Base.Position, Base.Scale != Vector3.one ? Base.Scale : Scale, Base.RoomType);
                ExitTeleports = new (Base.ExitTeleporters.Count);

                foreach (ExitTeleporterSerializable exitTeleporter in Base.ExitTeleporters)
                {
                    ExitTeleports.Add(CreateTeleporter(exitTeleporter.Position, exitTeleporter.Scale, exitTeleporter.RoomType, exitTeleporter.Chance, !_initial));
                }
            }
            else
            {
                EntranceTeleport = CreateTeleporter(Position, Base.Scale != Vector3.one ? Base.Scale : Scale, RoomType.Surface, showIndicator: MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn);

                ExitTeleports = new (1);
                ExitTeleports.Add(CreateTeleporter(Position + (Vector3.forward * 2f), Vector3.one, RoomType.Surface, 100f, MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn));
            }

            _initial = false;
        }

        /// <summary>
        /// Creates a teleporter.
        /// </summary>
        /// <param name="position">The specified position.</param>
        /// <param name="scale">The specified scale.</param>
        /// <param name="roomType">The specified <see cref="RoomType"/>.</param>
        /// <param name="chance">The specified teleport chance.</param>
        /// <param name="showIndicator">A value indicating whether the indicator should be showed off.</param>
        /// <returns>The created <see cref="TeleportObject"/> instance.</returns>
        internal TeleportObject CreateTeleporter(Vector3 position, Vector3 scale, RoomType roomType, float chance = -1f, bool showIndicator = false)
        {
            GameObject teleport = GameObject.CreatePrimitive(PrimitiveType.Cube);
            teleport.name = chance == -1f ? "TeleportEntrance" : "TeleportExit";
            teleport.transform.position = GetRelativePosition(position, GetRandomRoom(roomType));
            teleport.transform.localScale = scale;

            teleport.transform.parent = transform;

            return teleport.AddComponent<TeleportObject>().Init(this, chance, showIndicator);
        }

        private bool _initial = true;
    }
}
