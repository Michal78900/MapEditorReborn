namespace MapEditorReborn.API.Features.Objects
{
    using Extensions;
    using MapGeneration.Distributors;
    using Mirror;
    using Serializable;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    using static API;

    public class LockerObject : MapEditorObject
    {
        public LockerObject Init(LockerSerializable lockerSerializable)
        {
            Base = lockerSerializable;

            if (TryGetComponent(out Locker locker))
            {
                Locker = locker;
                Locker.Loot = System.Array.Empty<LockerLoot>();
            }

            Base.LockerType = Locker.GetLockerTypeByName();

            NetworkServer.Spawn(gameObject);

            List<LockerChamberSerializable> list = Base.Chambers.ToList();
            foreach (LockerChamber chamber in Locker.Chambers)
            {
                if (list.Count == 0)
                    break;

                LockerChamberSerializable choosedLoot = list[Random.Range(0, list.Count)];
                if (Random.Range(0, 101) > choosedLoot.Chance)
                {
                    list.Remove(choosedLoot);
                    continue;
                }

                chamber.SpawnItem(choosedLoot.Item, choosedLoot.Count);
                list.Remove(choosedLoot);
            }

            return this;
        }

        public LockerSerializable Base;

        public override void UpdateObject()
        {
            SpawnedObjects[SpawnedObjects.IndexOf(this)] = ObjectSpawner.SpawnLocker(Base, Position, Rotation, Scale);
            Destroy();
        }

        public Locker Locker { get; private set; }
    }
}
