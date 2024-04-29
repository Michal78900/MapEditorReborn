using System.Linq;

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
    internal class AttachSchematic : ICommand
    {
        /// <inheritdoc/>
        public string Command => "attachschematic";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "as" };

        /// <inheritdoc/>
        public string Description => "Привязывает или отвязывает схемат от игрока";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out var player))
            {
                response = "Произошла ошибка";
                return false;
            }

            if (!TryGetTarget(arguments, sender, out var target))
            {
                response = "Введены некорректные данные";
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

            if (SpawnedSchemats.Find(schematicobject => schematicobject.AttachedPlayer == target))
            {
                response = "На этой цели уже есть привязанный объект!";
                return false;
            }

            if (schem.AttachedPlayer is not null)
            {
                SchematicUnfollow(schem);
                response = "Схематик отвязан!";
                return true;
            }

            SchematicFollow(schem, target);
            response = "Схематик привязан!";
            return true;
        }

        /// <summary>
        /// Получаем игрока
        /// </summary>
        private bool TryGetTarget(ArraySegment<string> arguments, ICommandSender sender, out Player? player)
        {
            if (!arguments.Any() && Player.TryGet(sender, out player))
            {
                return true;
            }

            if (int.TryParse(arguments.At(0), out var id) && Player.TryGet(id, out player))
            {
                return true;
            }

            player = null;
            return false;
        }
    }
}