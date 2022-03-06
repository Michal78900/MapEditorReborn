namespace MapEditorReborn.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using API.Features.Objects;
    using API.Features.Serializable;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    using static API.API;

    /// <summary>
    /// Command used for modifing maps.
    /// </summary>
    public class Properties : ICommand
    {
        /// <inheritdoc/>
        public string Command => "properties";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "prop" };

        /// <inheritdoc/>
        public string Description => "Allows modifying properties of the selected map.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            MapSchematic map = CurrentLoadedMap;

            if (map == null)
            {
                response = $"You need to load a map before modifying it's properties!";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = $"\nName: <color=yellow><b>{map.Name}</b></color>\n";
                response += $"RemoveDefaultSpawnPoints: {(map.RemoveDefaultSpawnPoints ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}\n";
                response += $"RagdollRoleNames:\n";
                foreach (var list in map.RagdollRoleNames)
                {
                    response += $"  <color=yellow><b>{list.Key}</b></color>\n";
                    foreach (var name in list.Value)
                    {
                        response += $"  - <color=yellow>{name}</color>\n";
                    }
                }

                return true;
            }

            string variableName = arguments.At(0).ToLower();

            if ("name".Contains(variableName))
            {
                if (arguments.Count < 2)
                {
                    response = "You need to provide a new name for the map!";
                    return false;
                }

                string newName = arguments.At(1);

                if (File.Exists(Path.Combine(MapEditorReborn.MapsDir, $"{newName}.yml")))
                {
                    response = "Map with this name already exists!";
                    return false;
                }

                File.Move(Path.Combine(MapEditorReborn.MapsDir, $"{map.Name}.yml"), Path.Combine(MapEditorReborn.MapsDir, $"{newName}.yml"));
                map.Name = newName;

                response = $"Map has been renamed to \"{newName}\"!";
                return true;
            }

            if ("removedefaultspawnpoints".Contains(variableName))
            {
                if (arguments.Count < 2 || !bool.TryParse(arguments.ElementAt(1), out bool newValue))
                {
                    response = $"You need to provide a valid bool value!";
                    return false;
                }

                PlayerSpawnPointObject.VanillaSpawnPointsDisabled = newValue;
                map.RemoveDefaultSpawnPoints = newValue;

                response = "Default spawnpoints have been updated!";
                return true;
            }

            if ("ragdollrolenames".Contains(variableName))
            {
                if (arguments.Count < 3 || !Enum.TryParse(arguments.ElementAt(1), true, out RoleType roleType))
                {
                    response = $"You need to provide both valid role type and nickname!";
                    return false;
                }

                string nickname = arguments.At(2);
                for (int i = 1; i < arguments.Count - 2; i++)
                {
                    nickname += $" {arguments.At(2 + i)}";
                }

                response = string.Empty;

                if (!map.RagdollRoleNames.ContainsKey(roleType))
                {
                    response += $"{roleType} nickname list does not exists. Creating...\n";
                    map.RagdollRoleNames.Add(roleType, new List<string>());
                }

                if (map.RagdollRoleNames[roleType].Contains(nickname))
                {
                    map.RagdollRoleNames[roleType].Remove(nickname);
                    response += $"Successfully removed {nickname} nickname!";
                }
                else
                {
                    map.RagdollRoleNames[roleType].Add(nickname);
                    response += $"Successfully added {nickname} nickname!";
                }

                if (map.RagdollRoleNames[roleType].Count == 0)
                {
                    response += $"\n{roleType} nickname list is empty. Removing...";
                    map.RagdollRoleNames.Remove(roleType);
                }

                return true;
            }

            response = $"There isn't any map property that contains \"{arguments.At(0)}\" in it's name!";
            return false;
        }
    }
}
