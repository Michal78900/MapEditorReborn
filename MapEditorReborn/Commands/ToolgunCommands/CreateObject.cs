namespace MapEditorReborn.Commands
{
    using System;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;
    using UnityEngine;

    public class CreateObject : ICommand
    {
        /// <inheritdoc/>
        public string Command => "create";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "cr", "spawn" };

        /// <inheritdoc/>
        public string Description => "Creates a selected object at the point you are looking at.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            if (arguments.Count == 0)
            {
                int i = 0;

                response = "\nList of all spawnable objects:\n\n";
                foreach (string name in Enum.GetNames(typeof(ToolGunMode)))
                {
                    response += $"- {name} ({i})\n";
                    i++;
                }

                return true;
            }

            if (!Enum.TryParse(arguments.At(0), true, out ToolGunMode parsedEnum))
            {
                response = $"\"{arguments.At(0)}\" is an invalid object name!";
                return false;
            }

            // Player player = Player.Get(sender);
            Player player = Player.Get(sender as CommandSender);

            Vector3 forward = player.CameraTransform.forward;
            if (!Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
            {
                response = "Couldn't find a valid surface on which the object could be spawned!";
                return false;
            }

            Handler.SpawnObject(hit.point, parsedEnum);

            response = $"{parsedEnum} has been successfully spawned!";
            return true;
        }
    }
}
