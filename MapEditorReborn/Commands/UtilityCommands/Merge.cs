
namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using API.Features;
    using API.Features.Serializable;
    using CommandSystem;
    using Exiled.API.Features.Pools;
    using Exiled.Loader;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Merges two or more <see cref="MapSchematic"/>s into one.
    /// </summary>
    public class Merge : ICommand
    {
        /// <inheritdoc/>
        public string Command => "merge";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Merges two or more maps into one.";

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            if (arguments.Count < 3)
            {
                response = "\nUsage:\n" +
                    "mp merge outputMapName inputMap1 inputMap2 [inputMap3 ...]";

                return false;
            }

            List<MapSchematic> maps = ListPool<MapSchematic>.Pool.Get();

            for (int i = 1; i < arguments.Count; i++)
            {
                MapSchematic map = MapUtils.GetMapByName(arguments.At(i));

                if (map is null)
                {
                    response = $"Map named {arguments.At(i)} does not exist!";
                    return false;
                }

                maps.Add(map);
            }

            var outputMap = MapUtils.MergeMaps(arguments.At(0), maps);

            ListPool<MapSchematic>.Pool.Return(maps);

            File.WriteAllText(Path.Combine(MapEditorReborn.MapsDir, $"{outputMap.Name}.yml"), Loader.Serializer.Serialize(outputMap));

            response = $"You've successfully merged {arguments.Count - 1} maps into one!";
            return true;
        }
    }
}
