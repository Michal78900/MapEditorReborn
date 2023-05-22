// -----------------------------------------------------------------------
// <copyright file="LockerExtensions.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using PluginAPI.Core.Items;

namespace MapEditorReborn.API.Extensions;

using System;
using Enums;
using InventorySystem.Items.Pickups;
using MapGeneration.Distributors;
using Mirror;
using UnityEngine;

/// <summary>
/// A set of useful extensions to easily interact with in-game lockers.
/// </summary>
public static class LockerExtensions
{
    /// <summary>
    /// Spawns an item inside the locker.
    /// </summary>
    /// <param name="lockerChamber">The <see cref="LockerChamber"/> into which spawn the item.</param>
    /// <param name="item">The name of the item.</param>
    /// <param name="amount">The amount of items to spawn.</param>
    public static void SpawnItem(this LockerChamber lockerChamber, string item, uint amount)
    {
        if (Enum.TryParse(item, true, out ItemType parsedItem))
        {
            if (parsedItem == ItemType.None)
                return;

            for (var i = 0; i < amount; i++)
            {
                var itemPickup = ItemPickup.Create(parsedItem, lockerChamber._spawnpoint.position, lockerChamber._spawnpoint.rotation);
                itemPickup.Spawn();
                
                NetworkServer.UnSpawn(itemPickup.GameObject);

                itemPickup.Transform.SetParent(lockerChamber._spawnpoint);
                itemPickup.IsLocked = true;
                lockerChamber._content.Add(itemPickup.OriginalObject);

                (itemPickup.OriginalObject as IPickupDistributorTrigger)?.OnDistributed();

                if (lockerChamber._spawnOnFirstChamberOpening)
                    lockerChamber._toBeSpawned.Add(itemPickup.OriginalObject);
                else
                    ItemDistributor.SpawnPickup(itemPickup.OriginalObject);
            }

            return;
        }
    }

    /// <summary>
    /// Gets the <see cref="LockerType"/> from the given <see cref="Locker"/> object.
    /// </summary>
    /// <param name="locker">The <see cref="Locker"/> to check.</param>
    /// <returns>The corresponding <see cref="LockerType"/>.</returns>
    public static LockerType GetLockerType(this Locker locker) => locker.name.GetLockerTypeByName();

    /// <summary>
    /// Gets the <see cref="LockerType"/> by name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>The corresponding <see cref="LockerType"/>.</returns>
    public static LockerType GetLockerTypeByName(this string name) => name.Replace("(Clone)", string.Empty) switch
    {
        "Scp500PedestalStructure Variant" => LockerType.Pedestal,
        "LargeGunLockerStructure" => LockerType.LargeGun,
        "RifleRackStructure" => LockerType.RifleRack,
        "MiscLocker" => LockerType.Misc,
        "RegularMedkitStructure" => LockerType.Medkit,
        "AdrenalineMedkitStructure" => LockerType.Adrenaline,
        _ => throw new NotImplementedException($"Couldn't resolve locker type for {name}. Report that to the developer."),
    };

    public static GameObject GetLockerObjectByType(this LockerType doorType) => doorType switch
    {
        LockerType.Pedestal => ObjectType.PedestalLocker.GetObjectByMode(),
        LockerType.LargeGun => ObjectType.LargeGunLocker.GetObjectByMode(),
        LockerType.RifleRack => ObjectType.RifleRackLocker.GetObjectByMode(),
        LockerType.Misc => ObjectType.MiscLocker.GetObjectByMode(),
        LockerType.Medkit => ObjectType.MedkitLocker.GetObjectByMode(),
        LockerType.Adrenaline => ObjectType.AdrenalineLocker.GetObjectByMode(),
        _ => null,
    };
}