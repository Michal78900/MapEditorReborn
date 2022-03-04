namespace MapEditorReborn.Commands
{
    using System;
    using System.Linq;
    using API.Extensions;
    using API.Features.Components;
    using API.Features.Components.ObjectComponents;
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
        public string[] Aliases => new string[] { "si" };

        /// <inheritdoc/>
        public string Description => "Shows indicators for both player and item spawn points.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            var indicators = SpawnedObjects.Where(x => x is IndicatorObjectComponent).ToList();

            if (indicators.Count != 0)
            {
                foreach (IndicatorObjectComponent indicator in indicators.ToList())
                {
                    SpawnedObjects.Remove(indicator);
                    indicator.Destroy();
                }

                Player player = Player.Get(sender);
                if (player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject))
                {
                    if (mapObject is ItemSpawnPointComponent || mapObject is PlayerSpawnPointComponent || mapObject is RagdollSpawnPointComponent || mapObject is TeleportControllerComponent)
                        ToolGunHandler.SelectObject(player, null);
                }

                response = "Removed all indicators!";
                return true;
            }

            foreach (MapEditorObject mapEditorObject in SpawnedObjects.ToList())
            {
                if (mapEditorObject is TeleportControllerComponent teleportController)
                {
                    teleportController.EntranceTeleport.UpdateIndicator();

                    foreach (var exist in teleportController.ExitTeleports)
                    {
                        exist.UpdateIndicator();
                    }
                }
                else
                {
                    mapEditorObject.UpdateIndicator();
                }
            }

            response = "Indicators have been shown!";
            return true;
        }
    }
}
