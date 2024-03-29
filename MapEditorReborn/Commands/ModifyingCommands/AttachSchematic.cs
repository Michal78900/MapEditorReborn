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
            var player = Player.Get(sender);
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
                return false;
            }

            if (schem.AttachedPlayer is not null)
            {
                SchematicUnfollow(schem.Name);
                response = "Схематик отвязан!";
                return false;
            }

            SchematicFollow(schem.Name, player);
            response = "Схематик привязан!";
            return false;
        }
    }
}