﻿// -----------------------------------------------------------------------
// <copyright file="ToolGun.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using System.Linq;
    using API.Enums;
    using CommandSystem;
    using Events.EventArgs;
    using Events.Handlers;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Permissions.Extensions;
    using InventorySystem.Items.Firearms;
    using static API.API;
    using Firearm = Exiled.API.Features.Items.Firearm;

    /// <summary>
    /// Command which gives a ToolGun to a sender.
    /// </summary>
    public class ToolGun : ICommand
    {
        /// <inheritdoc/>
        public string Command => "toolgun";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "tg" };

        /// <inheritdoc/>
        public string Description => "Выдаёт тулган - инструмент для создания и редактирования объектов.";

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

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
                if (ToolGuns.ContainsKey(item.Serial))
                {
                    DroppingToolGunEventArgs droppingEv = new(player);
                    Utility.OnDroppingToolGun(droppingEv);

                    if (!droppingEv.IsAllowed)
                    {
                        response = droppingEv.Response;
                        return true;
                    }

                    ToolGuns.Remove(item.Serial);
                    player.RemoveItem(item);

                    response = "У вас больше нет тулгана!";
                    return true;
                }
            }

            if (player.Items.Count >= 8)
            {
                response = "У вас полный инвентарь!";
                return false;
            }

            PickingUpToolGunEventArgs ev = new(player);
            Utility.OnPickingUpToolGun(ev);

            if (!ev.IsAllowed)
            {
                response = ev.Response;
                return true;
            }

            Item toolgun = player.AddItem(ItemType.GunCOM15);
            Firearm firearm = toolgun as Firearm;

            firearm.Base.Status = new FirearmStatus((byte)(firearm.MaxAmmo + 1), (FirearmStatusFlags)28, 77);

            ToolGuns.Add(toolgun.Serial, ObjectType.LczDoor);

            response = "Вы получили тулган!";
            return true;
        }
    }
}
