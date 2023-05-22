// -----------------------------------------------------------------------
// <copyright file="Scp1576.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using InventorySystem.Items.Usables;
using InventorySystem.Items.Usables.Scp1576;
using MapEditorReborn.Exiled.Interfaces;

namespace MapEditorReborn.Exiled.Features.Items;

/// <summary>
/// A wrapper class for <see cref="Scp1576Item"/>.
/// </summary>
public class Scp1576 : Usable, IWrapper<Scp1576Item>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scp1576"/> class.
    /// </summary>
    /// <param name="itemBase">The base <see cref="Scp1576Item"/> class.</param>
    public Scp1576(Scp1576Item itemBase)
        : base(itemBase)
    {
        Base = itemBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Scp1576"/> class.
    /// </summary>
    internal Scp1576()
        : this((Scp1576Item)PluginAPI.Core.Server.Instance.ReferenceHub.inventory.CreateItemInstance(new(ItemType.SCP1576, 0), false))
    {
    }

    /// <summary>
    /// Gets the <see cref="UsableItem"/> that this class is encapsulating.
    /// </summary>
    public new Scp1576Item Base { get; }

    /// <summary>
    /// Gets Scp1576Playback.
    /// </summary>
    public Scp1576Playback PlaybackTemplate
    {
        get => Base.PlaybackTemplate;
    }

    /// <summary>
    /// Forcefully stops the transmission of SCP-1576.
    /// </summary>
    public void StopTransmitting() => Base.ServerStopTransmitting();
}