// -----------------------------------------------------------------------
// <copyright file="Unload.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.UtilityCommands;

using System;
using API.Features.Serializable;
using Events.EventArgs;
using static API.API;

/// <summary>
/// Command used for unloading <see cref="MapSchematic"/>.
/// </summary>
public class Unload : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "unload";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = { "unl" };

    /// <inheritdoc/>
    public string Description
    {
        get => "Unloads currently loaded map.";
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
        UnloadingMapEventArgs ev = new(player, true);
        Events.Handlers.Map.OnUnloadingMap(ev);

        if (!ev.IsAllowed)
        {
            response = ev.Response;
            return true;
        }

        CurrentLoadedMap = null;
        response = "Map has been successfully unloaded!";
        return true;
    }
}