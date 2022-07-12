// -----------------------------------------------------------------------
// <copyright file="LockerSerializable.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using Interactables.Interobjects.DoorUtils;

    public class LockerSerializable : SerializableObject
    {
        public LockerSerializable()
        {
        }

        public LockerSerializable(SchematicBlockData block)
        {
            LockerType = (LockerType)Enum.Parse(typeof(LockerType), block.Properties["LockerType"].ToString());
            Chambers = Utf8Json.JsonSerializer.Deserialize<Dictionary<int, List<LockerItemSerializable>>>(Utf8Json.JsonSerializer.Serialize(block.Properties["Chambers"]));
            AllowedRoleTypes = Utf8Json.JsonSerializer.Deserialize<List<string>>(Utf8Json.JsonSerializer.Serialize(block.Properties["AllowedRoleTypes"]));
            ShuffleChambers = bool.Parse(block.Properties["ShuffleChambers"].ToString());
            KeycardPermissions = (KeycardPermissions)Enum.Parse(typeof(KeycardPermissions), block.Properties["KeycardPermissions"].ToString());
            OpenedChambers = ushort.Parse(block.Properties["OpenedChambers"].ToString());
            Chance = float.Parse(block.Properties["Chance"].ToString());
        }

        public LockerType LockerType { get; set; }

        public Dictionary<int, List<LockerItemSerializable>> Chambers { get; set; } = new();

        public List<string> AllowedRoleTypes { get; set; } = new()
        {
            "Scp0492",
            "Scp049",
            "Scp096",
            "Scp106",
            "Scp173",
            "Scp93953",
            "Scp93989",
            "ClassD",
            "Scientist",
            "FacilityGuard",
            "NtfPrivate",
            "NtfSergeant",
            "NtfSpecialist",
            "NtfCaptain",
            "ChaosConscript",
            "ChaosRifleman",
            "ChaosRepressor",
            "ChaosMarauder",
            "Tutorial",
        };

        public bool ShuffleChambers { get; set; } = true;

        public KeycardPermissions KeycardPermissions { get; set; } = KeycardPermissions.None;

        public ushort OpenedChambers { get; set; } = 0;

        public float Chance { get; set; } = 100f;
    }
}
