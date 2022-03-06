namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Objects;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Contains all information before a <see cref="MapEditorObject.RelativePosition"/> is changed.
    /// </summary>
    public class ChangingObjectPositionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingObjectPositionEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="mapEditorObject"><inheritdoc cref="Object"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingObjectPositionEventArgs(Player player, MapEditorObject mapEditorObject, Vector3 position, bool isAllowed = true)
        {
            Player = player;
            Object = mapEditorObject;
            Position = position;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> who's spawned the <see cref="MapEditorObject"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="MapEditorObject"/> which is being modified.
        /// </summary>
        public MapEditorObject Object { get; }

        /// <summary>
        /// Gets or sets the requested position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the response to be displayed if the event cannot be executed.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MapEditorObject.RelativeRotation"/> can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
