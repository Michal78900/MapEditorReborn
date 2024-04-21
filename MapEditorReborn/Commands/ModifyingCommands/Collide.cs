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
                response = "You haven't selected any object!";
                return false;
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