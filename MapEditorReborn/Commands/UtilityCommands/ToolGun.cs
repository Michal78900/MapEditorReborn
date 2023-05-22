// -----------------------------------------------------------------------
// <copyright file="ToolGun.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using CommandSystem;
using InventorySystem;
using InventorySystem.Items;
using MapEditorReborn.Exiled.Features.Items;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.UtilityCommands;

using System;
using API.Enums;
using Events.EventArgs;
using static API.API;

/// <summary>
/// Command which gives a ToolGun to a sender.
/// </summary>
public class ToolGun : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "toolgun";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = { "tg" };

    /// <inheritdoc/>
    public string Description
    {
        get => "Tool gun for spawning and editing objects.";
    }

    /// <inheritdoc/>
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // if (!sender.CheckPermission($"mpr.{Command}"))
        // {
        //     response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
        //     return false;
        // }

        var player = Player.Get<MERPlayer>(sender);
        foreach (ItemBase item in player.Items.ToList())
        {
            if (ToolGuns.ContainsKey(item.ItemSerial))
            {
                DroppingToolGunEventArgs droppingEv = new(player, true);
                Events.Handlers.Utility.OnDroppingToolGun(droppingEv);

                if (!droppingEv.IsAllowed)
                {
                    response = droppingEv.Response;
                    return true;
                }

                ToolGuns.Remove(item.ItemSerial);
                player.RemoveItem(item.PickupDropModel);

                response = "You no longer have a Tool Gun!";
                return true;
            }
        }

        if (player.Items.Count >= 8)
        {
            response = "You have a full inventory!";
            return false;
        }

        PickingUpToolGunEventArgs ev = new(player, true);
        Events.Handlers.Utility.OnPickingUpToolGun(ev);

        if (!ev.IsAllowed)
        {
            response = ev.Response;
            return true;
        }

        Item toolgun = Item.Create(ItemType.GunCOM15);
        player.ReferenceHub.inventory.UserInventory.Items[toolgun.Serial] = toolgun.Base;
        var firearm = toolgun as Firearm;

        firearm.Base.Status = new InventorySystem.Items.Firearms.FirearmStatus((byte)(firearm.MaxAmmo + 1), (InventorySystem.Items.Firearms.FirearmStatusFlags)28, 77);

        ToolGuns.Add(toolgun.Serial, ObjectType.LczDoor);
        
        player.ReferenceHub.inventory.SendItemsNextFrame = true;

        response = "You now have the Tool Gun!";
        return true;
    }
}