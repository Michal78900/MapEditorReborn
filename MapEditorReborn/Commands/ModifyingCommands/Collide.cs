using System.Linq;
using MapEditorReborn.API.Extensions;

namespace MapEditorReborn.Commands.ModifyingCommands
{
    using System;
    using API.Features.Objects;
    using CommandSystem;
    using Exiled.API.Features;
    using static API.API;

    /// <summary>
    /// Команда для привязке к игроку схемата.
    /// </summary>
    internal class Collide : ICommand
    {
        /// <inheritdoc/>
        public string Command => "collide";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Отключает или включает коллайд у объекта";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out var player))
            {
                response = "Не смог получить игрока!";
                return false;
            }

            if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject))
            {
                response = "Вы не выбрали объект!";
                return false;
            }

            if (mapObject is not SchematicObject schem)
            {
                response = "Вы не можете модифицировать этот объект!";
                return false;
            }

            if (!arguments.Any())
            {
                response = "Аргументы не введены! Доступные аргументы: on/off";
                return false;
            }

            foreach (var admintoy in schem.AdminToyBases)
            {
                if (!admintoy.TryGetComponent(out PrimitiveObject primitive))
                {
                    continue;
                }

                switch (arguments.At(0))
                {
                    case "off":
                        primitive.Primitive.Collidable = false;
                        break;
                    case "on":
                        primitive.Primitive.Collidable = true;
                        break;
                    default:
                        response = "Введены не правильные аргументы! Доступные аргументы: on/off";
                        return false;
                }
            }

            player.ShowGameObjectHint(schem);
            schem.UpdateObject();

            response = "Изменения вошли в силу!";
            return true;
        }
    }
}