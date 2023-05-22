﻿// -----------------------------------------------------------------------
// <copyright file="Scp018Projectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


using InventorySystem.Items.ThrowableProjectiles;
using MapEditorReborn.Exiled.Interfaces;
using BaseScp018Projectile = InventorySystem.Items.ThrowableProjectiles.Scp018Projectile;

namespace MapEditorReborn.Exiled.Features.Pickups.Projectiles;

/// <summary>
/// A wrapper class for Scp018Projectile.
/// </summary>
public class Scp018Projectile : ExplosionGrenadeProjectile, IWrapper<BaseScp018Projectile>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scp018Projectile"/> class.
    /// </summary>
    /// <param name="pickupBase">The base <see cref="BaseScp018Projectile"/> class.</param>
    public Scp018Projectile(BaseScp018Projectile pickupBase)
        : base(pickupBase)
    {
        Base = pickupBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Scp018Projectile"/> class.
    /// </summary>
    internal Scp018Projectile()
        : base(ItemType.SCP018)
    {
        Base = (BaseScp018Projectile)((Pickup)this).Base;
    }

    /// <summary>
    /// Gets the <see cref="ExplosionGrenade"/> that this class is encapsulating.
    /// </summary>
    public new BaseScp018Projectile Base { get; }

    /// <summary>
    /// Gets a value indicating whether or not SCP-018 can injure teammates.
    /// </summary>
    public bool IgnoreFriendlyFire
    {
        get => Base.IgnoreFriendlyFire;
    }

    /// <summary>
    /// Gets or sets the time for SCP-018 not to ignore the friendly fire.
    /// </summary>
    public float FriendlyFireTime
    {
        get => Base._friendlyFireTime;
        set => Base._friendlyFireTime = value;
    }

    /// <summary>
    /// Gets the current damage of SCP-018.
    /// </summary>
    public float Damage
    {
        get => Base.CurrentDamage;
    }

    /// <summary>
    /// Returns the Scp018Pickup in a human readable format.
    /// </summary>
    /// <returns>A string containing Scp018Pickup-related data.</returns>
    public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{Damage}- ={IgnoreFriendlyFire}=";
}