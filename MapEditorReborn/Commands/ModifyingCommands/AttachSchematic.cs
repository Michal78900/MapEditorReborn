using System.Linq;

namespace MapEditorReborn.Commands.ModifyingCommands
{
    using System;
    using API.Features.Objects;
    using CommandSystem;
    using Events.Handlers.Internal;
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
        public string Description => string.Empty;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;
            if (arguments.At(0).IsEmpty())
            {
                if (Player.TryGet(sender, out player))
                {
                    response = "Невозможно определить цель!";
                    return false;
                }
            }
            else
            {
                var id = int.Parse(arguments.At(0));
                player = Player.List.First(p => p.Id == id);
            }

            if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
            {
                if (!ToolGunHandler.TryGetMapObject(player, out mapObject))
                {
                    response = "You haven't selected any object!";
                    return false;
                }

                ToolGunHandler.SelectObject(player, mapObject);
            }

            if (mapObject is not SchematicObject schem)
            {
                response = "You can't modify this object!";
                return true;
            }

            if (schem.AttachedPlayer is not null && schem.AttachedPlayer != player)
            {
                response = "Схематик уже привязан к другому игроку!";
                return false;
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
    }
}