﻿// -----------------------------------------------------------------------
// <copyright file="SetRoomType.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.Exiled.Enums;
using MapEditorReborn.Exiled.Features;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.ModifyingCommands;

using System;
using System.Linq;
using API.Features.Objects;
using Events.Handlers.Internal;
using static API.API;

/// <summary>
/// Command used for setting/reseting object's RoomType.
/// </summary>
public class SetRoomType : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "setroomtype";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = { "setroom", "resetroom", "rr" };

    /// <inheritdoc/>
    public string Description
    {
        get => "Sets the object's room type.";
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
        if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
        {
            if (!ToolGunHandler.TryGetMapObject(player, out mapObject))
            {
                response = "You haven't selected any object!";
                return false;
            }

            ToolGunHandler.SelectObject(player, mapObject);
        }

        if (arguments.Count == 0)
        {
            if (mapObject is RoomLightObject _)
            {
                RoomType playerRoomType = Room.Get(player.Room).Type;
                if (SpawnedObjects.FirstOrDefault(x => x is RoomLightObject light && light.ForcedRoomType == playerRoomType) != null)
                {
                    response = "There can be only one Light Controller per one room type!";
                    return false;
                }

                mapObject.ForcedRoomType = RoomType.Unknown;
                mapObject.ForcedRoomType = playerRoomType;
            }
            else
            {
                mapObject.ForcedRoomType = RoomType.Unknown;
                mapObject.ForcedRoomType = mapObject.FindRoom().Type;
            }

            player.ShowGameObjectHint(mapObject);

            if (mapObject is RoomLightObject)
                mapObject.UpdateObject();

            response = $"Object's RoomType has been reseted! It is now \"{mapObject.ForcedRoomType}\"";
            return true;
        }

        if (Enum.TryParse(arguments.At(0), true, out RoomType roomType))
        {
            if (roomType == RoomType.Unknown)
                roomType = RoomType.Surface;

            if (SpawnedObjects.FirstOrDefault(x => x is RoomLightObject light && light.ForcedRoomType == Room.Get(player.Room).Type) != null)
            {
                response = "There can be only one Light Controller per one room type!";
                return false;
            }

            mapObject.ForcedRoomType = roomType;
            player.ShowGameObjectHint(mapObject);

            if (mapObject is RoomLightObject)
                mapObject.UpdateObject();

            response = $"Object's RoomType has been set to \"{roomType}\"!";
            return true;
        }

        response = $"\"{arguments.At(0)}\" is an invalid room type!";
        return false;
    }
}