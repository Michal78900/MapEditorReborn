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
        public string Description => "Deletes the object which you are looking at.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            var player = Player.Get(sender);

            if (arguments.Count != 0)
            {
                switch (arguments.At(0))
                {
                    case "map":
                        var map = MapUtils.GetMapByName(arguments.At(1));
                        map?.CleanupAll();
                        response = "You've successfully deleted the object!";
                        return true;
                    case "schematic":
                        var schem = API.API.SpawnedObjects.ToList().FindLast(mapEditorObject => mapEditorObject.name == $"CustomSchematic-{arguments.At(1)}");
                        ToolGunHandler.DeleteObject(player, schem);
                        response = "You've successfully deleted the object!";
                        return true;
                    case "id":
                        var objectid = API.API.SpawnedObjects.ToList().FindLast(mapEditorObject => mapEditorObject.name == arguments.At(1));
                        ToolGunHandler.DeleteObject(player, objectid);
                        response = "You've successfully deleted the object!";
                        return true;
                    default:
                        response = "Введены неправильные аргументы!";
                        return false;
                }
            }

            if (ToolGunHandler.TryGetMapObject(player, out MapEditorObject mapObject))
            {
                DeletingObjectEventArgs ev = new(player, mapObject);
                Events.Handlers.MapEditorObject.OnDeletingObject(ev);

                if (!ev.IsAllowed)
                {
                    response = ev.Response;
                    return true;
                }

                ToolGunHandler.DeleteObject(player, ev.Object);
                response = "You've successfully deleted the object!";

                return true;
            }

            response = "You aren't looking at any Map Editor object!";
            return false;
        }
    }
}
