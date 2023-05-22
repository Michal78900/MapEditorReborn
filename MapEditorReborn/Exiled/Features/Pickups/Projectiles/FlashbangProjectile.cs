﻿// -----------------------------------------------------------------------
// <copyright file="FlashbangProjectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


using CustomPlayerEffects;
using InventorySystem.Items.ThrowableProjectiles;
using MapEditorReborn.Exiled.Interfaces;
using PluginAPI.Core.Zones;

namespace MapEditorReborn.Exiled.Features.Pickups.Projectiles;

/// <summary>
/// A wrapper class for FlashbangGrenade.
/// </summary>
public class FlashbangProjectile : EffectGrenadeProjectile, IWrapper<FlashbangGrenade>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlashbangProjectile"/> class.
    /// </summary>
    /// <param name="pickupBase">The base <see cref="FlashbangGrenade"/> class.</param>
    public FlashbangProjectile(FlashbangGrenade pickupBase)
        : base(pickupBase)
    {
        Base = pickupBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlashbangProjectile"/> class.
    /// </summary>
    internal FlashbangProjectile()
        : base(ItemType.GrenadeFlash)
    {
        Base = (FlashbangGrenade)((Pickup)this).Base;
    }

    /// <summary>
    /// Gets the <see cref="FlashbangGrenade"/> that this class is encapsulating.
    /// </summary>
    public new FlashbangGrenade Base { get; }

    /// <summary>
    /// Gets or sets the minimum duration of player can take the effect.
    /// </summary>
    public float MinimalDurationEffect
    {
        get => Base._minimalEffectDuration;
        set => Base._minimalEffectDuration = value;
    }

    /// <summary>
    /// Gets or sets the additional duration of the <see cref="Blinded"/> effect.
    /// </summary>
    public float AdditionalBlindedEffect
    {
        get => Base._additionalBlurDuration;
        set => Base._additionalBlurDuration = value;
    }

    /// <summary>
    /// Gets or sets the how mush the FlashBang going to be intensified when explode at <see cref="Surface"/>.
    /// </summary>
    public float SurfaceDistanceIntensifier
    {
        get => Base._surfaceZoneDistanceIntensifier;
        set => Base._surfaceZoneDistanceIntensifier = value;
    }

    /// <summary>
    /// Returns the FlashbangPickup in a human readable format.
    /// </summary>
    /// <returns>A string containing FlashbangPickup-related data.</returns>
    public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{IsLocked}- ={InUse}=";
}