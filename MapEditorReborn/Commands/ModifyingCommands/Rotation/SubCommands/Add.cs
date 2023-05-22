// -----------------------------------------------------------------------
// <copyright file="Add.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.ModifyingCommands.Rotation.SubCommands;

using System;
using API.Features.Objects;
using Events.EventArgs;
using Events.Handlers.Internal;
using static API.API;

/// <summary>
/// Modifies object's rotation by adding a certain value to the current one.
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
        // if (!sender.CheckPermission("mpr.rotation"))
        // {
        //     response = $"You don't have permission to execute this command. Required permission: mpr.rotation";
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

        if (!mapObject.IsRotatable)
        {
            response = "You can't modify this object's rotation!";
            return false;
        }

        if (arguments.Count >= 3 && TryGetVector(arguments.At(0), arguments.At(1), arguments.At(2), out var newRotation))
        {
            ChangingObjectRotationEventArgs ev = new(player, mapObject, newRotation, true);
            Events.Handlers.MapEditorObject.OnChangingObjectRotation(ev);

            if (!ev.IsAllowed)
            {
                response = ev.Response;
                return true;
            }

            mapObject.transform.eulerAngles += ev.Rotation;
            player.ShowGameObjectHint(mapObject);

            mapObject.UpdateObject();

            response = ev.Rotation.ToString("F3");
            return true;
        }

        response = "Invalid values.";
        return false;
    }
}