// -----------------------------------------------------------------------
// <copyright file="LockerObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Extensions;
    using MapGeneration.Distributors;
    using Mirror;
    using Serializable;
    using UnityEngine;
    using static API;

    public class LockerObject : MapEditorObject
    {
        public LockerObject Init(LockerSerializable lockerSerializable, bool first = false)
        {
            Base = lockerSerializable;

            if (TryGetComponent(out Locker locker))
            {
                Locker = locker;
                Locker.Loot = System.Array.Empty<LockerLoot>();
                Base.LockerType = Locker.GetLockerType();
            }

            if (first)
                Base.KeycardPermissions = Locker.Chambers[0].RequiredPermissions;

            foreach (LockerChamber lockerChamber in Locker.Chambers)
                lockerChamber.RequiredPermissions = Base.KeycardPermissions;

            for (int i = 0; i < Locker.Chambers.Length; i++)
            {
                if (i >= Base.Chambers.Count)
                    break;

                LockerItemSerializable choosedLoot = Choose(Base.Chambers[i]);
                Locker.Chambers[i].SpawnItem(choosedLoot.Item, choosedLoot.Count);
            }

            NetworkServer.Spawn(gameObject);

            return this;
        }

        public LockerObject Init(SchematicBlockData block, bool first = false)
        {
            IsSchematicBlock = true;

            gameObject.name = block.Name;
            gameObject.transform.localPosition = block.Position;
            gameObject.transform.localEulerAngles = block.Rotation;
            gameObject.transform.localScale = block.Scale;

            Base = new LockerSerializable(block);

            if (TryGetComponent(out Locker locker))
            {
                Locker = locker;
                Locker.Loot = System.Array.Empty<LockerLoot>();
            }

            foreach (LockerChamber lockerChamber in Locker.Chambers)
                lockerChamber.RequiredPermissions = Base.KeycardPermissions;

            for (int i = 0; i < Locker.Chambers.Length; i++)
            {
                if (i >= Base.Chambers.Count)
                    break;

                LockerItemSerializable choosedLoot = Choose(Base.Chambers[i]);
                Locker.Chambers[i].SpawnItem(choosedLoot.Item, choosedLoot.Count);
            }

            return this;
        }

        public LockerSerializable Base;

        public Locker Locker { get; private set; }

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            MapEditorObject newLocker = ObjectSpawner.SpawnPropertyObject(Position, this);
            SpawnedObjects[SpawnedObjects.IndexOf(this)] = newLocker;

            if (prevOwner != null)
                Events.Handlers.Internal.ToolGunHandler.SelectObject(prevOwner, newLocker);

            Destroy(gameObject);
        }

        private static LockerItemSerializable Choose(List<LockerItemSerializable> chambers)
        {
            float total = 0;

            foreach (LockerItemSerializable elem in chambers)
            {
                total += elem.Chance;
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < chambers.Count; i++)
            {
                if (randomPoint < chambers[i].Chance)
                {
                    return chambers[i];
                }
                else
                {
                    randomPoint -= chambers[i].Chance;
                }
            }

            return chambers[chambers.Count - 1];
        }
    }
}