﻿// -----------------------------------------------------------------------
// <copyright file="RadioPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


using InventorySystem.Items.Radio;
using MapEditorReborn.Exiled.Interfaces;
using BaseRadio = InventorySystem.Items.Radio.RadioPickup;

namespace MapEditorReborn.Exiled.Features.Pickups;

/// <summary>
/// A wrapper class for a Radio pickup.
/// </summary>
public class RadioPickup : Pickup, IWrapper<BaseRadio>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RadioPickup"/> class.
    /// </summary>
    /// <param name="pickupBase">The base <see cref="BaseRadio"/> class.</param>
    internal RadioPickup(BaseRadio pickupBase)
        : base(pickupBase)
    {
        Base = pickupBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RadioPickup"/> class.
    /// </summary>
    internal RadioPickup()
        : base(ItemType.Radio)
    {
        Base = (BaseRadio)((Pickup)this).Base;
    }

    /// <summary>
    /// Gets the <see cref="BaseRadio"/> that this class is encapsulating.
    /// </summary>
    public new BaseRadio Base { get; }

    /// <summary>
    /// Gets or sets the Radio Energy.
    /// </summary>
    public float BatteryLevel
    {
        get => Base.SavedBattery;
        set => Base.SavedBattery = value;
    }

    /// <summary>
    /// Gets or sets the <see cref="RadioMessages.RadioRangeLevel"/>.
    /// </summary>
    public RadioMessages.RadioRangeLevel Range
    {
        get => (RadioMessages.RadioRangeLevel)Base.NetworkSavedRange;
        set => Base.NetworkSavedRange = (byte)value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the radio is active.
    /// </summary>
    public bool IsEnabled
    {
        get => Base.NetworkSavedEnabled;
        set => Base.NetworkSavedEnabled = value;
    }

    /// <summary>
    /// Returns the RadioPickup in a human readable format.
    /// </summary>
    /// <returns>A string containing RadioPickup related data.</returns>
    public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{BatteryLevel}| -{Range}- /{IsEnabled}/";
}