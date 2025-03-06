// -----------------------------------------------------------------------
// <copyright file="GravityGun.cs" company="MapEditorReborn">
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
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Permissions.Extensions;
    using static API.API;

    public class GravityGun : ICommand
    {
        /// <inheritdoc/>
        public string Command => "gravitygun";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "gg", "gravgun" };

        /// <inheritdoc/>
        public string Description => "Gravity gun for picking up and throwing around schematics.";

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            Player player = Player.Get(sender);

            foreach (Item item in player.Items.ToList())
            {
                if (GravityGuns.ContainsKey(item.Serial))
                {
                    GravityGuns.Remove(item.Serial);
                    player.RemoveItem(item);

                    response = "You no longer have a Gravity Gun!";
                    return true;
                }
            }

            if (player.Items.Count >= 8)
            {
                response = "You have a full inventory!";
                return false;
            }

            Item gravityGun = player.AddItem(ItemType.GunRevolver);
            ((Firearm)gravityGun).MagazineAmmo = 0;
            GravityGuns.Add(gravityGun.Serial, GravityGunMode.Movement);

            response = "You now have the Gravity Gun!";
            return true;
        }
    }
}
