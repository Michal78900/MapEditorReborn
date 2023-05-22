// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Flashlight;
using InventorySystem.Items.Jailbird;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.Radio;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables;
using InventorySystem.Items.Usables.Scp1576;
using InventorySystem.Items.Usables.Scp244;
using MapEditorReborn.Exiled.Features.Core;
using MapEditorReborn.Exiled.Features.Pickups;
using MapEditorReborn.Exiled.Interfaces;
using PluginAPI.Core;
using UnityEngine;
using BaseConsumable = InventorySystem.Items.Usables.Consumable;
using Object = UnityEngine.Object;

namespace MapEditorReborn.Exiled.Features.Items;

/// <summary>
/// A wrapper class for <see cref="ItemBase"/>.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class Item : TypeCastObject<Item>, IWrapper<ItemBase>
{
    /// <summary>
    /// A dictionary of all <see cref="ItemBase"/>'s that have been converted into <see cref="Item"/>.
    /// </summary>
    internal static readonly Dictionary<ItemBase, Item> BaseToItem = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Item"/> class.
    /// </summary>
    /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
    public Item(ItemBase itemBase)
    {
        Base = itemBase;
        BaseToItem.Add(itemBase, this);

        if (Base.ItemSerial is 0 && itemBase.Owner != null)
        {
            var serial = ItemSerialGenerator.GenerateNext();
            Serial = serial;
            itemBase.OnAdded(null);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Item"/> class.
    /// </summary>
    /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
    internal Item(ItemType type)
        : this(PluginAPI.Core.Server.Instance.ReferenceHub.inventory.CreateItemInstance(new(type, 0), false))
    {
    }

    /// <summary>
    /// Gets a list of all <see cref="Item"/>'s on the server.
    /// </summary>
    public static IEnumerable<Item> List
    {
        get => BaseToItem.Values;
    }

    /// <summary>
    /// Gets or sets the unique serial number for the item.
    /// </summary>
    public ushort Serial
    {
        get
        {
            if (Base.ItemSerial is 0)
                return Serial = ItemSerialGenerator.GenerateNext();
            return Base.ItemSerial;
        }
        set => Base.ItemSerial = value;
    }
    
    /// <summary>
    /// Gets or sets the scale for the item.
    /// </summary>
    public Vector3 Scale { get; set; } = Vector3.one;

    /// <summary>
    /// Gets the <see cref="ItemBase"/> of the item.
    /// </summary>
    public ItemBase Base { get; }

    /// <summary>
    /// Gets the <see cref="ItemType"/> of the item.
    /// </summary>
    public ItemType Type
    {
        get => Base.ItemTypeId;
    }

    /// <summary>
    /// Gets the <see cref="ItemCategory"/> of the item.
    /// </summary>
    public ItemCategory Category
    {
        get => Base.Category;
    }

    /// <summary>
    /// Gets the <see cref="ItemTierFlags"/> of the item.
    /// </summary>
    public ItemTierFlags TierFlags
    {
        get => Base.TierFlags;
    }

    /// <summary>
    /// Gets the Weight of the item.
    /// </summary>
    public float Weight
    {
        get => Base.Weight;
    }

    /// <summary>
    /// Gets the <see cref="Player"/> who owns the item.
    /// </summary>
    public Player Owner
    {
        get => Player.Get(Base.Owner) ?? PluginAPI.Core.Server.Instance;
    }

    /// <summary>
    /// Gets an existing <see cref="Item"/> or creates a new instance of one.
    /// </summary>
    /// <param name="itemBase">The <see cref="ItemBase"/> to convert into an item.</param>
    /// <returns>The item wrapper for the given <see cref="ItemBase"/>.</returns>
    public static Item Get(ItemBase itemBase)
    {
        if (itemBase is null)
            return null;

        if (BaseToItem.TryGetValue(itemBase, out var item))
            return item;

        return itemBase switch
        {
            InventorySystem.Items.Firearms.Firearm firearm => new Firearm(firearm),
            KeycardItem keycard => new Keycard(keycard),
            UsableItem usable => usable switch
            {
                Scp244Item scp244Item => new Scp244(scp244Item),
                Scp1576Item scp1576 => new Scp1576(scp1576),
                BaseConsumable consumable => new Consumable(consumable),
                _ => new Usable(usable),
            },
            RadioItem radio => new Radio(radio),
            MicroHIDItem micro => new MicroHid(micro),
            BodyArmor armor => new Armor(armor),
            AmmoItem ammo => new Ammo(ammo),
            FlashlightItem flashlight => new Flashlight(flashlight),
            JailbirdItem jailbird => new Jailbird(jailbird),
            ThrowableItem throwable => throwable.Projectile switch
            {
                FlashbangGrenade => new FlashGrenade(throwable),
                ExplosionGrenade => new ExplosiveGrenade(throwable),
                _ => new Throwable(throwable),
            },
            _ => new Item(itemBase),
        };
    }

    /// <summary>
    /// Gets the Item belonging to the specified serial.
    /// </summary>
    /// <param name="serial">The Item serial.</param>
    /// <returns>Returns the Item found or <see langword="null"/> if not found.</returns>
    public static Item Get(ushort serial) => List.FirstOrDefault(x => x.Serial == serial);

    /// <summary>
    /// Creates a new <see cref="Item"/> with the proper inherited subclass.
    /// <para>
    /// Based on the <paramref name="type"/>, the returned <see cref="Item"/> can be casted into a subclass to gain more control over the object.
    /// <br />- Usable items (Adrenaline, Medkit, Painkillers, SCP-207, SCP-268, and SCP-500) should be casted to the <see cref="Usable"/> class.
    /// <br />- All valid ammo should be casted to the <see cref="Ammo"/> class.
    /// <br />- All valid firearms (not including the Micro HID) should be casted to the <see cref="Firearm"/> class.
    /// <br />- All valid keycards should be casted to the <see cref="Keycard"/> class.
    /// <br />- All valid armor should be casted to the <see cref="Armor"/> class.
    /// <br />- Explosive grenades and SCP-018 should be casted to the <see cref="ExplosiveGrenade"/> class.
    /// <br />- Flash grenades should be casted to the <see cref="FlashGrenade"/> class.
    /// </para>
    /// <para>
    /// <br />The following have their own respective classes:
    /// <br />- Flashlights can be casted to <see cref="Flashlight"/>.
    /// <br />- Radios can be casted to <see cref="Radio"/>.
    /// <br />- The Micro HID can be casted to <see cref="MicroHid"/>.
    /// <br />- SCP-244 A and B variants can be casted to <see cref="Scp244"/>.
    /// <br />- SCP-1576 can be casted to the <see cref="Scp1576"/> class.
    /// <br />- Jailbird can be casted to the <see cref="Jailbird"/> class.
    /// </para>
    /// <para>
    /// Items that are not listed above do not have a subclass, and can only use the base <see cref="Item"/> class.
    /// </para>
    /// </summary>
    /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
    /// <param name="owner">The <see cref="Player"/> who owns the item by default.</param>
    /// <returns>The <see cref="Item"/> created. This can be cast as a subclass.</returns>
    public static Item Create(ItemType type, Player owner = null) => type switch
    {
        ItemType.SCP268 => new Usable(type),
        ItemType.Adrenaline or ItemType.Medkit or ItemType.Painkillers or ItemType.SCP500 or ItemType.SCP207 or ItemType.SCP1853 => new Consumable(type),
        ItemType.SCP244a or ItemType.SCP244b => new Scp244(type),
        ItemType.Ammo9x19 or ItemType.Ammo12gauge or ItemType.Ammo44cal or ItemType.Ammo556x45 or ItemType.Ammo762x39 => new Ammo(type),
        ItemType.Flashlight => new Flashlight(),
        ItemType.Radio => new Radio(),
        ItemType.MicroHID => new MicroHid(),
        ItemType.GrenadeFlash => new FlashGrenade(owner),
        ItemType.GrenadeHE or ItemType.SCP018 => new ExplosiveGrenade(type, owner),
        ItemType.GunCrossvec or ItemType.GunLogicer or ItemType.GunRevolver or ItemType.GunShotgun or ItemType.GunAK or ItemType.GunCOM15 or ItemType.GunCOM18 or ItemType.GunCom45 or ItemType.GunE11SR or ItemType.GunFSP9 or ItemType.ParticleDisruptor => new Firearm(type),
        ItemType.KeycardGuard or ItemType.KeycardJanitor or ItemType.KeycardO5 or ItemType.KeycardScientist or ItemType.KeycardChaosInsurgency or ItemType.KeycardContainmentEngineer or ItemType.KeycardFacilityManager or ItemType.KeycardResearchCoordinator or ItemType.KeycardZoneManager or ItemType.KeycardNTFCommander or ItemType.KeycardNTFLieutenant or ItemType.KeycardNTFOfficer =>
            new Keycard(type),
        ItemType.ArmorLight or ItemType.ArmorCombat or ItemType.ArmorHeavy => new Armor(type),
        ItemType.SCP1576 => new Scp1576(),
        ItemType.Jailbird => new Jailbird(),
        _ => new Item(type),
    };
    
    /// <summary>
    /// Creates the <see cref="Pickup"/> that based on this <see cref="Item"/>.
    /// </summary>
    /// <param name="position">The location to spawn the item.</param>
    /// <param name="rotation">The rotation of the item.</param>
    /// <param name="spawn">Whether the <see cref="Pickup"/> should be initially spawned.</param>
    /// <returns>The created <see cref="Pickup"/>.</returns>
    public virtual Pickup CreatePickup(Vector3 position, Quaternion rotation = default, bool spawn = true)
    {
        var ipb = Object.Instantiate(Base.PickupDropModel, position, rotation);

        ipb.Info = new(Type, position, rotation, Weight, ItemSerialGenerator.GenerateNext());
        ipb.gameObject.transform.localScale = Scale;

        var pickup = Pickup.Get(ipb);

        if (spawn)
            pickup.Spawn();

        return pickup;
    }

    /// <summary>
    /// Clones the current item with a different serial.
    /// </summary>
    /// <returns> Cloned item object. </returns>
    public virtual Item Clone() => Create(Type);

    /// <summary>
    /// Returns the Item in a human readable format.
    /// </summary>
    /// <returns>A string containing Item-related data.</returns>
    public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* ={Owner}=";

    /// <summary>
    /// Change the owner of the <see cref="Item"/>.
    /// </summary>
    /// <param name="oldOwner">old <see cref="Item"/> owner.</param>
    /// <param name="newOwner">new <see cref="Item"/> owner.</param>
    internal virtual void ChangeOwner(Player oldOwner, Player newOwner)
    {
        Base.OnRemoved(null);

        Base.Owner = newOwner.ReferenceHub;

        Base.OnAdded(null);
    }
}