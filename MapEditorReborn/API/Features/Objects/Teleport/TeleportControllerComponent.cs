﻿namespace MapEditorReborn.API.Features.Objects
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
    public class TeleportControllerComponent : MapEditorObject
    {
        /// <summary>
        /// Instantiates <see cref="TeleportControllerComponent"/>.
        /// </summary>
        /// <param name="teleportObject">The <see cref="TeleportSerializable"/> used for instantiating the object.</param>
        /// <returns>Instance of this compoment.</returns>
        public TeleportControllerComponent Init(TeleportSerializable teleportObject)
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
        /// The EntranceTeleport object of the <see cref="TeleportControllerComponent"/>.
        /// </summary>
        public TeleportComponent EntranceTeleport;

        /// <summary>
        /// The <see cref="List{T}"/> of all <see cref="TeleportComponent"/>.
        /// </summary>
        public List<TeleportComponent> ExitTeleports = new List<TeleportComponent>();

        /// <summary>
        /// The time when the teleport was last used.
        /// </summary>
        public DateTime LastUsed;

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
            foreach (TeleportComponent exitTeleport in ExitTeleports)
            {
                exitTeleport?.Destroy();
            }

            ExitTeleports.Clear();

            if (Base.EntranceTeleporterPosition != Vector3.zero)
            {
                EntranceTeleport = CreateTeleporter(Base.EntranceTeleporterPosition, Base.EntranceTeleporterScale != Vector3.one ? Base.EntranceTeleporterScale : Scale, Base.EntranceTeleporterRoomType);

                foreach (ExitTeleporterSerializable exitTeleporter in Base.ExitTeleporters)
                {
                    ExitTeleports.Add(CreateTeleporter(exitTeleporter.Position, exitTeleporter.Scale, exitTeleporter.RoomType, exitTeleporter.Chance));
                }
            }
            else
            {
                EntranceTeleport = CreateTeleporter(transform.position, Base.EntranceTeleporterScale != Vector3.one ? Base.EntranceTeleporterScale : Scale, RoomType.Surface, showIndicator: MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn);
                ExitTeleports.Add(CreateTeleporter(transform.position + (Vector3.forward * 2f), Vector3.one, RoomType.Surface, 100f, MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn));
            }
        }

        /// <summary>
        /// Creates a teleporter.
        /// </summary>
        /// <param name="position">The specified position.</param>
        /// <param name="scale">The specified scale.</param>
        /// <param name="roomType">The specified <see cref="RoomType"/>.</param>
        /// <param name="chance">The specified teleport chance.</param>
        /// <param name="showIndicator">A value indicating whether the indicator should be showed off.</param>
        /// <returns>The created <see cref="TeleportComponent"/> instance.</returns>
        internal TeleportComponent CreateTeleporter(Vector3 position, Vector3 scale, RoomType roomType, float chance = -1f, bool showIndicator = false)
        {
            GameObject teleport = GameObject.CreatePrimitive(PrimitiveType.Cube);
            teleport.name = chance == -1f ? "TeleportEntrance" : "TeleportExit";
            teleport.transform.position = GetRelativePosition(position, GetRandomRoom(roomType));
            teleport.transform.localScale = scale;

            teleport.transform.parent = transform;

            return teleport.AddComponent<TeleportComponent>().Init(chance, showIndicator);
        }
    }
}
