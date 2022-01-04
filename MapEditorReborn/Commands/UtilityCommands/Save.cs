namespace MapEditorReborn.Commands
{
    using System;
    using API.Features;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Command used for saving <see cref="API.Features.Objects.MapSchematic"/>.
    /// </summary>
    public class Save : ICommand
    {
        /// <inheritdoc/>
        public string Command => "save";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "s" };

        /// <inheritdoc/>
        public string Description => "Saves the map.";

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
                response = "You need to provide a map name!";
                return false;
            }

            MapUtils.SaveMap(arguments.At(0));

            response = $"MapSchematic named {arguments.At(0)} has been successfully saved!";
            return true;
        }
    }
}
