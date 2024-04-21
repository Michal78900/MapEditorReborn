// -----------------------------------------------------------------------
// <copyright file="DeleteObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using MapEditorReborn.API.Features;

namespace MapEditorReborn.Commands.ToolgunCommands
{
    using System;
    using API.Features.Objects;
    using CommandSystem;
    using Events.EventArgs;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using static API.API;

    /// <summary>
    /// Command used for deleting the objects.
    /// </summary>
    public class DeleteObject : ICommand
    {
        /// <inheritdoc/>
        public string Command => "delete";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "del", "remove", "rm" };

        /// <inheritdoc/>
        public string Description => "Удаляет выделенный объект или по аргументу map/schematic/id.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            if (!Player.TryGet(sender, out var player))
            {
                response = "Не смог получить игрока!";
                return false;
            }

            if (arguments.Count > 1)
            {
                var slug = arguments.At(1);
                switch (arguments.At(0))
                {
                    case "map":
                        var map = MapUtils.GetMapByName(slug);
                        map?.CleanupAll();
                        response = "Вы успешно удалили объект!";
                        return true;
                    case "schematic":
                        var schem = SpawnedObjects.ToList().FindLast(mapEditorObject => mapEditorObject.name == $"CustomSchematic-{slug}");
                        ToolGunHandler.DeleteObject(player, schem);
                        response = "Вы успешно удалили объект!";
                        return true;
                    case "id":
                        foreach (var merobject in SpawnedObjects.ToList())
                        {
                            if (merobject is not SchematicObject schematic)
                            {
                                continue;
                            }

                            if (schematic.Id == slug)
                            {
                                ToolGunHandler.DeleteObject(player, schematic);
                            }
                        }

                        response = "Вы успешно удалили объект!";
                        return true;
                    default:
                        response = "Введены неправильные аргументы!";
                        return false;
                }
            }

            if (player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject))
            {
                DeletingObjectEventArgs ev = new(player, mapObject);
                Events.Handlers.MapEditorObject.OnDeletingObject(ev);

                if (!ev.IsAllowed)
                {
                    response = ev.Response;
                    return true;
                }

                ToolGunHandler.DeleteObject(player, ev.Object);
                response = "Вы успешно удалили объект!";

                return true;
            }

            response = "Вы не выбрали объект для удаления!";
            return false;
        }
    }
}
