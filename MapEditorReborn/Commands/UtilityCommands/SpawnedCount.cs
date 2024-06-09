namespace MapEditorReborn.Commands.UtilityCommands;

using System;
using CommandSystem;
using API.Features.Objects;
using NorthwoodLib.Pools;

public class SpawnedCount : ICommand
{
    public string Command => "spawnedcount";

    public string[] Aliases { get; } = { "sc" };

    public string Description => "Количество заспавленых объектов";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        const string green = "#00fa9a";

        var sB = StringBuilderPool.Shared.Rent();
        sB.AppendLine($"Заспавлено объектов всего - {API.API.SpawnedObjects.Count}".ToColor(green));

        var countBlock = 0;
        foreach (var mapEditorObject in API.API.SpawnedObjects)
        {
            if (mapEditorObject is not SchematicObject schematicObject)
            {
                continue;
            }

            sB.AppendLine($"{schematicObject.Name} - Количество примитивов: {schematicObject.AttachedBlocks.Count} - ID: {schematicObject.Id}");
            countBlock += schematicObject.AttachedBlocks.Count;
        }

        sB.AppendLine($"Заспавнено примитивов всего - {countBlock}".ToColor(green));

        response = StringBuilderPool.Shared.ToStringReturn(sB);
        return true;
    }
}