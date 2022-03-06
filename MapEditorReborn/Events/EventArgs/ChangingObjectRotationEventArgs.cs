namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Objects;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Contains all information before a <see cref="MapEditorObject.RelativeRotation"/> is changed.
    /// </summary>
    public class ChangingObjectRotationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingObjectRotationEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="mapEditorObject"><inheritdoc cref="Object"/></param>
        /// <param name="rotation"><inheritdoc cref="Rotation"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingObjectRotationEventArgs(Player player, MapEditorObject mapEditorObject, Vector3 rotation, bool isAllowed = true)
        {
            Player = player;
            Object = mapEditorObject;
            Rotation = rotation;
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
        /// Gets or sets the requested rotation.
        /// </summary>
        public Vector3 Rotation { get; set; }

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
