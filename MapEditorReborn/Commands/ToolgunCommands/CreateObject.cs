namespace MapEditorReborn.Commands
{
    using System;
    using System.Linq;
    using API;
    using API.Enums;
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
                if (player.TryGetSessionVariable(Methods.CopiedObjectSessionVarName, out MapEditorObject prefab) && prefab != null)
                {
                    Vector3 forward = player.CameraTransform.forward;
                    if (!Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
                    {
                        response = "Couldn't find a valid surface on which the object could be spawned!";
                        return false;
                    }

                    Methods.SpawnPropertyObject(hit.point, prefab);
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

                response += "\nTo spawn a custom schematic, please use it's file name as an argument.";

                return true;
            }
            else
            {
                Vector3 forward = player.CameraTransform.forward;
                if (!Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
                {
                    response = "Couldn't find a valid surface on which the object could be spawned!";
                    return false;
                }

                string arg = arguments.At(0);

                if (!Enum.TryParse(arg, true, out ToolGunMode parsedEnum))
                {
                    SaveDataObjectList schematicData = Methods.GetSchematicDataByName(arg);

                    if (schematicData != null)
                    {
                        Methods.SpawnedObjects.Add(Methods.SpawnSchematic(new SchematicObject() { SchematicName = arg }, hit.point + Vector3.up, Quaternion.identity, Vector3.one));

                        response = $"{arg} has been successfully spawned!";
                        return true;
                    }

                    response = $"\"{arg}\" is an invalid object/schematic name!";
                    return false;
                }

                if (parsedEnum == ToolGunMode.RoomLight)
                {
                    Room colliderRoom = Map.FindParentRoom(hit.collider.gameObject);
                    if (Methods.SpawnedObjects.FirstOrDefault(x => x is RoomLightComponent light && light.ForcedRoomType == colliderRoom.Type) != null)
                    {
                        response = "There can be only one Light Controller per one room type!";
                        return false;
                    }
                }

                Methods.SpawnObject(hit.point, parsedEnum);
                response = $"{parsedEnum} has been successfully spawned!";

                return true;
            }
        }
    }
}
