namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using MEC;
    using UnityEngine;

    public class TeleportControllerComponent : MapEditorObject
    {
        public TeleportControllerComponent Init(TeleportObject teleportObject)
        {
            Base = teleportObject;

            UpdateObject();

            return this;
        }

        public TeleportObject Base;

        public TeleportComponent EntranceTeleport;

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

        public DateTime LastUsed;

        public void OnTeleported()
        {
            if (Base.IsVisible)
            {
                Timing.RunCoroutine(EntranceTeleport.FuniCoin());
                Timing.RunCoroutine(ExitTeleport.FuniCoin());
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
