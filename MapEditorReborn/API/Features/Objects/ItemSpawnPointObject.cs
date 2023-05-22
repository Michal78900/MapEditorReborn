// -----------------------------------------------------------------------
// <copyright file="ItemSpawnPointObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using MapEditorReborn.Exiled.Features.Items;
using MapEditorReborn.Exiled.Features.Pickups;

namespace MapEditorReborn.API.Features.Objects;

using System;
using System.Collections.Generic;
using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using Serializable;
using UnityEngine;

using static API;

using Random = UnityEngine.Random;

/// <summary>
/// Component added to spawned ItemSpawnPoint. Is is used for easier idendification of the object and it's variables.
/// </summary>
public class ItemSpawnPointObject : MapEditorObject
{
    /// <summary>
    /// Gets or sets a <see cref="List{T}"/> of <see cref="Pickup"/> which contains all attached pickups.
    /// </summary>
    public List<Pickup> AttachedPickups { get; set; } = new();

    /// <summary>
    /// Initializes the <see cref="ItemSpawnPointObject"/>.
    /// </summary>
    /// <param name="itemSpawnPointSerializable">The <see cref="ItemSpawnPointSerializable"/> to initialize.</param>
    /// <returns>Instance of this component.</returns>
    public ItemSpawnPointObject Init(ItemSpawnPointSerializable itemSpawnPointSerializable)
    {
        Base = itemSpawnPointSerializable;

        ForcedRoomType = itemSpawnPointSerializable.RoomType != Exiled.Enums.RoomType.Unknown ? itemSpawnPointSerializable.RoomType : FindRoom().Type;
        UpdateObject();

        return this;
    }

    /// <summary>
    /// The config-base of the object containing all of it's properties.
    /// </summary>
    public ItemSpawnPointSerializable Base;

    /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
    public override void UpdateObject()
    {
        foreach (var pickup in AttachedPickups)
        {
            if (pickup.Base != null)
                NetworkServer.Destroy(pickup.Base.gameObject);
        }

        if (Random.Range(0, 101) > Base.SpawnChance)
            return;

        if (Enum.TryParse(Base.Item, out ItemType parsedItem))
        {
            for (var i = 0; i < Base.NumberOfItems; i++)
            {
                var item = Item.Create(parsedItem);
                Pickup pickup = item.CreatePickup(transform.position, transform.rotation);
                pickup.Spawn();
                pickup.Base.transform.parent = transform;

                if (!Base.UseGravity && pickup.Base.gameObject.TryGetComponent(out Rigidbody rb))
                    rb.isKinematic = true;

                if (!Base.CanBePickedUp)
                    PickupsLocked.Add(pickup.Serial);

                if (pickup.Base is InventorySystem.Items.Firearms.FirearmPickup firearmPickup)
                {
                    var rawCode = GetAttachmentsCode(Base.AttachmentsCode);
                    var code = rawCode != -1 ? (item.Base as InventorySystem.Items.Firearms.Firearm).ValidateAttachmentsCode((uint)rawCode) : AttachmentsUtils.GetRandomAttachmentsCode(parsedItem);

                    firearmPickup.NetworkStatus = new InventorySystem.Items.Firearms.FirearmStatus(firearmPickup.NetworkStatus.Ammo, firearmPickup.NetworkStatus.Flags, code);
                }

                pickup.Scale = transform.localScale;

                AttachedPickups.Add(pickup);
            }
        }
    }

    private static int GetAttachmentsCode(string attachmentsString)
    {
        if (attachmentsString == "-1")
            return -1;

        var attachementsCode = 0;

        if (attachmentsString.Contains("+"))
        {
            var array = attachmentsString.Split(new[] { '+' });

            for (var j = 0; j < array.Length; j++)
            {
                if (int.TryParse(array[j], out var num))
                {
                    attachementsCode += num;
                }
            }
        }
        else
        {
            attachementsCode = int.Parse(attachmentsString);
        }

        return attachementsCode;
    }
}