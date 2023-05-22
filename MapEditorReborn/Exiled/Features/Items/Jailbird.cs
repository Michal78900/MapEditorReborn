// -----------------------------------------------------------------------
// <copyright file="Jailbird.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using InventorySystem.Items.Jailbird;
using MapEditorReborn.Exiled.Interfaces;
using UnityEngine;

namespace MapEditorReborn.Exiled.Features.Items;

/// <summary>
/// A wrapped class for <see cref="JailbirdItem"/>.
/// </summary>
public class Jailbird : Item, IWrapper<JailbirdItem>
{
    /// <summary>
    /// Number of Charges use before the weapon become AlmostDepleted.
    /// </summary>
    public const int ChargesWarning = JailbirdItem.ChargesWarning;

    /// <summary>
    /// Number of Charges use before the weapon will being destroy.
    /// </summary>
    public const int ChargesLimit = JailbirdItem.ChargesLimit;

    /// <summary>
    /// Number of Damage made before the weapon become AlmostDepleted.
    /// </summary>
    public const float DamageWarning = JailbirdItem.DamageWarning;

    /// <summary>
    /// Number of Damage made before the weapon will being destroy.
    /// </summary>
    public const float DamageLimit = JailbirdItem.DamageLimit;

    /// <summary>
    /// Initializes a new instance of the <see cref="Jailbird"/> class.
    /// </summary>
    /// <param name="itemBase">The base <see cref="JailbirdItem"/> class.</param>
    public Jailbird(JailbirdItem itemBase)
        : base(itemBase)
    {
        Base = itemBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Jailbird"/> class, as well as a new Jailbird item.
    /// </summary>
    internal Jailbird()
        : this((JailbirdItem)PluginAPI.Core.Server.Instance.ReferenceHub.inventory.CreateItemInstance(new(ItemType.Jailbird, 0), false))
    {
    }

    /// <summary>
    /// Gets the <see cref="JailbirdItem"/> that this class is encapsulating.
    /// </summary>
    public new JailbirdItem Base { get; }

    /// <summary>
    /// Gets or sets the amount of damage dealt with a Jailbird melee hit.
    /// </summary>
    public float MeleeDamage
    {
        get => Base._hitreg._damageMelee;
        set => Base._hitreg._damageMelee = value;
    }

    /// <summary>
    /// Gets or sets the amount of damage dealt with a Jailbird charge hit.
    /// </summary>
    public float ChargeDamage
    {
        get => Base._hitreg._damageCharge;
        set => Base._hitreg._damageCharge = value;
    }

    /// <summary>
    /// Gets or sets the amount of time in seconds that the <see cref="CustomPlayerEffects.Flashed"/> effect will be applied on being hit.
    /// </summary>
    public float FlashDuration
    {
        get => Base._hitreg._flashDuration;
        set => Base._hitreg._flashDuration = value;
    }

    /// <summary>
    /// Gets or sets the radius of the Jailbird's hit register.
    /// </summary>
    public float Radius
    {
        get => Base._hitreg._hitregRadius;
        set => Base._hitreg._hitregRadius = value;
    }

    /// <summary>
    /// Gets or sets the total amount of damage dealt with the Jailbird.
    /// </summary>
    /// <seealso cref="RemainingDamage"/>
    public float TotalDamageDealt
    {
        get => Base._hitreg.TotalMeleeDamageDealt;
        set => Base._hitreg.TotalMeleeDamageDealt = value;
    }

    /// <summary>
    /// Gets or sets the amount of damage remaining before the Jailbird breaks.
    /// </summary>
    /// <remarks>Modifying this value will directly modify <see cref="TotalDamageDealt"/>.</remarks>
    /// <seealso cref="TotalDamageDealt"/>
    public float RemainingDamage
    {
        get => Mathf.Clamp(DamageLimit - TotalDamageDealt, int.MinValue, int.MaxValue);
        set => TotalDamageDealt = Mathf.Clamp(DamageLimit - value, float.MinValue, float.MaxValue);
    }

    /// <summary>
    /// Gets or sets the number of times the item has been charged and used.
    /// </summary>
    /// <seealso cref="RemainingCharges"/>
    public int TotalCharges
    {
        get => Base.TotalChargesPerformed;
        set => Base.TotalChargesPerformed = value;
    }

    /// <summary>
    /// Gets a value indicating whether the weapon warn the player than the Item will be broken.
    /// </summary>
    public bool IsAlmostDepleted
    {
        get => IsDamageWarning || IsChargesWarning;
    }

    /// <summary>
    /// Gets a value indicating whether .
    /// </summary>
    public bool IsDamageWarning
    {
        get => TotalDamageDealt >= DamageWarning;
    }

    /// <summary>
    /// Gets a value indicating whether .
    /// </summary>
    public bool IsChargesWarning
    {
        get => TotalCharges >= ChargesWarning;
    }

    /// <summary>
    /// Gets or sets the amount of charges remaining before the Jailbird breaks.
    /// </summary>
    /// <remarks>Modifying this value will directly modify <see cref="TotalCharges"/>.</remarks>
    /// <seealso cref="TotalCharges"/>
    public int RemainingCharges
    {
        get => Mathf.Clamp(ChargesLimit - TotalCharges, int.MinValue, int.MaxValue);
        set => TotalCharges = Mathf.Clamp(ChargesLimit - value, int.MinValue, int.MaxValue);
    }

    /// <summary>
    /// Breaks the Jailbird.
    /// </summary>
    public void Break()
    {
        Base._broken = true;
        Base.SendRpc(JailbirdMessageType.Broken);
    }

    /// <summary>
    /// Clones current <see cref="Jailbird"/> object.
    /// </summary>
    /// <returns> New <see cref="Jailbird"/> object. </returns>
    public override Item Clone() => new Jailbird()
    {
        MeleeDamage = MeleeDamage,
        ChargeDamage = ChargeDamage,
        TotalDamageDealt = TotalDamageDealt,
        TotalCharges = TotalCharges,
    };

    /// <summary>
    /// Returns the JailBird in a human readable format.
    /// </summary>
    /// <returns>A string containing JailBird-related data.</returns>
    public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";
}