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
    using System.Linq;
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
            Locker = GetComponent<Locker>();
            StructurePositionSync = GetComponent<StructurePositionSync>();
        }

        public LockerObject Init(LockerSerializable lockerSerializable, bool first = false)
        {
            Base = lockerSerializable;
            Base.LockerType = Locker.GetLockerType();
            Locker.Loot = Array.Empty<LockerLoot>();

            if (first)
                Base.KeycardPermissions = Locker.Chambers[0].RequiredPermissions;

            HandleItems();
            NetworkServer.Spawn(gameObject);

            return this;
        }

        public override MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new LockerSerializable(block);
            Locker.Loot = Array.Empty<LockerLoot>();

            HandleItems();

            return this;
        }

        private void HandleItems()
        {
            foreach (LockerChamber lockerChamber in Locker.Chambers)
                lockerChamber.RequiredPermissions = Base.KeycardPermissions;

            Dictionary<int, List<LockerItemSerializable>> chambersCopy = null;
            if (Base.ShuffleChambers)
            {
                chambersCopy = new(Base.Chambers.Count);
                List<List<LockerItemSerializable>> chambersRandomValues = Base.Chambers.Values.OrderBy(x => Random.value).ToList();
                for (int i = 0; i < Base.Chambers.Count; i++)
                {
                    chambersCopy.Add(i, chambersRandomValues[i]);
                }
            }

            for (int i = 0; i < Locker.Chambers.Length; i++)
            {
                if (i == Base.Chambers.Count)
                    break;

                LockerItemSerializable chosenLoot = Choose(Base.ShuffleChambers ? chambersCopy[i] : Base.Chambers[i]);
                if (chosenLoot == null)
                    continue;

                Locker.Chambers[i].SpawnItem(chosenLoot.Item, chosenLoot.Count);
            }

            Locker.NetworkOpenedChambers = Base.OpenedChambers;
            _usedChambers = new(Locker.Chambers.Length);
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

        internal HashSet<LockerChamber> _usedChambers;

        private static LockerItemSerializable Choose(List<LockerItemSerializable> chambers)
        {
            if (chambers == null || chambers.Count == 0)
                return null;

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