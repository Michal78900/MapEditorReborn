// -----------------------------------------------------------------------
// <copyright file="CopyingObjectEventArgs.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.EventArgs
{
    using API.Features.Objects;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a <see cref="MapEditorObject"/> is deleted.
    /// </summary>
    public class CopyingObjectEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyingObjectEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="mapEditorObject"><inheritdoc cref="Object"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public CopyingObjectEventArgs(Player player, MapEditorObject mapEditorObject, bool isAllowed = true)
        {
            Player = player;
            Object = mapEditorObject;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> who's copying the <see cref="MapEditorObject"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="MapEditorObject"/> which is being copied.
        /// </summary>
        public MapEditorObject Object { get; set; }

        /// <summary>
        /// Gets or sets the response to be displayed if the event cannot be executed.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MapEditorObject"/> can be copied.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
