// -----------------------------------------------------------------------
// <copyright file="LockerItemSerializable.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Serializable
{
    using System.Collections.Generic;
    using InventorySystem.Items.Firearms.Attachments;

    public class LockerItemSerializable
    {
        public LockerItemSerializable()
        {
        }

        public LockerItemSerializable(string item, uint count, List<AttachmentName> attachments, int chance)
        {
            Item = item;
            Count = count;
            Attachments = attachments;
            Chance = chance;
        }

        public string Item { get; set; } = "Coin";

        public uint Count { get; set; } = 1;

        public List<AttachmentName> Attachments { get; set; }

        public int Chance { get; set; } = 100;
    }
}
