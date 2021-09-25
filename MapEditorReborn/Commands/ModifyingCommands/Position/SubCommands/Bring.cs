namespace MapEditorReborn.Commands.Position.SubCommands
{
    using System;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using UnityEngine;

    /// <summary>
    /// Modifies object's position by setting it to the sender's current position.
    /// </summary>
    public class Bring : ICommand
    {
        /// <inheritdoc/>
        public string Command => "bring";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => string.Empty;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mpr.position"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.position";
                return false;
            }

            // Player player = Player.Get(sender);
            Player player = Player.Get(sender as CommandSender);

            if (!player.TryGetSessionVariable(Handler.SelectedObjectSessionVarName, out MapEditorObject mapEditorObject) || mapEditorObject == null)
            {
                response = "You haven't selected any object!";
                return false;
            }

            Vector3 newPosition = player.Position;

            if (mapEditorObject.name.Contains("Door"))
                newPosition += Vector3.down * 1.33f;

            // NetworkServer.UnSpawn(mapEditorObject.gameObject);
            mapEditorObject.transform.position = newPosition;
            // NetworkServer.Spawn(mapEditorObject.gameObject);

            mapEditorObject.UpdateObject();

            response = newPosition.ToString();
            return true;
        }
    }
}
