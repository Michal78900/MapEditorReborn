﻿// -----------------------------------------------------------------------
// <copyright file="TimeGrenadeProjectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


using InventorySystem.Items.ThrowableProjectiles;
using MapEditorReborn.Exiled.Interfaces;
using UnityEngine;

namespace MapEditorReborn.Exiled.Features.Pickups.Projectiles;

/// <summary>
/// A wrapper class for TimeGrenade.
/// </summary>
public class TimeGrenadeProjectile : Projectile, IWrapper<TimeGrenade>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeGrenadeProjectile"/> class.
    /// </summary>
    /// <param name="pickupBase">The base <see cref="TimeGrenade"/> class.</param>
    internal TimeGrenadeProjectile(TimeGrenade pickupBase)
        : base(pickupBase)
    {
        Base = pickupBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeGrenadeProjectile"/> class.
    /// </summary>
    /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
    internal TimeGrenadeProjectile(ItemType type)
        : base(type)
    {
        Base = (TimeGrenade)((Pickup)this).Base;
    }

    /// <summary>
    /// Gets the <see cref="TimeGrenade"/> that this class is encapsulating.
    /// </summary>
    public new TimeGrenade Base { get; }

    /// <summary>
    /// Gets a value indicating whether the grenade has already exploded.
    /// </summary>
    public bool IsAlreadyDetonated
    {
        get => Base._alreadyDetonated;
    }

    /// <summary>
    /// Gets or sets FuseTime.
    /// </summary>
    public float FuseTime
    {
        get => Base._fuseTime;
        set
        {
            if (IsActive)
                Base.RpcSetTime(value);
            else
                Base._fuseTime = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the greande is active.
    /// </summary>
    public bool IsActive
    {
        get => Base.TargetTime != 0.0f;
        set
        {
            if (value && Base.TargetTime == 0.0f)
                Base.RpcSetTime(FuseTime);
            else if (!value && Base.TargetTime != 0.0f)
                Base.RpcSetTime(-Time.timeSinceLevelLoad);
        }
    }

    /// <summary>
    /// Immediately exploding the <see cref="TimeGrenadeProjectile"/>.
    /// </summary>
    public void Explode()
    {
        Base.ServerFuseEnd();
        Base._alreadyDetonated = true;
    }

    /// <summary>
    /// Returns the TimeGrenadePickup in a human readable format.
    /// </summary>
    /// <returns>A string containing TimeGrenadePickup related data.</returns>
    public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}| ={IsAlreadyDetonated}=";
}