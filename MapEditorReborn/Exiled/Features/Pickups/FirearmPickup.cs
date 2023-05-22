﻿// -----------------------------------------------------------------------
// <copyright file="FirearmPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


using InventorySystem.Items.Firearms;
using MapEditorReborn.Exiled.Interfaces;
using BaseFirearm = InventorySystem.Items.Firearms.FirearmPickup;

namespace MapEditorReborn.Exiled.Features.Pickups;

/// <summary>
/// A wrapper class for a Firearm pickup.
/// </summary>
public class FirearmPickup : Pickup, IWrapper<BaseFirearm>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
    /// </summary>
    /// <param name="pickupBase">The base <see cref="BaseFirearm"/> class.</param>
    internal FirearmPickup(BaseFirearm pickupBase)
        : base(pickupBase)
    {
        Base = pickupBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
    /// </summary>
    /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
    internal FirearmPickup(ItemType type)
        : base(type)
    {
        Base = (BaseFirearm)((Pickup)this).Base;
        IsDistributed = true;
    }

    /// <summary>
    /// Gets the <see cref="BaseFirearm"/> that this class is encapsulating.
    /// </summary>
    public new BaseFirearm Base { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the pickup is already distributed.
    /// </summary>
    public bool IsDistributed
    {
        get => Base.Distributed;
        set => Base.Distributed = value;
    }

    /// <summary>
    /// Gets or sets the <see cref="FirearmStatus"/>.
    /// </summary>
    public FirearmStatus Status
    {
        get => Base.NetworkStatus;
        set => Base.NetworkStatus = value;
    }

    /// <summary>
    /// Gets or sets a value indicating how many ammo have this <see cref="FirearmPickup"/>.
    /// </summary>
    public byte Ammo
    {
        get => Base.NetworkStatus.Ammo;
        set => Base.NetworkStatus = new(value, Base.NetworkStatus.Flags, Base.NetworkStatus.Attachments);
    }

    /// <summary>
    /// Gets or sets the <see cref="FirearmStatusFlags"/>.
    /// </summary>
    public FirearmStatusFlags Flags
    {
        get => Base.NetworkStatus.Flags;
        set => Base.NetworkStatus = new(Base.NetworkStatus.Ammo, value, Base.NetworkStatus.Attachments);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Attachement code have this <see cref="FirearmPickup"/>.
    /// </summary>
    public uint Attachments
    {
        get => Base.NetworkStatus.Attachments;
        set => Base.NetworkStatus = new(Base.NetworkStatus.Ammo, Base.NetworkStatus.Flags, value);
    }

    /// <summary>
    /// Returns the FirearmPickup in a human readable format.
    /// </summary>
    /// <returns>A string containing FirearmPickup related data.</returns>
    public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{IsDistributed}| -{Ammo}-";
}