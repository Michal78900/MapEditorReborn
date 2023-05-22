// -----------------------------------------------------------------------
// <copyright file="Radio.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using InventorySystem.Items.Radio;
using MapEditorReborn.Exiled.Interfaces;
using PluginAPI.Core;

namespace MapEditorReborn.Exiled.Features.Items;

/// <summary>
/// A wrapper class for <see cref="RadioItem"/>.
/// </summary>
public class Radio : Item, IWrapper<RadioItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Radio"/> class.
    /// </summary>
    /// <param name="itemBase">The base <see cref="RadioItem"/> class.</param>
    public Radio(RadioItem itemBase)
        : base(itemBase)
    {
        Base = itemBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Radio"/> class, as well as a new Radio item.
    /// </summary>
    internal Radio()
        : this((RadioItem)PluginAPI.Core.Server.Instance.ReferenceHub.inventory.CreateItemInstance(new(ItemType.Radio, 0), false))
    {
    }

    /// <summary>
    /// Gets the <see cref="RadioItem"/> that this class is encapsulating.
    /// </summary>
    public new RadioItem Base { get; }

    /// <summary>
    /// Gets or sets the percentage of the radio's battery, between <c>0-100</c>.
    /// </summary>
    public byte BatteryLevel
    {
        get => Base.BatteryPercent;
        set => Base.BatteryPercent = value;
    }

    /// <summary>
    /// Gets or sets the current <see cref="RadioRange"/>.
    /// </summary>
    public RadioMessages.RadioRangeLevel Range
    {
        get => (RadioMessages.RadioRangeLevel)Base._rangeId;
        set => Base._rangeId = (byte)value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the radio is enabled or not.
    /// </summary>
    public bool IsEnabled
    {
        get => Base._enabled;
        set => Base._enabled = value;
    }

    /// <summary>
    /// Returns the Radio in a human readable format.
    /// </summary>
    /// <returns>A string containing Radio-related data.</returns>
    public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Range}| -{BatteryLevel}-";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="oldOwner">old <see cref="Item"/> owner.</param>
    /// <param name="newOwner">new <see cref="Item"/> owner.</param>
    internal override void ChangeOwner(Player oldOwner, Player newOwner) => Base.Owner = newOwner.ReferenceHub;
}