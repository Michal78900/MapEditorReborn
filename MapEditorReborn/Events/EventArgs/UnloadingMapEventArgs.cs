// -----------------------------------------------------------------------
// <copyright file="UnloadingMapEventArgs.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Events.EventArgs;

using System;
using API.Features.Serializable;

/// <summary>
/// Contains all information before the <see cref="MapSchematic"/> is unloaded.
/// </summary>
public class UnloadingMapEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnloadingMapEventArgs"/> class.
    /// </summary>
    /// <param name="player"><inheritdoc cref="Player"/></param>
    /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
    public UnloadingMapEventArgs(MERPlayer player, bool isAllowed = true)
    {
        Player = player;
        IsAllowed = isAllowed;
    }

    /// <summary>
    /// Gets the <see cref="PluginAPI.Core.Player"/> who's unloading the map.
    /// </summary>
    public MERPlayer Player { get; }

    /// <summary>
    /// Gets or sets the response to be displayed if the event cannot be executed.
    /// </summary>
    public string Response { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the map will be unloaded.
    /// </summary>
    public bool IsAllowed { get; set; }
}