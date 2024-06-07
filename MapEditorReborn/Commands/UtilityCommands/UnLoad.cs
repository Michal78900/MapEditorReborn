// -----------------------------------------------------------------------
// <copyright file="Unload.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using API.Features.Serializable;
    using CommandSystem;
    using Events.EventArgs;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using static API.API;
    using Map = global::MapEditorReborn.Events.Handlers.Map;

    /// <summary>
    /// Command used for unloading <see cref="MapSchematic"/>.
    /// </summary>
    public class Unload : ICommand
    {
        /// <inheritdoc/>
        public string Command => "unload";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "unl" };

        /// <inheritdoc/>
        public string Description => "Unloads currently loaded map.";

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

            UnloadingMapEventArgs ev = new(sender as Player);
            Map.OnUnloadingMap(ev);

            if (!ev.IsAllowed)
            {
                response = ev.Response;
                return true;
            }

            CurrentLoadedMap = null;
            response = "Map has been successfully unloaded!";
            return true;
        }
    }
}
