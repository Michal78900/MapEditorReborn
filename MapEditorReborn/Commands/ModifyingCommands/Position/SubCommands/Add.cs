namespace MapEditorReborn.Commands.Position.SubCommands
{
    using System;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using Mirror;
    using RemoteAdmin;
    using UnityEngine;

    /// <summary>
    /// Modifies object's position by adding a certain value to the current one.
    /// </summary>
    public class Add : ICommand
    {
        /// <inheritdoc/>
        public string Command => "add";

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

            Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);

            if (!player.TryGetSessionVariable(Handler.SelectedObjectSessionVarName, out MapEditorObject mapEditorObject) || mapEditorObject == null)
            {
                response = "You haven't selected any object!";
                return false;
            }

            if (float.TryParse(arguments.At(0), out float x) && float.TryParse(arguments.At(1), out float y) && float.TryParse(arguments.At(2), out float z))
            {
                Vector3 newPosition = new Vector3(x, y, z);

                NetworkServer.UnSpawn(mapEditorObject.gameObject);
                mapEditorObject.transform.position += newPosition;
                NetworkServer.Spawn(mapEditorObject.gameObject);

                mapEditorObject.UpdateObject(player);

                response = newPosition.ToString();
                return true;
            }

            response = "Invalid values.";
            return false;
        }
    }
}
