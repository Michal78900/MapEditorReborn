// -----------------------------------------------------------------------
// <copyright file="GravityGun.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using API.Enums;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Structs;
    using Exiled.Permissions.Extensions;
    using static API.API;

    public class GravityGun : ICommand
    {
        /// <inheritdoc/>
        public string Command => "gravitygun";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "gg", "gravgun" };

        /// <inheritdoc/>
        public string Description => "Выдаёт гравити-пушку - инструмент для перемещения и поворота объектов.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = "У вас недостаточно прав на выполнения этой команды.";
                return false;
            }

            Player player = Player.Get(sender);

            foreach (Item item in player.Items.ToList())
            {
                if (GravityGuns.ContainsKey(item.Serial))
                {
                    GravityGuns.Remove(item.Serial);
                    player.RemoveItem(item);

                    response = "У вас больше нет гравити-пушки!";
                    return true;
                }
            }

            if (player.Items.Count >= 8)
            {
                response = "У вас полный инвентарь!";
                return false;
            }

            Item gravityGun = player.AddItem(ItemType.GunRevolver);
            ((Firearm)gravityGun).Ammo = 0;

            GravityGuns.Add(gravityGun.Serial, GravityGunMode.Movement);

            response = "Теперь у вас есть гравити-пушка!";
            return true;
        }
    }
}