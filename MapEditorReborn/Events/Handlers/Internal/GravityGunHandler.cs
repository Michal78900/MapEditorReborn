// -----------------------------------------------------------------------
// <copyright file="GravityGunHandler.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using InventorySystem.Items;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.Exiled.Features.Items;
using MapEditorReborn.Factories;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

namespace MapEditorReborn.Events.Handlers.Internal;

using System.Collections.Generic;
using API.Features.Objects;
using InventorySystem.Items.Flashlight;
using MEC;
using UnityEngine;
using Utils.Networking;

using static API.API;

internal class GravityGunHandler
{
    [PluginEvent(ServerEventType.PlayerChangeItem)]
    internal void OnChangingItem(MERPlayer player, ushort oldItem, ushort newItem)
    {
        Log.Info(newItem.ToString());
        if (!GravityGuns.Contains(newItem))
            return;

        Timing.CallDelayed(0.01f, () =>
        {
            new FlashlightNetworkHandler.FlashlightMessage(newItem, false).SendToAuthenticated(0);
        });
    }

    [PluginEvent(ServerEventType.PlayerToggleFlashlight)]
    internal void OnTogglingFlashlight(MERPlayer player, ItemBase item, bool isToggled)
    {
        Log.Info(item.ItemSerial.ToString());
        if (!GravityGuns.Contains(item.ItemSerial))
            return;
        
        Timing.CallDelayed(0.25f, () =>
        {
            new FlashlightNetworkHandler.FlashlightMessage(item.ItemSerial, false).SendToAuthenticated(0);
        });
        
        if (grabbingPlayers.Contains(player))
        {
            grabbingPlayers.Remove(player);
            return;
        }
        
        Log.Info("Beg grab");
        Vector3 forward = player.Camera.forward;
        if (Physics.Raycast(player.Camera.position + forward, forward, out var hit, 5f))
        {
            Log.Info(hit.collider.ToString());
            Log.Info(hit.collider.transform.root.ToString());
            if (hit.collider.transform.root.TryGetComponent(out SchematicObject schematicObject) && schematicObject != null)
            {
                Log.Info(schematicObject.ToString());
                if (!schematicObject.gameObject.TryGetComponent(out Rigidbody rigidbody))
                {
                    rigidbody = schematicObject.gameObject.AddComponent<Rigidbody>();
                    rigidbody.mass = 1;
                }
        
                grabbingPlayers.Add(player);
                Timing.RunCoroutine(GravityGunMovementCoroutine(player, rigidbody));
            }
        }
    }

    private static IEnumerator<float> GravityGunMovementCoroutine(Player player, Rigidbody rigidbody)
    {
        Log.Info("Grabbing");
        rigidbody.isKinematic = true;
        rigidbody.transform.eulerAngles = Vector3.zero;
    
        while (grabbingPlayers.Contains(player) && player.CurrentItem.IsGravityGun())
        {
            yield return Timing.WaitForOneFrame;
            rigidbody.MovePosition(player.Camera.position + (player.Camera.forward * 2f));
        }
    
        rigidbody.isKinematic = false;
        grabbingPlayers.Remove(player);
    }

    private static List<Player> grabbingPlayers = new();
}