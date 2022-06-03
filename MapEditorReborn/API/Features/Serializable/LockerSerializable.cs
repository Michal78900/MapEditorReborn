// -----------------------------------------------------------------------
// <copyright file="LockerSerializable.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Serializable
{
    using System.Collections.Generic;
    using Enums;
    using Interactables.Interobjects.DoorUtils;

    public class LockerSerializable : SerializableObject
    {
        public LockerType LockerType { get; set; }

        public KeycardPermissions KeycardPermissions { get; set; }

        public Dictionary<int, List<LockerChamberSerializable>> Chambers { get; set; } = new ()
        {
            { 0, new () { new () } },
        };
    }
}
