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

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            EntranceTeleport?.Destroy();
            ExitTeleport?.Destroy();

            if (Base.EntranceTeleporterPosition != Vector3.zero && Base.ExitTeleporterPosition != Vector3.zero)
            {
                CreateEntrance(Base.EntranceTeleporterPosition, Base.EntranceTeleporterRoomType);
                CreateExit(Base.ExitTeleporterPosition, Base.ExitTeleporterRoomType);
            }
            else
            {
                CreateEntrance(transform.position, RoomType.Surface);
                CreateExit(transform.position + transform.forward, RoomType.Surface);
            }
        }

        /// <summary>
        /// The time when the teleport was last used.
        /// </summary>
        public DateTime LastUsed;

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

        private void CreateEntrance(Vector3 position, RoomType roomType)
        {
            GameObject teleportEntrance = GameObject.CreatePrimitive(PrimitiveType.Cube);
            teleportEntrance.name = "TeleportEntrance";
            teleportEntrance.transform.position = Handler.GetRelativePosition(position, Handler.GetRandomRoom(roomType));

            teleportEntrance.transform.parent = transform;

            EntranceTeleport = teleportEntrance.AddComponent<TeleportComponent>();
            EntranceTeleport.Init(true);
        }

        private void CreateExit(Vector3 position, RoomType roomType)
        {
            GameObject teleportExit = GameObject.CreatePrimitive(PrimitiveType.Cube);
            teleportExit.name = "TeleportExit";
            teleportExit.transform.position = Handler.GetRelativePosition(position, Handler.GetRandomRoom(roomType));

            teleportExit.transform.parent = transform;

            ExitTeleport = teleportExit.AddComponent<TeleportComponent>();
            ExitTeleport.Init(false);
        }
    }
}
