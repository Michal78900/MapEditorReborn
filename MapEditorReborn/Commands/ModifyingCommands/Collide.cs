using AdminToys;
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
        
        public bool SanitizeResponse => false;

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

            foreach (var admintoy in schem.AttachedBlocks)
            {
                if (!admintoy.TryGetComponent(out PrimitiveObject primitive))
                {
                    continue;
                }

                if (primitive.Primitive.Flags.HasFlag(PrimitiveFlags.Collidable))
                {
                    primitive.Base.PrimitiveFlags -= PrimitiveFlags.Collidable;
                }
                else
                {
                    primitive.Base.PrimitiveFlags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;
                }
            }

            player.ShowGameObjectHint(schem);
            schem.UpdateObject();

            response = "Изменения вошли в силу!";
            return true;
        }
    }
}