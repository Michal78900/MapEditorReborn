namespace MapEditorReborn.API.Extensions
{
    using System;
    using Exiled.API.Features.Items;
    using InventorySystem.Items.Pickups;
    using Enums;
    using MapGeneration;
    using MapGeneration.Distributors;
    using UnityEngine;

    using Object = UnityEngine.Object;

    public static class LockerExtensions
    {
        public static void SpawnItem(this LockerChamber lockerChamber, string item, uint amount)
        {
            if (Enum.TryParse(item, true, out ItemType parsedItem))
            {
                for (int i = 0; i < amount; i++)
                {
                    ItemPickupBase itemPickupBase = new Item(parsedItem).Spawn(lockerChamber._spawnpoint.position, lockerChamber._spawnpoint.rotation).Base;

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
                        // ItemDistributor.SpawnPickup(itemPickupBase);
                        InitiallySpawnedItems.Singleton.AddInitial(itemPickupBase.NetworkInfo.Serial);
                    }
                }
            }
        }

        public static LockerType GetLockerTypeByName(this Locker locker)
        {
            switch (locker.name)
            {
                case "Scp268PedestalStructure Variant(Clone)":
                    return LockerType.Pedestal;

                case "LargeGunLockerStructure(Clone)":
                    return LockerType.LargeGun;

                case "RifleRackStructure(Clone)":
                    return LockerType.RifleRack;

                case "MiscLocker(Clone)":
                    return LockerType.Misc;

                case "RegularMedkitStructure(Clone)":
                    return LockerType.Medkit;

                default:
                    return LockerType.Adrenaline;
            }
        }

        public static GameObject GetLockerObjectByType(this LockerType doorType)
        {
            switch (doorType)
            {
                case LockerType.Pedestal:
                    return ObjectType.PedestalLocker.GetObjectByMode();

                case LockerType.LargeGun:
                    return ObjectType.LargeGunLocker.GetObjectByMode();

                case LockerType.RifleRack:
                    return ObjectType.RifleRackLocker.GetObjectByMode();

                case LockerType.Misc:
                    return ObjectType.MiscLocker.GetObjectByMode();

                case LockerType.Medkit:
                    return ObjectType.MedkitLocker.GetObjectByMode();

                case LockerType.Adrenaline:
                    return ObjectType.AdrenalineLocker.GetObjectByMode();

                default:
                    return null;
            }
        }

    }
}
