using UnityEngine;

namespace MapEditorReborn.Commands.UtilityCommands;

using System;
using CommandSystem;
using API.Features.Objects;
using NorthwoodLib.Pools;

public class ObjectCount : ICommand
{
    public string Command => "objectcount";

    public string[] Aliases { get; } = { "oc" };

    public string Description => "Количество заспавленых игровых объектов";

    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        const string green = "#00fa9a";

        var sB = StringBuilderPool.Shared.Rent();
        sB.AppendLine($"Заспавлено объектов всего - {API.API.SpawnedObjects.Count}".ToColor(green));

        var cylinder = 0;
        var cube = 0;
        var quad = 0;
        var sphere = 0;
        var lightSource = 0;
        var plane = 0;
        var capsule = 0;
        var unknown = 0;
        var all = 0;

        foreach (var mapEditorObject in API.API.SpawnedObjects)
        {
            if (mapEditorObject is LightSourceObject)
            {
                lightSource++;
                all++;
                continue;
            }

            if (mapEditorObject is not SchematicObject schematicObject)
            {
                continue;
            }

            foreach (var gameObject in schematicObject.AttachedBlocks)
            {
                all++;
                switch (gameObject.GetComponent<PrimitiveType>())
                {
                    case PrimitiveType.Sphere:
                        sphere++;
                        continue;
                    case PrimitiveType.Capsule:
                        capsule++;
                        continue;
                    case PrimitiveType.Cylinder:
                        cylinder++;
                        continue;
                    case PrimitiveType.Cube:
                        cube++;
                        continue;
                    case PrimitiveType.Plane:
                        plane++;
                        continue;
                    case PrimitiveType.Quad:
                        quad++;
                        continue;
                    default:
                        unknown++;
                        continue;
                }
            }
        }

        sB.AppendLine($"Заспавнено примитивов всего - {all}".ToColor(green));
        sB.AppendLine($"Заспавнено кубов - {cube}".ToColor(green));
        sB.AppendLine($"Заспавнено сфер - {sphere}".ToColor(green));
        sB.AppendLine($"Заспавнено плейнов - {plane}".ToColor(green));
        sB.AppendLine($"Заспавнено квадов - {quad}".ToColor(green));
        sB.AppendLine($"Заспавнено капсул - {capsule}".ToColor(green));
        sB.AppendLine($"Заспавнено квадов - {cylinder}".ToColor(green));
        sB.AppendLine($"Заспавнено источников освещения - {lightSource}".ToColor(green));

        if (unknown != 0)
        {
            sB.AppendLine($"Заспавнено объектов, которые не прошли индетификацию - {unknown}".ToColor(green));
        }

        response = StringBuilderPool.Shared.ToStringReturn(sB);
        return true;
    }
}