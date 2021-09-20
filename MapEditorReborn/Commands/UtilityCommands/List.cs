namespace MapEditorReborn.Commands
{
    using System;
    using System.IO;
    using API;
    using CommandSystem;
    using Exiled.Loader;
    using Exiled.Permissions.Extensions;

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

            response = "\nList of maps:\n\n";

            foreach (string filePath in Directory.GetFiles(MapEditorReborn.PluginDir))
            {
                try
                {
                    MapSchematic map = Loader.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(filePath));

                    int doorsNum = map.Doors.Count;
                    int workstationsNum = map.WorkStations.Count;
                    int itemSpawnPointNum = map.ItemSpawnPoints.Count;
                    int playerSpawnPointsNum = map.PlayerSpawnPoints.Count;
                    int ragdollSpawnPointsNum = map.RagdollSpawnPoints.Count;
                    int shootingTargetsNum = map.ShootingTargetObjects.Count;
                    int lightControllersNum = map.LightControllerObjects.Count;

                    response += $"<color=yellow><b>{map.Name}</b></color>\n";
                    response += $"Doors: <color=yellow><b>{doorsNum}</b></color>\n";
                    response += $"Workstations: <color=yellow><b>{workstationsNum}</b></color>\n";
                    response += $"ItemSpawnPoints: <color=yellow><b>{itemSpawnPointNum}</b></color>\n";
                    response += $"PlayerSpawnPoints: <color=yellow><b>{playerSpawnPointsNum}</b></color>\n";
                    response += $"RagdollSpawnPoints: <color=yellow><b>{ragdollSpawnPointsNum}</b></color>\n";
                    response += $"ShootingTargets: <color=yellow><b>{shootingTargetsNum}</b></color>\n";
                    response += $"LightControllers: <color=yellow><b>{lightControllersNum}</b></color>\n";
                    response += $"Total number of objects: <color=yellow><b>{doorsNum + workstationsNum + itemSpawnPointNum + playerSpawnPointsNum + ragdollSpawnPointsNum + shootingTargetsNum + lightControllersNum}</b></color>\n\n";
                }
                catch (Exception)
                {
                    response += $"<color=red><b>{Path.GetFileNameWithoutExtension(filePath)}</b>\nTHIS MAP FILE IS CORRUPTED.</color>\n\n";
                }
            }

            return true;
        }
    }
}
