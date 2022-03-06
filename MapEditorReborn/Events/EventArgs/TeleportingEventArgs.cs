namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Objects;
    using Exiled.API.Features;
    using UnityEngine;

    public class TeleportingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportingEventArgs"/> class.
        /// </summary>
        public TeleportingEventArgs(TeleportObject teleport, bool isEntrance, GameObject teleportedObject, Player teleportedPlayer, Vector3 destination, bool isAllowed = true)
        {
            Teleport = teleport;
            IsEntrance = isEntrance;
            TeleportedObject = teleportedObject;
            TeleportedPlayer = teleportedPlayer;
            Destination = destination;
            IsAllowed = isAllowed;
        }

        public TeleportObject Teleport { get; }

        public bool IsEntrance { get; }

        public GameObject TeleportedObject { get; set; }

        public Player TeleportedPlayer { get; set; }

        public Vector3 Destination { get; set; }

        public bool IsAllowed { get; set; }
    }
}
