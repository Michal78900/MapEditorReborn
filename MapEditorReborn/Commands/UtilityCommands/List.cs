namespace MapEditorReborn.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using HarmonyLib;
    using API;
    using Exiled.Loader;
    using Exiled.API.Features;

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

                foreach (string filePath in Directory.GetFiles(MapEditorReborn.SchematicsDir))
                {
                    response += $"- <color=yellow>{Path.GetFileNameWithoutExtension(filePath)}</color>\n";
                }

                return true;
            }
            else
            {
                List<string> fileNames = new List<string>(Directory.GetFiles(MapEditorReborn.MapsDir));
                fileNames.AddRange(Directory.GetFiles(MapEditorReborn.SchematicsDir));

                string path = fileNames.FirstOrDefault(x => x.Contains(arguments.At(0)));

                if (path == null)
                {
                    response = $"\"{arguments.At(0)}\" does not exist!";
                    return false;
                }

                if (path.EndsWith(".json"))
                {
                    SaveDataObjectList data = Utf8Json.JsonSerializer.Deserialize<SaveDataObjectList>(File.ReadAllText(path));

                    response = $"\n<color=yellow><b>{Path.GetFileNameWithoutExtension(path)}</b></color>\n";
                    response += $"Number of blocks: <color=yellow><b>{data.Blocks.Count}</b></color>";
                }
                else
                {
                    MapSchematic map = Loader.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(path));

                    int doorsNum = map.Doors.Count;
                    int workstationsNum = map.WorkStations.Count;
                    int itemSpawnPointNum = map.ItemSpawnPoints.Count;
                    int playerSpawnPointsNum = map.PlayerSpawnPoints.Count;
                    int ragdollSpawnPointsNum = map.RagdollSpawnPoints.Count;
                    int shootingTargetsNum = map.ShootingTargetObjects.Count;
                    int lightControllersNum = map.LightControllerObjects.Count;
                    int teleportersNum = map.TeleportObjects.Count;
                    int schematicsNum = map.TeleportObjects.Count;

                    response = $"\n<color=yellow><b>{Path.GetFileNameWithoutExtension(path)}</b></color>\n";
                    response += $"Doors: <color=yellow><b>{doorsNum}</b></color>\n";
                    response += $"Workstations: <color=yellow><b>{workstationsNum}</b></color>\n";
                    response += $"ItemSpawnPoints: <color=yellow><b>{itemSpawnPointNum}</b></color>\n";
                    response += $"PlayerSpawnPoints: <color=yellow><b>{playerSpawnPointsNum}</b></color>\n";
                    response += $"RagdollSpawnPoints: <color=yellow><b>{ragdollSpawnPointsNum}</b></color>\n";
                    response += $"ShootingTargets: <color=yellow><b>{shootingTargetsNum}</b></color>\n";
                    response += $"LightControllers: <color=yellow><b>{lightControllersNum}</b></color>\n";
                    response += $"Teleports: <color=yellow><b>{teleportersNum}</b></color>\n";
                    response += $"Schematics: <color=yellow><b>{schematicsNum}</b></color>\n";
                    response += $"Total number of objects: <color=yellow><b>{doorsNum + workstationsNum + itemSpawnPointNum + playerSpawnPointsNum + ragdollSpawnPointsNum + shootingTargetsNum + lightControllersNum + teleportersNum + schematicsNum}</b></color>\n\n";
                }
            }

            return true;
        }
    }
}
