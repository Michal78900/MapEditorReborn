namespace MapEditorReborn.Commands
{
    using System;
    using System.IO;
    using API;
    using CommandSystem;
    using Exiled.Loader;
    using Exiled.Permissions.Extensions;

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
        public string Description => "Loads a map.";

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

            MapSchematic map = Handler.GetMapByName(arguments.At(0));

            // string path = Path.Combine(MapEditorReborn.PluginDir, $"{arguments.At(0)}.yml");

            // if (!File.Exists(path))
            if (map == null)
            {
                response = $"MapSchematic with this name does not exist!";
                return false;
            }

            // Handler.CurrentLoadedMap = Loader.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(path));
            Handler.CurrentLoadedMap = map;

            response = $"You've successfully loaded map named {arguments.At(0)}!";
            return true;
        }
    }
}
