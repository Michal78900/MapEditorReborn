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

    public static class LockerExtensions
    {
        public static void SpawnItem(this LockerChamber lockerChamber, string item, uint amount)
        {
            if (Enum.TryParse(item, true, out ItemType parsedItem))
            {
                if (parsedItem == ItemType.None)
                    return;

                for (int i = 0; i < amount; i++)
                {
                    ItemPickupBase itemPickupBase = new Item(parsedItem).Spawn(lockerChamber._spawnpoint.position, lockerChamber._spawnpoint.rotation).Base;
                    NetworkServer.UnSpawn(itemPickupBase.gameObject);

                    itemPickupBase.transform.SetParent(lockerChamber._spawnpoint);
                    itemPickupBase.Info.Locked = true;
                    lockerChamber._content.Add(itemPickupBase);

                    (itemPickupBase as IPickupDistributorTrigger)?.OnDistributed();

                    if (lockerChamber._spawnOnFirstChamberOpening)
                    {
                        lockerChamber._toBeSpawned.Add(itemPickupBase);
                    }
                    else
                    {
                        ItemDistributor.SpawnPickup(itemPickupBase);
                    }
                }

                return;
            }

            if (CustomItem.TryGet(item, out CustomItem customItem))
            {
                for (int i = 0; i < amount; i++)
                {
                    ItemPickupBase itemPickupBase = customItem.Spawn(lockerChamber._spawnpoint.position).Base;
                    NetworkServer.UnSpawn(itemPickupBase.gameObject);

                    itemPickupBase.transform.SetParent(lockerChamber._spawnpoint);
                    itemPickupBase.transform.rotation = lockerChamber._spawnpoint.rotation;
                    itemPickupBase.Info.Locked = true;
                    lockerChamber._content.Add(itemPickupBase);

                    (itemPickupBase as IPickupDistributorTrigger)?.OnDistributed();

                    if (lockerChamber._spawnOnFirstChamberOpening)
                    {
                        lockerChamber._toBeSpawned.Add(itemPickupBase);
                    }
                    else
                    {
                        ItemDistributor.SpawnPickup(itemPickupBase);
                    }
                }
            }
        }

        public static LockerType GetLockerTypeByName(this Locker locker)
        {
            return locker.name switch
            {
                "Scp268PedestalStructure Variant(Clone)" => LockerType.Pedestal,
                "LargeGunLockerStructure(Clone)" => LockerType.LargeGun,
                "RifleRackStructure(Clone)" => LockerType.RifleRack,
                "MiscLocker(Clone)" => LockerType.Misc,
                "RegularMedkitStructure(Clone)" => LockerType.Medkit,
                _ => LockerType.Adrenaline,
            };
        }

        public static GameObject GetLockerObjectByType(this LockerType doorType)
        {
            return doorType switch
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
}
