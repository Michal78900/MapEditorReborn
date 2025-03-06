// -----------------------------------------------------------------------
// <copyright file="TeleportingEventArgs.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.EventArgs
{
    using API.Features.Objects;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using UnityEngine;

    /// <summary>
    /// Contains all information before the object gets teleported.
    /// </summary>
    public class TeleportingEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportingEventArgs"/> class.
        /// </summary>
        public TeleportingEventArgs(TeleportObject entranceTeleport, TeleportObject exitTeleport, Player player, GameObject gameObject, Vector3 destination, Vector2 playerRotation, int teleportSoundId)
        {
            EntranceTeleport = entranceTeleport;
            ExitTeleport = exitTeleport;
            Player = player;
            GameObject = gameObject;
            Destination = destination;
            PlayerRotation = playerRotation;
            TeleportSoundId = teleportSoundId;
        }

        /// <summary>
        /// Gets the entrance teleport.
        /// </summary>
        public TeleportObject EntranceTeleport { get; }

        /// <summary>
        /// Gets the exit teleport.
        /// </summary>
        public TeleportObject ExitTeleport { get; }

        /// <summary>
        /// Gets or sets the player that is being teleported. May be null.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets or sets the game object that is being teleported. May be null.
        /// </summary>
        public GameObject GameObject { get; set; }

        /// <summary>
        /// Gets or sets the destination of the teleport.
        /// </summary>
        public Vector3 Destination { get; set; }

        /// <summary>
        /// Gets or sets the forced rotation of the player after teleport.
        /// </summary>
        public Vector2 PlayerRotation { get; set; }

        /// <summary>
        /// Gets or sets the teleport sound id played after teleport.
        /// </summary>
        public int TeleportSoundId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the teleport can teleport the object.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
