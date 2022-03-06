namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Objects;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Contains all information before a <see cref="MapEditorObject"/> is brought.
    /// </summary>
    public class BringingObjectEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BringingObjectEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="mapEditorObject"><inheritdoc cref="Object"/></param>
        /// <param name="newPosition"><inheritdoc cref="Position"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public BringingObjectEventArgs(Player player, MapEditorObject mapEditorObject, Vector3 newPosition, bool isAllowed = true)
        {
            Player = player;
            Object = mapEditorObject;
            Position = newPosition;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> who's grabbing the <see cref="MapEditorObject"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="MapEditorObject"/> which is being brought.
        /// </summary>
        public MapEditorObject Object { get; set; }

        /// <summary>
        /// Gets or sets the position to be set after bringing the <see cref="MapEditorObject"/>.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the response to be displayed if the event cannot be executed.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MapEditorObject"/> can be brought.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
