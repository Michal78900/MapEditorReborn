// -----------------------------------------------------------------------
// <copyright file="GravityGun.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using CommandSystem;
using InventorySystem.Items;
using MapEditorReborn.Exiled.Features.Items;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.UtilityCommands;

using System;
using static API.API;

public class GravityGun : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "gravitygun";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = { "gg", "gravgun" };

    /// <inheritdoc/>
    public string Description
    {
        get => "Gravity gun for picking up and throwing around schematics.";
    }

    /// <inheritdoc/>
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // if (!sender.CheckPermission($"mpr.{Command}"))
        // {
        //     response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
        //     return false;
        // }

        Log.Info(sender.LogName);
        var player = Player.Get<MERPlayer>(sender);
        Log.Info(player.Nickname);
        foreach (var item in player.Items.ToList().Where(item => GravityGuns.Contains(item.ItemSerial)))
        {
            GravityGuns.Remove(item.ItemSerial);
            player.RemoveItem(item.PickupDropModel);

            response = "You no longer have a Gravity Gun!";
            return true;
        }

        if (player.Items.Count >= 8)
        {
            response = "You have a full inventory!";
            return false;
        }

        var gravityGun = player.AddItem(ItemType.Flashlight);

        GravityGuns.Add(gravityGun.ItemSerial);

        response = "You now have the Gravity Gun!";
        return true;
    }
}