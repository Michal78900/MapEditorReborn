// -----------------------------------------------------------------------
// <copyright file="ChangingObjectScaleEventArgs.cs" company="MapEditorReborn">
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
    /// Contains all information before a <see cref="MapEditorObject.Scale"/> is changed.
    /// </summary>
    public class ChangingObjectScaleEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingObjectScaleEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="mapEditorObject"><inheritdoc cref="Object"/></param>
        /// <param name="scale"><inheritdoc cref="Scale"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingObjectScaleEventArgs(Player player, MapEditorObject mapEditorObject, Vector3 scale, bool isAllowed = true)
        {
            Player = player;
            Object = mapEditorObject;
            Scale = scale;
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
        /// Gets or sets the requested scale.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets the response to be displayed if the event cannot be executed.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MapEditorObject.Scale"/> can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
