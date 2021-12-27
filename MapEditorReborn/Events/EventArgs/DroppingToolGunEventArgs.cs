namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before dropping the ToolGun.
    /// </summary>
    public class DroppingToolGunEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DroppingToolGunEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DroppingToolGunEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> who's dropping the ToolGun.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the response to be displayed if the event cannot be executed.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can drop ToolGun.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
