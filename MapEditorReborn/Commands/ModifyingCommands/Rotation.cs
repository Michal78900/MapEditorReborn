namespace MapEditorReborn.Commands
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
    /// Command used for modifing object's rotation.
    /// </summary>
    public class Rotation : ICommand
    {
        /// <inheritdoc/>
        public string Command => "rotation";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "rot" };

        /// <inheritdoc/>
        public string Description => "Modifies object's rotation.";

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
                response = "\nUsage:\n";
                response += "mp rotation set (x) (y) (z)\n";
                response += "mp rotation add (x) (y) (z)\n";
                return false;
            }

            Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);

            if (!player.SessionVariables.TryGetValue(Handler.SelectedObjectSessionVarName, out object @object) || (GameObject)@object == null)
            {
                response = "You haven't selected any object!";
                return false;
            }

            GameObject gameObject = (GameObject)@object;

            if (gameObject.name == "PlayerSpawnPointObject(Clone)")
            {
                response = "You can't modify this object's rotation!";
                return false;
            }

            Quaternion newRotation;

            switch (arguments.At(0).ToUpper())
            {
                case "SET":
                    {
                        if (arguments.Count < 4)
                        {
                            response = "You need to provide all X Y Z arguments!";
                            return false;
                        }

                        if (float.TryParse(arguments.At(1), out float x) && float.TryParse(arguments.At(2), out float y) && float.TryParse(arguments.At(3), out float z))
                        {
                            newRotation = Quaternion.Euler(x, y, z);

                            NetworkServer.UnSpawn(gameObject);
                            gameObject.transform.rotation = newRotation;
                            NetworkServer.Spawn(gameObject);

                            response = newRotation.ToString();
                            break;
                        }

                        response = "Invalid values.";
                        return false;
                    }

                case "ADD":
                    {
                        if (arguments.Count < 4)
                        {
                            response = "You need to provide all X Y Z arguments!";
                            return false;
                        }

                        if (float.TryParse(arguments.At(1), out float x) && float.TryParse(arguments.At(2), out float y) && float.TryParse(arguments.At(3), out float z))
                        {
                            newRotation = Quaternion.Euler(x, y, z);

                            NetworkServer.UnSpawn(gameObject);
                            gameObject.transform.rotation *= newRotation;
                            NetworkServer.Spawn(gameObject);

                            response = newRotation.ToString();
                            break;
                        }

                        response = "Invalid values.";
                        return false;
                    }

                default:
                    {
                        response = "Invalid command option. Use a command without any arugments to get a list of valid options.";
                        return false;
                    }
            }

            gameObject.UpdateIndicator();
            return true;
        }
    }
}
