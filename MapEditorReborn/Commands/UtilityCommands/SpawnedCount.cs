namespace MapEditorReborn.Commands.UtilityCommands;

using System;
using CommandSystem;

public class SpawnedCount : ICommand
{
    public string Command => "spawnedcount";

    public string[] Aliases { get; } = Array.Empty<string>();

    public string Description => "Количество заспавленых объектов";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = $"Заспавлено объектов - {API.API.SpawnedObjects.Count}";
        return true;
    }
}