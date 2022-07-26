namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using System.IO;
    using API.Features;
    using API.Features.Serializable;
    using CommandSystem;
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
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Merges two or more maps into one.";

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

            MapSchematic outputMap = new(arguments.At(0));

            for (int i = 1; i < arguments.Count; i++)
            {
                MapSchematic map = MapUtils.GetMapByName(arguments.At(i));

                if (map is null)
                {
                    response = $"Map named {arguments.At(i)} does not exist!";
                    return false;
                }

                outputMap.Doors.AddRange(map.Doors);
                outputMap.WorkStations.AddRange(map.WorkStations);
                outputMap.ItemSpawnPoints.AddRange(map.ItemSpawnPoints);
                outputMap.PlayerSpawnPoints.AddRange(map.PlayerSpawnPoints);
                outputMap.RagdollSpawnPoints.AddRange(map.RagdollSpawnPoints);
                outputMap.ShootingTargets.AddRange(map.ShootingTargets);
                outputMap.Primitives.AddRange(map.Primitives);
                outputMap.LightSources.AddRange(map.LightSources);
                outputMap.RoomLights.AddRange(map.RoomLights);
                outputMap.Teleports.AddRange(map.Teleports);
                outputMap.Lockers.AddRange(map.Lockers);
                outputMap.Schematics.AddRange(map.Schematics);
            }

            File.WriteAllText(Path.Combine(MapEditorReborn.MapsDir, $"{outputMap.Name}.yml"), Loader.Serializer.Serialize(outputMap));

            response = $"You've successfully merged {arguments.Count - 1} maps into one!";
            return true;
        }
    }
}
