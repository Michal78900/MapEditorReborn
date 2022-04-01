namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using Exiled.API.Features;
    using global::MapEditorReborn.API.Enums;
    using global::MapEditorReborn.API.Features.Components;
    using UnityEngine;

    /// <summary>
    /// Contains all information before a <see cref="MapEditorObject"/> is spawned.
    /// </summary>
    public class SpawningObjectEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningObjectEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="objectType"><inheritdoc cref="ObjectType"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SpawningObjectEventArgs(Player player, Vector3 position, ObjectType objectType, bool isAllowed = true)
        {
            Player = player;
            Position = position;
            ObjectType = objectType;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> who's spawning the <see cref="MapEditorObject"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets spawn position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="API.Enums.ObjectType"/> which is being spawned.
        /// </summary>
        public ObjectType ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the response to be displayed if the event cannot be executed.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MapEditorObject"/> can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
