// -----------------------------------------------------------------------
// <copyright file="LockerObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using MapGeneration.Distributors;
    using Mirror;
    using Serializable;
    using UnityEngine;

    using Random = UnityEngine.Random;

    public class LockerObject : MapEditorObject
    {
        private void Awake()
        {
            StructurePositionSync = GetComponent<StructurePositionSync>();
        }

        public LockerObject Init(LockerSerializable lockerSerializable, bool first = false)
        {
            Base = lockerSerializable;

            if (TryGetComponent(out Locker locker))
            {
                Locker = locker;
                Locker.Loot = Array.Empty<LockerLoot>();
                Base.LockerType = Locker.GetLockerType();
            }

            if (first)
                Base.KeycardPermissions = Locker.Chambers[0].RequiredPermissions;

            foreach (LockerChamber lockerChamber in Locker.Chambers)
                lockerChamber.RequiredPermissions = Base.KeycardPermissions;

            for (int i = 0; i < Locker.Chambers.Length; i++)
            {
                if (i > Base.Chambers.Count)
                    break;

                LockerItemSerializable chosenLoot = Choose(Base.Chambers[i]);
                Locker.Chambers[i].SpawnItem(chosenLoot.Item, chosenLoot.Count);
            }

            Locker.NetworkOpenedChambers = Base.OpenedChambers;
            NetworkServer.Spawn(gameObject);
            return this;
        }

        public override MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new LockerSerializable(block);

            if (TryGetComponent(out Locker locker))
            {
                Locker = locker;
                Locker.Loot = Array.Empty<LockerLoot>();
            }

            foreach (LockerChamber lockerChamber in Locker.Chambers)
                lockerChamber.RequiredPermissions = Base.KeycardPermissions;

            for (int i = 0; i < Locker.Chambers.Length; i++)
            {
                if (i > Base.Chambers.Count)
                    break;

                LockerItemSerializable chosenLoot = Choose(Base.Chambers[i]);
                Locker.Chambers[i].SpawnItem(chosenLoot.Item, chosenLoot.Count);
            }

            Locker.NetworkOpenedChambers = Base.OpenedChambers;
            return this;
        }

        public LockerSerializable Base;

        public Locker Locker { get; private set; }

        public StructurePositionSync StructurePositionSync { get; private set; }

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            StructurePositionSync.Network_position = Position;
            StructurePositionSync.Network_rotationY = (sbyte)Mathf.RoundToInt(transform.eulerAngles.y / 5.625f);
            base.UpdateObject();
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

                randomPoint -= chambers[i].Chance;
            }

            return chambers[chambers.Count - 1];
        }
    }
}