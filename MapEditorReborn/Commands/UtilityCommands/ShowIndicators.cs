// -----------------------------------------------------------------------
// <copyright file="ShowIndicators.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using API.Extensions;
    using API.Features.Objects;
    using CommandSystem;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using static API.API;

    /// <summary>
    /// Command used for showing indicators.
    /// </summary>
    public class ShowIndicators : ICommand
    {
        /// <inheritdoc/>
        public string Command => "showindicators";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "si" };

        /// <inheritdoc/>
        public string Description => "Shows indicators for both player and item spawn points.";

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

            List<MapEditorObject> indicators = SpawnedObjects.Where(x => x is IndicatorObject).ToList();

            if (indicators.Count != 0)
            {
                foreach (IndicatorObject indicator in indicators.ToList())
                {
                    SpawnedObjects.Remove(indicator);
                    indicator.Destroy();
                }

                Player player = Player.Get(sender);
                if (player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject))
                {
                    if (mapObject is ItemSpawnPointObject or PlayerSpawnPointObject or RagdollSpawnPointObject or TeleportObject)
                        ToolGunHandler.SelectObject(player, null);
                }

                response = "Removed all indicators!";
                return true;
            }

            foreach (MapEditorObject mapEditorObject in SpawnedObjects.ToList())
            {
                /*
                if (mapEditorObject is TeleportControllerObject teleportController)
                {
                    teleportController.EntranceTeleport.UpdateIndicator();

                    foreach (TeleportObject exist in teleportController.ExitTeleports)
                    {
                        exist.UpdateIndicator();
                    }
                }
                else
                {
                    mapEditorObject.UpdateIndicator();
                }
                */

                mapEditorObject.UpdateIndicator();
            }

            response = "Indicators have been shown!";
            return true;
        }
    }
}
