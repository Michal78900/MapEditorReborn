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
            if (!TryGetPlayer(arguments, sender, out var player))
            {
                response = "анлак";
                return false;
            }

            if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject))
            {
                response = "You haven't selected any object!";
                return false;
            }

            if (mapObject is not SchematicObject schem)
            {
                response = "You can't modify this object!";
                return true;
            }

            if (schem.AttachedPlayer is not null)
            {
                SchematicUnfollow(schem);
                response = "Схематик отвязан!";
                return true;
            }

            SchematicFollow(schem, player);
            response = "Схематик привязан!";
            return true;
        }

        /// <summary>
        /// Получаем игрока
        /// </summary>
        private bool TryGetPlayer(ArraySegment<string> arguments, ICommandSender sender, out Player? player)
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