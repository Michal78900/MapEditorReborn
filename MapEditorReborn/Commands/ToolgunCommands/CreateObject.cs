// -----------------------------------------------------------------------
// <copyright file="CreateObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using MapEditorReborn.Exiled.Features;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.ToolgunCommands;

using System;
using System.Linq;
using API.Enums;
using API.Extensions;
using API.Features;
using API.Features.Objects;
using Events.EventArgs;
using Events.Handlers.Internal;
using UnityEngine;
using static API.API;

/// <summary>
/// Command used for creating the objects.
/// </summary>
public class CreateObject : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "create";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = { "cr", "spawn" };

    /// <inheritdoc/>
    public string Description
    {
        get => "Creates a selected object at the point you are looking at.";
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
        var i = 0;

        if (arguments.Count == 0)
        {
            if (player.TryGetSessionVariable(CopiedObjectSessionVarName, out MapEditorObject prefab) && prefab != null)
            {
                Vector3 forward = player.Camera.forward;
                if (!Physics.Raycast(player.Camera.position + forward, forward, out var hit, 100f))
                {
                    response = "Couldn't find a valid surface on which the object could be spawned!";
                    return false;
                }

                SpawnedObjects.Add(ObjectSpawner.SpawnPropertyObject(hit.point, prefab));

                if (MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn)
                    SpawnedObjects.Last().UpdateIndicator();

                response = $"Copy object has been successfully pasted!";
                return true;
            }

            response = "\nList of all spawnable objects:\n\n";
            foreach (var name in Enum.GetNames(typeof(ObjectType)))
            {
                response += $"- {name} ({i})\n";
                i++;
            }

            response += "\nTo spawn a custom schematic, please use it's file name as an argument.";

            return true;
        }

        var position = Vector3.zero;

        if (arguments.Count >= 4 && !TryGetVector(arguments.At(1), arguments.At(2), arguments.At(3), out position))
        {
            response = "Invalid arguments. Usage: mp create <object> <posX> <posY> <posZ>";
            return false;
        }

        if (arguments.Count == 1)
        {
            Vector3 forward = player.Camera.forward;
            if (!Physics.Raycast(player.Camera.position + forward, forward, out var hit, 100f))
            {
                response = "Couldn't find a valid surface on which the object could be spawned!";
                return false;
            }

            position = hit.point;
        }
        else if (arguments.Count < 4)
        {
            response = "Invalid arguments. Usage: mp create <object> optionally: <posX> <posY> <posZ>";
            return false;
        }

        var objectName = arguments.At(0);

        if (!Enum.TryParse(objectName, true, out ObjectType parsedEnum) || !Enum.IsDefined(typeof(ObjectType), parsedEnum))
        {
            var data = MapUtils.GetSchematicDataByName(objectName);

            if (data is not null)
            {
                SchematicObject schematicObject;

                SpawnedObjects.Add(schematicObject = ObjectSpawner.SpawnSchematic(objectName, position, Quaternion.identity, Vector3.one, data));

                if (MapEditorReborn.Singleton.Config.AutoSelect)
                    TrySelectObject(player, schematicObject);

                response = $"{objectName} has been successfully spawned!";
                return true;
            }

            response = $"\"{objectName}\" is an invalid object/schematic name!";
            return false;
        }

        if (parsedEnum == ObjectType.RoomLight)
        {
            Room colliderRoom = Room.Get(position);
            if (SpawnedObjects.FirstOrDefault(x => x is RoomLightObject light && light.ForcedRoomType == colliderRoom.Type) != null)
            {
                response = "There can be only one Light Controller per one room type!";
                return false;
            }
        }

        SpawningObjectEventArgs ev = new(player, position, parsedEnum, true);
        Events.Handlers.MapEditorObject.OnSpawningObject(ev);

        if (!ev.IsAllowed)
        {
            response = ev.Response;
            return true;
        }

        var pos = ev.Position;
        parsedEnum = ev.ObjectType;

        var gameObject = ToolGunHandler.SpawnObject(pos, parsedEnum);
        response = $"{parsedEnum} has been successfully spawned!";

        if (MapEditorReborn.Singleton.Config.AutoSelect)
            TrySelectObject(player, SpawnedObjects.FirstOrDefault(o => o.gameObject == gameObject));

        return true;
    }

    private static void TrySelectObject(MERPlayer player, MapEditorObject mapEditorObject)
    {
        if (mapEditorObject is null)
            return;

        var ev = new SelectingObjectEventArgs(player, mapEditorObject, true);
        Events.Handlers.MapEditorObject.OnSelectingObject(ev);

        if (!ev.IsAllowed)
            return;

        ToolGunHandler.SelectObject(player, mapEditorObject);
    }
}