using MapEditorReborn.API.Features.Objects;

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
        var attachedBlocks = 0;

        foreach (var mapEditorObject in API.API.SpawnedObjects)
        {
            if (mapEditorObject is SchematicObject schematicObject)
            {
                attachedBlocks += schematicObject.AttachedBlocks.Count;
            }
        }

        response = $"Заспавлено объектов - {API.API.SpawnedObjects.Count}. Дочерних блоков схематиков - {attachedBlocks}.";
        return true;
    }
}