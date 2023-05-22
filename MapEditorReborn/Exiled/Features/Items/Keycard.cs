// -----------------------------------------------------------------------
// <copyright file="Keycard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using MapEditorReborn.Exiled.Interfaces;

namespace MapEditorReborn.Exiled.Features.Items;

/// <summary>
/// A wrapper class for <see cref="KeycardItem"/>.
/// </summary>
public class Keycard : Item, IWrapper<KeycardItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Keycard"/> class.
    /// </summary>
    /// <param name="itemBase">The base <see cref="KeycardItem"/> class.</param>
    public Keycard(KeycardItem itemBase)
        : base(itemBase)
    {
        Base = itemBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Keycard"/> class.
    /// </summary>
    /// <param name="type">The <see cref="ItemType"/> of the keycard.</param>
    internal Keycard(ItemType type)
        : this((KeycardItem)PluginAPI.Core.Server.Instance.ReferenceHub.inventory.CreateItemInstance(new(type, 0), false))
    {
    }

    /// <summary>
    /// Gets the <see cref="KeycardItem"/> that this class is encapsulating.
    /// </summary>
    public new KeycardItem Base { get; }

    /// <summary>
    /// Gets or sets the <see cref="KeycardPermissions"/> of the keycard.
    /// </summary>
    public KeycardPermissions Permissions
    {
        get => (KeycardPermissions)Base.Permissions;
        set => Base.Permissions = (Interactables.Interobjects.DoorUtils.KeycardPermissions)value;
    }

    /// <summary>
    /// Clones current <see cref="Keycard"/> object.
    /// </summary>
    /// <returns> New <see cref="Keycard"/> object. </returns>
    public override Item Clone() => new Keycard(Type)
    {
        Permissions = Permissions,
    };

    /// <summary>
    /// Returns the Keycard in a human readable format.
    /// </summary>
    /// <returns>A string containing Keycard-related data.</returns>
    public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Permissions}|";
}