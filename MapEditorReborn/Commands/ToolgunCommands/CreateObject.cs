// -----------------------------------------------------------------------
// <copyright file="CreateObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.ToolgunCommands
{
    using System;
    using System.Linq;
    using API.Enums;
    using API.Extensions;
    using API.Features;
    using API.Features.Objects;
    using API.Features.Serializable;
    using CommandSystem;
    using Events.EventArgs;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using UnityEngine;
    using static API.API;

    /// <summary>
    /// Command used for creating the objects.
    /// </summary>
    public class CreateObject : ICommand
    {
        /// <inheritdoc/>
        public string Command => "create";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "cr", "spawn" };

        /// <inheritdoc/>
        public string Description => "Создаёт объект на точку прицела";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = "У вас недостаточно прав на выполнения этой команды.";
                return false;
            }

            Player player = Player.Get(sender);
            int i = 0;

            if (arguments.Count == 0)
            {
                if (player.TryGetSessionVariable(CopiedObjectSessionVarName, out MapEditorObject prefab) && prefab != null)
                {
                    Vector3 forward = player.CameraTransform.forward;
                    if (!Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
                    {
                        response = "Невозможно определить поверхность для создания объекта!";
                        return false;
                    }

                    SpawnedObjects.Add(ObjectSpawner.SpawnPropertyObject(hit.point, prefab));

                    if (MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn)
                    {
                        SpawnedObjects.Last().UpdateIndicator();
                    }

                    var count = SpawnedObjects.ToList()
                        .FindAll(mapEditorObject => mapEditorObject.Id == arguments.At(1))
                        .Count;

                    if (count != 0)
                    {
                        prefab.Id = $"{prefab.name}{count}";
                    }

                    response = "Скопированный объект успешно создан!";
                    return true;
                }

                response = "\nСписок доступный объектов:\n\n";
                foreach (string name in Enum.GetNames(typeof(ObjectType)))
                {
                    response += $"- {name} ({i})\n";
                    i++;
                }

                response += "\nЧтобы заспавнить схемат, предоставьте его название в аргументах.";

                return true;
            }

            Vector3 position = Vector3.zero;

            if (arguments.Count >= 4 && !TryGetVector(arguments.At(1), arguments.At(2), arguments.At(3), out position))
            {
                response = "Неправильно введены аргументы. Пример: mp create <объектt> <X> <Y> <Z>";
                return false;
            }

            if (arguments.Count == 1)
            {
                Vector3 forward = player.CameraTransform.forward;
                if (!Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
                {
                    response = "Невозможно определить поверхность для создания объекта!";
                    return false;
                }

                position = hit.point;
            }
            else if (arguments.Count < 4)
            {
                response = "Неправильно введены аргументы. Пример: mp create <объектt> опционально: <X> <Y> <Z>";
                return false;
            }

            string objectName = arguments.At(0);

            if (!Enum.TryParse(objectName, true, out ObjectType parsedEnum) || !Enum.IsDefined(typeof(ObjectType), parsedEnum))
            {
                SchematicObjectDataList data = MapUtils.GetSchematicDataByName(objectName);

                if (data is not null)
                {
                    SchematicObject schematicObject;

                    SpawnedObjects.Add(schematicObject = ObjectSpawner.SpawnSchematic(objectName, position, Quaternion.identity, Vector3.one, data, true));
                    SpawnedSchemats.Add(schematicObject);

                    if (MapEditorReborn.Singleton.Config.AutoSelect)
                        TrySelectObject(player, schematicObject);

                    response = $"{objectName} был успешно создан!";
                    return true;
                }

                response = $"\"Объект с названием {objectName} не существует!\"!";
                return false;
            }

            if (parsedEnum == ObjectType.RoomLight)
            {
                Room colliderRoom = Room.Get(position);
                if (SpawnedObjects.FirstOrDefault(x => x is RoomLightObject light && light.ForcedRoomType == colliderRoom.Type) != null)
                {
                    response = "В комнате может быть только один контроллер света!";
                    return false;
                }
            }

            SpawningObjectEventArgs ev = new(player, position, parsedEnum);
            Events.Handlers.MapEditorObject.OnSpawningObject(ev);

            if (!ev.IsAllowed)
            {
                response = ev.Response;
                return true;
            }

            Vector3 pos = ev.Position;
            parsedEnum = ev.ObjectType;

            GameObject gameObject = ToolGunHandler.SpawnObject(pos, parsedEnum);
            response = $"{parsedEnum} был успешно создан!";

            if (MapEditorReborn.Singleton.Config.AutoSelect)
                TrySelectObject(player, SpawnedObjects.FirstOrDefault(o => o.gameObject == gameObject));

            return true;
        }

        private static void TrySelectObject(Player player, MapEditorObject mapEditorObject)
        {
            if (mapEditorObject is null)
                return;

            SelectingObjectEventArgs ev = new SelectingObjectEventArgs(player, mapEditorObject);
            Events.Handlers.MapEditorObject.OnSelectingObject(ev);

            if (!ev.IsAllowed)
                return;

            ToolGunHandler.SelectObject(player, mapEditorObject);
        }
    }
}
