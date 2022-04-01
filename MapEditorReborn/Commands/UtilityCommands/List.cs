namespace MapEditorReborn.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using API.Enums;
    using API.Features.Serializable;
    using CommandSystem;
    using Exiled.Loader;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Command used for listing all saved maps.
    /// </summary>
    public class List : ICommand
    {
        /// <inheritdoc/>
        public string Command => "list";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "li" };

        /// <inheritdoc/>
        public string Description => "Shows the list of all available maps.";

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
                response = "\n\n<color=green><b>List of maps:</b></color>\n";

                foreach (string filePath in Directory.GetFiles(MapEditorReborn.MapsDir))
                {
                    response += $"- <color=yellow>{Path.GetFileNameWithoutExtension(filePath)}</color>\n";
                }

                response += "\n<color=orange><b>List of schematics:</b></color>\n";

                foreach (string filePath in Directory.GetDirectories(MapEditorReborn.SchematicsDir))
                {
                    response += $"- <color=yellow>{Path.GetFileNameWithoutExtension(filePath)}</color>\n";
                }

                return true;
            }
            else
            {
                List<string> fileNames = new List<string>(Directory.GetFiles(MapEditorReborn.MapsDir));
                fileNames.AddRange(Directory.GetDirectories(MapEditorReborn.SchematicsDir));

                string[] paths = fileNames.Where(x => x.Contains(arguments.At(0))).ToArray();

                if (paths == null)
                {
                    response = $"\"{arguments.At(0)}\" does not exist!";
                    return false;
                }

                response = string.Empty;

                foreach (string path in paths)
                {
                    if (path.EndsWith(".yml"))
                    {
                        MapSchematic map = Loader.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(path));

                        response += $"\n<color=green><b>{Path.GetFileNameWithoutExtension(path)}</b></color>\n";
                        response += $"Doors: <color=yellow><b>{map.Doors.Count}</b></color>\n";
                        response += $"Workstations: <color=yellow><b>{map.WorkStations.Count}</b></color>\n";
                        response += $"ItemSpawnPoints: <color=yellow><b>{map.ItemSpawnPoints.Count}</b></color>\n";
                        response += $"PlayerSpawnPoints: <color=yellow><b>{map.PlayerSpawnPoints.Count}</b></color>\n";
                        response += $"RagdollSpawnPoints: <color=yellow><b>{map.RagdollSpawnPoints.Count}</b></color>\n";
                        response += $"ShootingTargets: <color=yellow><b>{map.ShootingTargets.Count}</b></color>\n";
                        response += $"RoomLights: <color=yellow><b>{map.RoomLights.Count}</b></color>\n";
                        response += $"Teleports: <color=yellow><b>{map.Teleports.Count}</b></color>\n";
                        response += $"Schematics: <color=yellow><b>{map.Teleports.Count}</b></color>\n";
                        response += $"Total number of objects: <color=yellow><b>{map.Doors.Count + map.WorkStations.Count + map.ItemSpawnPoints.Count + map.PlayerSpawnPoints.Count + map.RagdollSpawnPoints.Count + map.ShootingTargets.Count + map.RoomLights.Count + map.Teleports.Count + map.Schematics.Count}</b></color>\n\n";
                    }
                    else
                    {
                        SchematicObjectDataList data = Utf8Json.JsonSerializer.Deserialize<SchematicObjectDataList>(File.ReadAllText(Path.Combine(path, Path.GetFileNameWithoutExtension(path) + ".json")));

                        int emptyTransformsNum = 0, primitivesNum = 0, lightsNum = 0, pickupsNum = 0, workstationsNum = 0, totalNum = 0;

                        foreach (SchematicBlockData block in data.Blocks)
                        {
                            switch (block.BlockType)
                            {
                                case BlockType.Empty:
                                    emptyTransformsNum++;
                                    break;

                                case BlockType.Primitive:
                                    primitivesNum++;
                                    break;

                                case BlockType.Light:
                                    lightsNum++;
                                    break;

                                case BlockType.Pickup:
                                    pickupsNum++;
                                    break;

                                case BlockType.Workstation:
                                    workstationsNum++;
                                    break;
                            }

                            totalNum++;
                        }

                        response += $"\n<color=orange><b>{Path.GetFileNameWithoutExtension(path)}</b></color>\n";
                        response += $"Empty transforms: <color=yellow><b>{emptyTransformsNum}</b></color>\n";
                        response += $"Primitives: <color=yellow><b>{primitivesNum}</b></color>\n";
                        response += $"Lights: <color=yellow><b>{lightsNum}</b></color>\n";
                        response += $"Pickups: <color=yellow><b>{pickupsNum}</b></color>\n";
                        response += $"Workstations: <color=yellow><b>{workstationsNum}</b></color>\n";
                        response += $"Total number of blocks: <color=yellow><b>{totalNum}</b></color>";
                    }
                }
            }

            return true;
        }
    }
}
