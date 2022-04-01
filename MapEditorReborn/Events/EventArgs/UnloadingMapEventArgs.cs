namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before the <see cref="API.Features.Objects.MapSchematic"/> is unloaded.
    /// </summary>
    public class UnloadingMapEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadingMapEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UnloadingMapEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> who's unloading the map.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the response to be displayed if the event cannot be executed.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the map will be unloaded.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
