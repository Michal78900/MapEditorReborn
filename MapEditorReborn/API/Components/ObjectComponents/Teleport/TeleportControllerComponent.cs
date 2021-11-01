namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// Component added to spawned TeleportController. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class TeleportControllerComponent : MapEditorObject
    {
        /// <summary>
        /// Instantiates <see cref="TeleportControllerComponent"/>.
        /// </summary>
        /// <param name="teleportObject">The <see cref="TeleportObject"/> used for instantiating the object.</param>
        /// <returns>Instance of this compoment.</returns>
        public TeleportControllerComponent Init(TeleportObject teleportObject)
        {
            Base = teleportObject;

            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public TeleportObject Base;

        /// <summary>
        /// The EntranceTeleport object of the <see cref="TeleportControllerComponent"/>.
        /// </summary>
        public TeleportComponent EntranceTeleport;

        /// <summary>
        /// The ExitTeleport object of the <see cref="TeleportControllerComponent"/>.
        /// </summary>
        public TeleportComponent ExitTeleport;

        /// <summary>
        /// The time when the teleport was last used.
        /// </summary>
        public DateTime LastUsed;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            EntranceTeleport?.Destroy();
            ExitTeleport?.Destroy();

            if (Base.EntranceTeleporterPosition != Vector3.zero && Base.ExitTeleporterPosition != Vector3.zero)
            {
                EntranceTeleport = CreateEntrance(Base.EntranceTeleporterPosition, Base.EntranceTeleporterRoomType, Base.EntranceTeleporterScale != Vector3.one ? Base.EntranceTeleporterScale : Scale);
                ExitTeleport = CreateExit(Base.ExitTeleporterPosition, Base.ExitTeleporterRoomType, Base.ExitTeleporterScale != Vector3.one ? Base.ExitTeleporterScale : Scale);
            }
            else
            {
                EntranceTeleport = CreateEntrance(transform.position, RoomType.Surface, Base.EntranceTeleporterScale != Vector3.one ? Base.EntranceTeleporterScale : Scale);
                ExitTeleport = CreateExit(transform.position + (Vector3.forward * 2f), RoomType.Surface, Base.ExitTeleporterScale != Vector3.one ? Base.ExitTeleporterScale : Scale);
            }
        }

        /// <summary>
        /// Method called when a teleport teleports something.
        /// </summary>
        public void OnTeleported()
        {
            if (Base.IsVisible)
            {
                Timing.RunCoroutine(EntranceTeleport.SlowdownCoin());
                Timing.RunCoroutine(ExitTeleport.SlowdownCoin());
            }
        }

        private TeleportComponent CreateEntrance(Vector3 position, RoomType roomType, Vector3 scale)
        {
            GameObject teleportEntrance = GameObject.CreatePrimitive(PrimitiveType.Cube);
            teleportEntrance.name = "TeleportEntrance";
            teleportEntrance.transform.position = Handler.GetRelativePosition(position, Handler.GetRandomRoom(roomType));
            teleportEntrance.transform.localScale = scale;

            teleportEntrance.transform.parent = transform;

            return teleportEntrance.AddComponent<TeleportComponent>().Init(true);
        }

        private TeleportComponent CreateExit(Vector3 position, RoomType roomType, Vector3 scale)
        {
            GameObject teleportExit = GameObject.CreatePrimitive(PrimitiveType.Cube);
            teleportExit.name = "TeleportExit";
            teleportExit.transform.position = Handler.GetRelativePosition(position, Handler.GetRandomRoom(roomType));
            teleportExit.transform.localScale = scale;

            teleportExit.transform.parent = transform;

            return teleportExit.AddComponent<TeleportComponent>().Init(false);
        }
    }
}
