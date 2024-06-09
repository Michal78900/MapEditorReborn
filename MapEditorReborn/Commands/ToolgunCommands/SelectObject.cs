﻿// -----------------------------------------------------------------------
// <copyright file="SelectObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.ToolgunCommands
{
    using System;
    using API;
    using API.Features.Objects;
    using CommandSystem;
    using Events.EventArgs;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Command used for selecting the objects.
    /// </summary>
    public class SelectObject : ICommand
    {
        /// <inheritdoc/>
        public string Command => "select";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "sel", "choose" };

        /// <inheritdoc/>
        public string Description => "Выделяет объект, на который вы смотрите.";

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

            Player player = Player.Get(sender);
            if (!ToolGunHandler.TryGetMapObject(player, out MapEditorObject mapObject))
            {
                if (player.TryGetSessionVariable(API.SelectedObjectSessionVarName, out object _))
                {
                    ToolGunHandler.SelectObject(player, null);
                    response = "Вы успешно сняли выделение с объекта!";
                    return true;
                }

                response = "Вы не смотрите на объект!";
                return false;
            }

            SelectingObjectEventArgs ev = new(player, mapObject);
            Events.Handlers.MapEditorObject.OnSelectingObject(ev);

            if (!ev.IsAllowed)
            {
                response = ev.Response;
                return true;
            }

            if (ToolGunHandler.SelectObject(player, ev.Object))
            {
                response = "Вы успешно выделили объект!";
            }
            else
            {
                response = "Вы успешно сняли выделение с объекта!";
            }

            return true;
        }
    }
}
