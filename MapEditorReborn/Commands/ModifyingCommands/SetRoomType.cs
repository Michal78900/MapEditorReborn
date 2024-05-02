﻿// -----------------------------------------------------------------------
// <copyright file="SetRoomType.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.ModifyingCommands
{
    using System;
    using System.Linq;
    using API.Extensions;
    using API.Features.Objects;
    using CommandSystem;
    using Events.Handlers.Internal;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using static API.API;

    /// <summary>
    /// Command used for setting/reseting object's RoomType.
    /// </summary>
    public class SetRoomType : ICommand
    {
        /// <inheritdoc/>
        public string Command => "setroomtype";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "setroom", "resetroom", "rr" };

        /// <inheritdoc/>
        public string Description => "Устанавливает тип комнаты для объекта.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = "У вас недостаточно прав на выполнения этой команды.";
                return false;
            }

            Player player = Player.Get(sender);
            if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
            {
                if (!ToolGunHandler.TryGetMapObject(player, out mapObject))
                {
                    response = "Вы не выделели объект!";
                    return false;
                }

                ToolGunHandler.SelectObject(player, mapObject);
            }

            if (arguments.Count == 0)
            {
                if (mapObject is RoomLightObject _)
                {
                    RoomType playerRoomType = player.CurrentRoom.Type;
                    if (SpawnedObjects.FirstOrDefault(x => x is RoomLightObject light && light.ForcedRoomType == playerRoomType) != null)
                    {
                        response = "В комнате может быть только один контроллер света!";
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

                response = $"Тип комнаты для объект успешно поменялся! Текущий тип: \"{mapObject.ForcedRoomType}\"";
                return true;
            }

            if (Enum.TryParse(arguments.At(0), true, out RoomType roomType))
            {
                if (roomType == RoomType.Unknown)
                    roomType = RoomType.Surface;

                if (SpawnedObjects.FirstOrDefault(x => x is RoomLightObject light && light.ForcedRoomType == player.CurrentRoom.Type) != null)
                {
                    response = "В комнате может быть только один контроллер света!";
                    return false;
                }

                mapObject.ForcedRoomType = roomType;
                player.ShowGameObjectHint(mapObject);

                if (mapObject is RoomLightObject)
                    mapObject.UpdateObject();

                response = $"Тип комнаты для объект успешно поменялся на \"{roomType}\"!";
                return true;
            }

            response = $"\"Типа комнаты \"{arguments.At(0)}\" не существует!";
            return false;
        }
    }
}
