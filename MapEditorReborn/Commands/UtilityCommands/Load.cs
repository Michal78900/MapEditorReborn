namespace MapEditorReborn.Commands
{
    using System;
    using API.Features;
    using API.Features.Objects;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    using static API.API;

    /// <summary>
    /// Command used for loading <see cref="MapSchematic"/>.
    /// </summary>
    public class Load : ICommand
    {
        /// <inheritdoc/>
        public string Command => "load";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "l" };

        /// <inheritdoc/>
        public string Description => "Loads the map.";

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

            MapSchematic map = MapUtils.GetMapByName(arguments.At(0));

            if (map == null)
            {
                response = $"MapSchematic with this name does not exist!";
                return false;
            }

            CurrentLoadedMap = map;

            response = $"You've successfully loaded map named {arguments.At(0)}!";
            return true;
        }
    }
}
