// -----------------------------------------------------------------------
// <copyright file="Load.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using API.Features;
    using API.Features.Serializable;
    using CommandSystem;
    using Events.EventArgs;
    using Events.Handlers;
    using Exiled.Permissions.Extensions;
    using static API.API;

    /// <summary>
    /// Command used for loading <see cref="MapSchematic"/>.
    /// </summary>
    public class Load : ICommand
    {
        /// <inheritdoc/>
        public string Command => "load";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "l" };

        /// <inheritdoc/>
        public string Description => "Загружает карту по названию.";

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

            if (arguments.Count == 0)
            {
                response = "Вам нужно предоставить название карты!";
                return false;
            }

            MapSchematic map = MapUtils.GetMapByName(arguments.At(0));

            if (map == null)
            {
                response = "Карты с таким названием не существует!";
                return false;
            }

            LoadingMapEventArgs ev = new(CurrentLoadedMap, map);
            Map.OnLoadingMap(ev);

            if (!ev.IsAllowed)
            {
                response = ev.Response;
                return true;
            }

            CurrentLoadedMap = map;

            response = map.IsValid ? $"Вы успешно загрузили карту {arguments.At(0)}!" : $"Карта {arguments.At(0)} не может быть загружена, т.к. требуемые типы комнта не были загружены в этом раунде!";
            Log.Info(map.Name);
            return map.IsValid;
        }
    }
}
