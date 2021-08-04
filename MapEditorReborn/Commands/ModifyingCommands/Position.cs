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
    /// Command used for modifing object's position.
    /// </summary>
    public class Position : ICommand
    {
        /// <inheritdoc/>
        public string Command => "position";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "pos" };

        /// <inheritdoc/>
        public string Description => "Modifies object's posistion.";

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
                response += "mp position set (x) (y) (z)\n";
                response += "mp position add (x) (y) (z)\n";
                response += "mp position bring";
                return false;
            }

            Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);

            if (!player.SessionVariables.TryGetValue(Handler.SelectedObjectSessionVarName, out object @object) || (GameObject)@object == null)
            {
                response = "You haven't selected any object!";
                return false;
            }

            GameObject gameObject = (GameObject)@object;
            Vector3 newPosition;

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
                            newPosition = new Vector3(x, y, z);

                            NetworkServer.UnSpawn(gameObject);
                            gameObject.transform.position = newPosition;
                            NetworkServer.Spawn(gameObject);

                            response = newPosition.ToString();
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
                            break;
                        }

                        if (float.TryParse(arguments.At(1), out float x) && float.TryParse(arguments.At(2), out float y) && float.TryParse(arguments.At(3), out float z))
                        {
                            newPosition = new Vector3(x, y, z);

                            NetworkServer.UnSpawn(gameObject);
                            gameObject.transform.position += newPosition;
                            NetworkServer.Spawn(gameObject);

                            response = newPosition.ToString();
                            break;
                        }

                        response = "Invalid values.";
                        return false;
                    }

                case "BRING":
                    {
                        newPosition = player.Position + (Vector3.down * 1.33f);

                        NetworkServer.UnSpawn(gameObject);
                        gameObject.transform.position = newPosition;
                        NetworkServer.Spawn(gameObject);

                        response = newPosition.ToString();
                        break;
                    }

                default:
                    {
                        response = "Invalid command option. Use a command without any arugments to get a list of valid options.";
                        return false;
                    }
            }

            gameObject.UpdateObject(player);
            return true;
        }
    }
}
