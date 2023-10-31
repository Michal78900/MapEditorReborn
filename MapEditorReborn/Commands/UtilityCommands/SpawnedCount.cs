namespace MapEditorReborn.Commands.UtilityCommands;

using System;
using CommandSystem;
using API.Features.Objects;
using NorthwoodLib.Pools;

public class SpawnedCount : ICommand
{
    public string Command => "spawnedcount";

    public string[] Aliases { get; } = Array.Empty<string>();

    public string Description => "Количество заспавленых объектов";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var sB = StringBuilderPool.Shared.Rent();
        sB.AppendLine($"Заспавлено объектов всего - {API.API.SpawnedObjects.Count}");

        foreach (var mapEditorObject in API.API.SpawnedObjects)
        {
            if (mapEditorObject is SchematicObject schematicObject)
            {
                sB.AppendLine($"{schematicObject.Name} - {schematicObject.AttachedBlocks.Count}");
            }
        }

        response = StringBuilderPool.Shared.ToStringReturn(sB);
        return true;
    }
}