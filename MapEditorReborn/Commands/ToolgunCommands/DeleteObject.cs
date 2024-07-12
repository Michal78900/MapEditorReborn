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
        public bool SanitizeResponse => false;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = "У вас недостаточно прав на выполнения этой команды.";
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
                        if (map is not null)
                        {
                            map.CleanupAll();
                            response = "Вы успешно удалили объект!";
                            return true;
                        }

                        response = "Подобного объекта не существует!";
                        return false;
                    case "schematic":
                        var schematmap = SpawnedObjects.ToList().Find(map => map.name == $"CustomSchematic-{slug}" && map is SchematicObject);

                        if (schematmap is null)
                        {
                            response = "Подобного объекта не существует!";
                            return false;
                        }

                        var schematicObject = schematmap as SchematicObject;

                        if (AttachedSchemats.ContainsKey(schematicObject.AttachedPlayer))
                        {
                            AttachedSchemats.Remove(schematicObject.AttachedPlayer);
                        }

                        ToolGunHandler.DeleteObject(player, schematicObject);
                        response = "Вы успешно удалили объект!";
                        return true;
                    case "id":
                        var schematIdMap = SpawnedObjects.ToList().Find(map => map.Id == slug && map is SchematicObject);

                        if (schematIdMap is null)
                        {
                            response = "Не удалось удалить подобный объект!";
                            return false;
                        }

                        var schematId = schematIdMap as SchematicObject;

                        if (AttachedSchemats.ContainsKey(schematId.AttachedPlayer))
                        {
                            AttachedSchemats.Remove(schematId.AttachedPlayer);
                        }

                        ToolGunHandler.DeleteObject(player, schematId);
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

                var schemat = mapObject as SchematicObject;

                if (AttachedSchemats.ContainsKey(schemat.AttachedPlayer))
                {
                    AttachedSchemats.Remove(schemat.AttachedPlayer);
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
