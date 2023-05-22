﻿// -----------------------------------------------------------------------
// <copyright file="Scp1576Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


using MapEditorReborn.Exiled.Interfaces;
using BaseScp1576 = InventorySystem.Items.Usables.Scp1576.Scp1576Pickup;

namespace MapEditorReborn.Exiled.Features.Pickups;

/// <summary>
/// A wrapper class for dropped SCP-330 bags.
/// </summary>
public class Scp1576Pickup : Pickup, IWrapper<BaseScp1576>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scp1576Pickup"/> class.
    /// </summary>
    /// <param name="pickupBase">The base <see cref="BaseScp1576"/> class.</param>
    internal Scp1576Pickup(BaseScp1576 pickupBase)
        : base(pickupBase)
    {
        Base = pickupBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Scp1576Pickup"/> class.
    /// </summary>
    internal Scp1576Pickup()
        : base(ItemType.SCP1576)
    {
        Base = (BaseScp1576)((Pickup)this).Base;
    }

    /// <summary>
    /// Gets the <see cref="BaseScp1576"/> that this class is encapsulating.
    /// </summary>
    public new BaseScp1576 Base { get; }

    /// <summary>
    /// Returns the Scp1576Pickup in a human readable format.
    /// </summary>
    /// <returns>A string containing Scp1576Pickup related data.</returns>
    public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";
}