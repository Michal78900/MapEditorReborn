// -----------------------------------------------------------------------
// <copyright file="ShowingObjectHintEventArgs.cs" company="MapEditorReborn">
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
    /// Contains all information before showing a <see cref="MapEditorObject"/>'s hint.
    /// </summary>
    public class ShowingObjectHintEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowingObjectHintEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="mapEditorObject"><inheritdoc cref="Object"/></param>
        /// <param name="hint"><inheritdoc cref="Hint"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ShowingObjectHintEventArgs(Player player, MapEditorObject mapEditorObject, string hint, bool isAllowed = true)
        {
            Player = player;
            Object = mapEditorObject;
            Hint = hint;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> the <see cref="MapEditorObject"/>'s hint is being shown to.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="MapEditorObject"/> which is being looked.
        /// </summary>
        public MapEditorObject Object { get; set; }

        /// <summary>
        /// Gets or sets the hint to show.
        /// </summary>
        public string Hint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MapEditorObject"/>'s hint can be displayed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
