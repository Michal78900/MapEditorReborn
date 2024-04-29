
namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using API.Features;
    using API.Features.Serializable;
    using CommandSystem;
    using Exiled.API.Features.Pools;
    using Exiled.Loader;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Merges two or more <see cref="MapSchematic"/>s into one.
    /// </summary>
    public class Merge : ICommand
    {
        /// <inheritdoc/>
        public string Command => "merge";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Сливает несколько карт в одну.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = "У вас недостаточно прав на выполнения этой команды.";
                return false;
            }

            if (arguments.Count < 3)
            {
                response = "\nПример использование:\n" +
                    "mp merge НазваниеИтоговойКарты Карта1 Карта2 [Карта3 ...]";

                return false;
            }

            List<MapSchematic> maps = ListPool<MapSchematic>.Pool.Get();

            for (int i = 1; i < arguments.Count; i++)
            {
                MapSchematic map = MapUtils.GetMapByName(arguments.At(i));

                if (map is null)
                {
                    response = $"Карты под названием {arguments.At(i)} не существует!";
                    return false;
                }

                maps.Add(map);
            }

            var outputMap = MapUtils.MergeMaps(arguments.At(0), maps);

            ListPool<MapSchematic>.Pool.Return(maps);

            File.WriteAllText(Path.Combine(MapEditorReborn.MapsDir, $"{outputMap.Name}.yml"), Loader.Serializer.Serialize(outputMap));

            response = $"Вы успешно совместили {arguments.Count - 1} в одну!";
            return true;
        }
    }
}
