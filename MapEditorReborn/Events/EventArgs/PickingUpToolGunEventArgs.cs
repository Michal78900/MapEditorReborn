// -----------------------------------------------------------------------
// <copyright file="PickingUpToolGunEventArgs.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using PluginAPI.Core;

namespace MapEditorReborn.Events.EventArgs;

using System;

/// <summary>
/// Contains all information before a picking up the ToolGun.
/// </summary>
public class PickingUpToolGunEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PickingUpToolGunEventArgs"/> class.
    /// </summary>
    /// <param name="player"><inheritdoc cref="Player"/></param>
    /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
    public PickingUpToolGunEventArgs(Player player, bool isAllowed = true)
    {
        Player = player;
        IsAllowed = isAllowed;
    }

    /// <summary>
    /// Gets the <see cref="PluginAPI.Core.Player"/> who's picking up the ToolGun.
    /// </summary>
    public Player Player { get; }

    /// <summary>
    /// Gets or sets the response to be displayed if the event cannot be executed.
    /// </summary>
    public string Response { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the player can pick up the ToolGun.
    /// </summary>
    public bool IsAllowed { get; set; }
}