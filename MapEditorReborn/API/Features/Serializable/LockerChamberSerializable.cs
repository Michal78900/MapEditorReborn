// -----------------------------------------------------------------------
// <copyright file="LockerChamberSerializable.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Serializable
{
    public class LockerChamberSerializable
    {
        public LockerChamberSerializable()
        {
        }

        public LockerChamberSerializable(string item, uint count, int chance)
        {
            Item = item;
            Count = count;
            Chance = chance;
        }

        public string Item { get; set; } = "Coin";

        public uint Count { get; set; } = 1;

        public int Chance { get; set; } = 100;
    }
}
