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
        public string Description => "Copies the object which you are looking at.";

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
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
                response = "You've successfully copied the object!";
                return true;
            }

            response = "You aren't looking at any Map Editor object!";
            return false;
        }
    }
}
