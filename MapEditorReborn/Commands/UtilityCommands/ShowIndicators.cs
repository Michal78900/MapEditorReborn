namespace MapEditorReborn.Commands
{
    using System;
    using System.Linq;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

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

            var indicators = Handler.SpawnedObjects.FindAll(x => x is IndicatorObjectComponent);

            if (indicators.Count != 0)
            {
                foreach (IndicatorObjectComponent indicator in indicators.ToList())
                {
                    Handler.SpawnedObjects.Remove(indicator);
                    indicator.Destroy();
                }

                Player player = Player.Get(sender);
                if (player.TryGetSessionVariable(Handler.SelectedObjectSessionVarName, out MapEditorObject mapObject))
                {
                    if (mapObject is ItemSpawnPointComponent || mapObject is PlayerSpawnPointComponent || mapObject is RagdollSpawnPointComponent || mapObject is TeleportControllerComponent)
                        Handler.SelectObject(player, null);
                }

                response = "Removed all indicators!";
                return true;
            }

            foreach (MapEditorObject mapEditorObject in Handler.SpawnedObjects.ToList())
            {
                mapEditorObject.UpdateIndicator();
            }

            response = "Indicators have been shown!";
            return true;
        }
    }
}
