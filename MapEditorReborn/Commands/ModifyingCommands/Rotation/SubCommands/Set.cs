﻿namespace MapEditorReborn.Commands.Rotation.SubCommands
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
    /// Modifies object's rotation by setting it to a certain value.
    /// </summary>
    public class Set : ICommand
    {
        /// <inheritdoc/>
        public string Command => "set";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => string.Empty;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mpr.rotation"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.rotation";
                return false;
            }

            Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);

            if (!player.TryGetSessionVariable(Handler.SelectedObjectSessionVarName, out MapEditorObject mapEditorObject) || mapEditorObject == null)
            {
                response = "You haven't selected any object!";
                return false;
            }

            if (mapEditorObject is PlayerSpawnPointComponent)
            {
                response = "You can't modify this object's rotation!";
                return false;
            }

            if (float.TryParse(arguments.At(0), out float x) && float.TryParse(arguments.At(1), out float y) && float.TryParse(arguments.At(2), out float z))
            {
                Quaternion newRotation = Quaternion.Euler(x, y, z);

                NetworkServer.UnSpawn(mapEditorObject.gameObject);
                mapEditorObject.transform.rotation = newRotation;
                NetworkServer.Spawn(mapEditorObject.gameObject);

                mapEditorObject.UpdateObject(player);

                response = newRotation.ToString();
                return true;
            }

            response = "Invalid values.";
            return false;
        }
    }
}
