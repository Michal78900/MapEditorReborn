// -----------------------------------------------------------------------
// <copyright file="LockerExtensions.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Extensions
{
    using System;
    using Enums;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;
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

                for (int i = 0; i < amount; i++)
                {
                    ItemPickupBase itemPickupBase = Item.Create(parsedItem).Spawn(lockerChamber._spawnpoint.position, lockerChamber._spawnpoint.rotation).Base;
                    NetworkServer.UnSpawn(itemPickupBase.gameObject);

                    itemPickupBase.transform.SetParent(lockerChamber._spawnpoint);
                    itemPickupBase.Info.Locked = true;
                    lockerChamber._content.Add(itemPickupBase);

                    (itemPickupBase as IPickupDistributorTrigger)?.OnDistributed();

                    if (lockerChamber._spawnOnFirstChamberOpening)
                        lockerChamber._toBeSpawned.Add(itemPickupBase);
                    else
                        ItemDistributor.SpawnPickup(itemPickupBase);
                }

                return;
            }
            
            if (CustomItem.TryGet(item, out CustomItem customItem))
            {
                for (int i = 0; i < amount; i++)
                {
                    ItemPickupBase itemPickupBase = customItem.Spawn(lockerChamber._spawnpoint.position, (Exiled.API.Features.Player)null).Base;
                    NetworkServer.UnSpawn(itemPickupBase.gameObject);

                    itemPickupBase.transform.SetParent(lockerChamber._spawnpoint);
                    itemPickupBase.transform.rotation = lockerChamber._spawnpoint.rotation;
                    itemPickupBase.Info.Locked = true;
                    lockerChamber._content.Add(itemPickupBase);

                    (itemPickupBase as IPickupDistributorTrigger)?.OnDistributed();

                    if (lockerChamber._spawnOnFirstChamberOpening)
                        lockerChamber._toBeSpawned.Add(itemPickupBase);
                    else
                        ItemDistributor.SpawnPickup(itemPickupBase);
                }
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
        /// <param name="locker">The name to check.</param>
        /// <returns>The corresponding <see cref="LockerType"/>.</returns>
        public static LockerType GetLockerTypeByName(this string name) => name switch
        {
            "Scp268PedestalStructure Variant(Clone)" => LockerType.Pedestal,
            "LargeGunLockerStructure(Clone)" => LockerType.LargeGun,
            "RifleRackStructure(Clone)" => LockerType.RifleRack,
            "MiscLocker(Clone)" => LockerType.Misc,
            "RegularMedkitStructure(Clone)" => LockerType.Medkit,
            _ => LockerType.Adrenaline,
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
}
