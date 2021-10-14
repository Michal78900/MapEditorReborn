namespace MapEditorReborn.Commands
{
    using System;
    using System.Linq;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using UnityEngine;

    /// <summary>
    /// Command used for creating the objects.
    /// </summary>
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

            Player player = Player.Get(sender);

            if (arguments.Count == 0)
            {
                if (player.TryGetSessionVariable(Handler.CopiedObjectSessionVarName, out MapEditorObject prefab) && prefab != null)
                {
                    Vector3 forward = player.CameraTransform.forward;
                    if (!Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
                    {
                        response = "Couldn't find a valid surface on which the object could be spawned!";
                        return false;
                    }

                    Handler.SpawnPropertyObject(hit.point, prefab);
                    response = $"Copy object has been successfully pasted!";
                    return true;
                }

                int i = 0;

                response = "\nList of all spawnable objects:\n\n";
                foreach (string name in Enum.GetNames(typeof(ToolGunMode)))
                {
                    response += $"- {name} ({i})\n";
                    i++;
                }

                return true;
            }
            else
            {
                if (!Enum.TryParse(arguments.At(0), true, out ToolGunMode parsedEnum))
                {
                    response = $"\"{arguments.At(0)}\" is an invalid object name!";
                    return false;
                }

                Vector3 forward = player.CameraTransform.forward;
                if (!Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
                {
                    response = "Couldn't find a valid surface on which the object could be spawned!";
                    return false;
                }

                if (parsedEnum == ToolGunMode.LightController)
                {
                    Room colliderRoom = Map.FindParentRoom(hit.collider.gameObject);
                    if (Handler.SpawnedObjects.FirstOrDefault(x => x is LightControllerComponent light && (Map.FindParentRoom(x.gameObject) == colliderRoom || light.Base.RoomType == colliderRoom.Type)) != null)
                    {
                        response = "There can be only one Light Controller per one room type!";
                        return false;
                    }
                }

                Handler.SpawnObject(hit.point, parsedEnum);
                response = $"{parsedEnum} has been successfully spawned!";

                return true;
            }
        }
    }
}
