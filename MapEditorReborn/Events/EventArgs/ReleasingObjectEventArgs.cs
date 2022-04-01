namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Objects;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before a <see cref="MapEditorObject"/> is released.
    /// </summary>
    public class ReleasingObjectEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReleasingObjectEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="mapEditorObject"><inheritdoc cref="Object"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ReleasingObjectEventArgs(Player player, MapEditorObject mapEditorObject, bool isAllowed = true)
        {
            Player = player;
            Object = mapEditorObject;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> who's releasing the <see cref="MapEditorObject"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="MapEditorObject"/> which is being released.
        /// </summary>
        public MapEditorObject Object { get; }

        /// <summary>
        /// Gets or sets the response to be displayed if the event cannot be executed.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MapEditorObject"/> can be released.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
