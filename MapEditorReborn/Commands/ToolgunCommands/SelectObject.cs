// -----------------------------------------------------------------------
// <copyright file="SelectObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.ToolgunCommands;

using System;
using Events.EventArgs;
using Events.Handlers.Internal;

/// <summary>
/// Command used for selecting the objects.
/// </summary>
public class SelectObject : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "select";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = { "sel", "choose" };

    /// <inheritdoc/>
    public string Description
    {
        get => "Selects the object which you are looking at.";
    }

    /// <inheritdoc/>
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // if (!sender.CheckPermission($"mpr.{Command}"))
        // {
        //     response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
        //     return false;
        // }

        var player = Player.Get<MERPlayer>(sender);
        if (!ToolGunHandler.TryGetMapObject(player, out var mapObject))
        {
            if (player.TryGetSessionVariable(API.API.SelectedObjectSessionVarName, out object _))
            {
                ToolGunHandler.SelectObject(player, null);
                response = "You've successfully unselected the object!";
                return true;
            }

            response = "You aren't looking at any Map Editor object!";
            return false;
        }

        SelectingObjectEventArgs ev = new(player, mapObject, true);
        Events.Handlers.MapEditorObject.OnSelectingObject(ev);

        if (!ev.IsAllowed)
        {
            response = ev.Response;
            return true;
        }

        if (ToolGunHandler.SelectObject(player, ev.Object))
        {
            response = "You've successfully selected the object!";
        }
        else
        {
            response = "You've successfully unselected the object!";
        }

        return true;
    }
}