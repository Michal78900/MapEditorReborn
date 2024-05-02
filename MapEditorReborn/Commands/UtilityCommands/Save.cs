// -----------------------------------------------------------------------
// <copyright file="Save.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using API.Features;
    using API.Features.Serializable;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Command used for saving <see cref="MapSchematic"/>.
    /// </summary>
    public class Save : ICommand
    {
        /// <inheritdoc/>
        public string Command => "save";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "s" };

        /// <inheritdoc/>
        public string Description => "Сохраняет текущие объекты в карту.";

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
                response = "Вам нужно придумать название карты!";
                return false;
            }

            MapUtils.SaveMap(arguments.At(0));

            response = $"Карта с названием {arguments.At(0)} успешно сохранена!";
            return true;
        }
    }
}
