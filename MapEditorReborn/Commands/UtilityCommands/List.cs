// -----------------------------------------------------------------------
// <copyright file="List.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using API.Enums;
    using API.Features.Serializable;
    using CommandSystem;
    using Exiled.Loader;
    using Exiled.Permissions.Extensions;
    using NorthwoodLib.Pools;
    using Utf8Json;

    /// <summary>
    /// Command used for listing all saved maps.
    /// </summary>
    public class List : ICommand
    {
        /// <inheritdoc/>
        public string Command => "list";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "li" };

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

            StringBuilder builder = StringBuilderPool.Shared.Rent();

            if (arguments.Count == 0)
            {
                builder.AppendLine();
                builder.AppendLine();
                builder.Append("<color=green><b>List of maps:</b></color>");

                foreach (string filePath in Directory.GetFiles(MapEditorReborn.MapsDir))
                {
                    builder.AppendLine();
                    builder.Append($"- <color=yellow>{Path.GetFileNameWithoutExtension(filePath)}</color>");
                }

                builder.AppendLine();
                builder.AppendLine();
                builder.Append("<color=orange><b>List of schematics:</b></color>");

                foreach (string directoryPath in Directory.GetDirectories(MapEditorReborn.SchematicsDir))
                {
                    IEnumerable<string> files = Directory.GetFiles(directoryPath).Select(Path.GetFileName);
                    string jsonFilePath = files.FirstOrDefault(x => x.EndsWith(".json") && !x.Contains('-'));
                    if (jsonFilePath is null)
                        continue;

                    builder.AppendLine();
                    builder.Append($"- <color=yellow>{Path.GetFileNameWithoutExtension(jsonFilePath)}</color>");
                }

                response = StringBuilderPool.Shared.ToStringReturn(builder);
                return true;
            }

            List<string> fileNames = new(Directory.GetFiles(MapEditorReborn.MapsDir));
            fileNames.AddRange(Directory.GetDirectories(MapEditorReborn.SchematicsDir));

            string[] paths = fileNames.Where(x => x.Contains(arguments.At(0))).ToArray();

            if (!paths.Any())
            {
                response = $"\"{arguments.At(0)}\" does not exist!";
                return false;
            }

            foreach (string path in paths)
            {
                if (path.EndsWith(".yml"))
                {
                    MapSchematic map = Loader.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(path));

                    builder.AppendLine();
                    builder.AppendLine($"<color=green><b>{Path.GetFileNameWithoutExtension(path)}</b></color>");

                    builder.AppendLine($"Doors: <color=yellow><b>{map.Doors.Count}</b></color>");
                    builder.AppendLine($"Workstations: <color=yellow><b>{map.WorkStations.Count}</b></color>");
                    builder.AppendLine($"ItemSpawnPoints: <color=yellow><b>{map.ItemSpawnPoints.Count}</b></color>");
                    builder.AppendLine($"PlayerSpawnPoints: <color=yellow><b>{map.PlayerSpawnPoints.Count}</b></color>");
                    builder.AppendLine($"RagdollSpawnPoints: <color=yellow><b>{map.RagdollSpawnPoints.Count}</b></color>");
                    builder.AppendLine($"ShootingTargets: <color=yellow><b>{map.ShootingTargets.Count}</b></color>");
                    builder.AppendLine($"RoomLights: <color=yellow><b>{map.RoomLights.Count}</b></color>");
                    builder.AppendLine($"Primitives: <color=yellow><b>{map.Primitives.Count}</b></color>");
                    builder.AppendLine($"LightSources: <color=yellow><b>{map.LightSources.Count}</b></color>");
                    builder.AppendLine($"Teleports: <color=yellow><b>{map.Teleports.Count}</b></color>");
                    builder.AppendLine($"Lockers: <color=yellow><b>{map.Lockers.Count}</b></color>");
                    builder.AppendLine($"Schematics: <color=yellow><b>{map.Schematics.Count}</b></color>");
                    builder.AppendLine($"Total number of objects: <color=yellow><b>{map.Doors.Count + map.WorkStations.Count + map.ItemSpawnPoints.Count + map.PlayerSpawnPoints.Count + map.RagdollSpawnPoints.Count + map.ShootingTargets.Count + map.RoomLights.Count + map.Primitives.Count + map.LightSources.Count + map.Teleports.Count + map.Schematics.Count}</b></color>");
                }
                else
                {
                    SchematicObjectDataList data = JsonSerializer.Deserialize<SchematicObjectDataList>(File.ReadAllText(Path.Combine(path, Path.GetFileNameWithoutExtension(path) + ".json")));

                    int emptyTransformsNum = 0, primitivesNum = 0, lightsNum = 0, pickupsNum = 0, workstationsNum = 0, lockerNum = 0, totalNum = 0;

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

                            case BlockType.Locker:
                                lockerNum++;
                                break;
                        }

                        totalNum++;
                    }

                    builder.AppendLine();
                    builder.AppendLine($"<color=orange><b>{Path.GetFileNameWithoutExtension(path)}</b></color>");

                    builder.AppendLine($"Empty transforms: <color=yellow><b>{emptyTransformsNum}</b></color>");
                    builder.AppendLine($"Primitives: <color=yellow><b>{primitivesNum}</b></color>");
                    builder.AppendLine($"Lights: <color=yellow><b>{lightsNum}</b></color>");
                    builder.AppendLine($"Pickups: <color=yellow><b>{pickupsNum}</b></color>");
                    builder.AppendLine($"Workstations: <color=yellow><b>{workstationsNum}</b></color>");
                    builder.AppendLine($"Lockers: <color=yellow><b>{lockerNum}</b></color>");
                    builder.AppendLine($"Total number of blocks: <color=yellow><b>{totalNum}</b></color>");
                }
            }

            response = StringBuilderPool.Shared.ToStringReturn(builder);
            return true;
        }
    }
}