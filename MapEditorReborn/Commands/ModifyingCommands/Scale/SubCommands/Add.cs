// -----------------------------------------------------------------------
// <copyright file="Add.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.ModifyingCommands.Scale.SubCommands;

using System;
using API.Extensions;
using API.Features.Objects;
using Events.EventArgs;
using Events.Handlers.Internal;
using static API.API;

/// <summary>
/// Modifies object's scale by adding a certain value to the current one.
/// </summary>
public class Add : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "add";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = Array.Empty<string>();

    /// <inheritdoc/>
    public string Description
    {
        get => string.Empty;
    }

    /// <inheritdoc/>
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // if (!sender.CheckPermission("mpr.scale"))
        // {
        //     response = $"You don't have permission to execute this command. Required permission: mpr.scale";
        //     return false;
        // }

        var player = Player.Get<MERPlayer>(sender);
        if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
        {
            if (!ToolGunHandler.TryGetMapObject(player, out mapObject))
            {
                response = "You haven't selected any object!";
                return false;
            }

            ToolGunHandler.SelectObject(player, mapObject);
        }

        if (!mapObject.IsScalable)
        {
            response = "You can't modify this object's scale!";
            return false;
        }

        if (arguments.Count >= 3 && TryGetVector(arguments.At(0), arguments.At(1), arguments.At(2), out var newScale))
        {
            ChangingObjectScaleEventArgs ev = new(player, mapObject, newScale, true);
            Events.Handlers.MapEditorObject.OnChangingObjectScale(ev);

            if (!ev.IsAllowed)
            {
                response = ev.Response;
                return true;
            }

            mapObject.transform.localScale += ev.Scale;
            player.ShowGameObjectHint(mapObject);

            mapObject.UpdateObject();
            mapObject.UpdateIndicator();

            response = ev.Scale.ToString("F3");
            return true;
        }

        response = "Invalid values.";
        return false;
    }
}