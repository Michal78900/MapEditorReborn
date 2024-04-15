using MapEditorReborn.API.Extensions;

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
    internal class Collide : ICommand
    {
        /// <inheritdoc/>
        public string Command => "collide";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

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

            foreach (var primitive in schem.AttachedBlocks)
            {
                primitive.transform.localScale = -primitive.transform.localScale;
            }

            player.ShowGameObjectHint(schem);
            schem.UpdateObject();

            response = "Изменения вошли в силу!";
            return true;
        }
    }
}