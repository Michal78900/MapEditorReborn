// -----------------------------------------------------------------------
// <copyright file="CopyObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

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
    /// Command used for copying the objects.
    /// </summary>
    public class CopyObject : ICommand
    {
        /// <inheritdoc/>
        public string Command => "copy";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "cp" };

        /// <inheritdoc/>
        public string Description => "Копирует объект, на который вы смотрите.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = "У вас недостаточно прав на выполнения этой команды.";
                return false;
            }

            Player player = Player.Get(sender);

            if (ToolGunHandler.TryGetMapObject(player, out MapEditorObject mapObject))
            {
                CopyingObjectEventArgs ev = new(player, mapObject);
                Events.Handlers.MapEditorObject.OnCopyingObject(ev);

                if (!ev.IsAllowed)
                {
                    response = ev.Response;
                    return true;
                }

                ToolGunHandler.CopyObject(player, ev.Object);
                response = "Вы успешно скопировали объект!";
                return true;
            }

            response = "Вы не смотрите на объект для копирования!";
            return false;
        }
    }
}
