﻿// -----------------------------------------------------------------------
// <copyright file="Door.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapEditorReborn.Exiled.Enums;
using MapEditorReborn.Exiled.Extensions;
using MapEditorReborn.Exiled.Interfaces;
using MEC;
using Mirror;
using PluginAPI.Core;
using UnityEngine;

namespace MapEditorReborn.Exiled.Features;

using static Interactables.Interobjects.ElevatorManager;

/// <summary>
/// A wrapper class for <see cref="DoorVariant"/>.
/// </summary>
public class Door : IWrapper<DoorVariant>, IWorldSpace
{
    /// <summary>
    /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="DoorVariant"/>s and their corresponding <see cref="Door"/>.
    /// </summary>
    internal static readonly Dictionary<DoorVariant, Door> DoorVariantToDoor = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Door"/> class.
    /// </summary>
    /// <param name="door">The base <see cref="DoorVariant"/> for this door.</param>
    /// <param name="room">The <see cref="Room"/> for this door.</param>
    public Door(DoorVariant door, Room room)
    {
        if (room != null)
            DoorVariantToDoor.Add(door, this);

        Base = door;
        Room = room;
        Type = GetDoorType();
    }

    /// <summary>
    /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains all the <see cref="Door"/> instances.
    /// </summary>
    public static IEnumerable<Door> List
    {
        get => DoorVariantToDoor.Values;
    }

    /// <summary>
    /// Gets the base-game <see cref="DoorVariant"/> for this door.
    /// </summary>
    public DoorVariant Base { get; }

    /// <summary>
    /// Gets the <see cref="UnityEngine.GameObject"/> of the door.
    /// </summary>
    public GameObject GameObject
    {
        get => Base.gameObject;
    }

    /// <summary>
    /// Gets the door's <see cref="UnityEngine.Transform"/>.
    /// </summary>
    public Transform Transform
    {
        get => Base.transform;
    }

    /// <summary>
    /// Gets the <see cref="DoorType"/> of the door.
    /// </summary>
    public DoorType Type { get; }

    /// <summary>
    /// Gets the <see cref="Features.Room"/> that the door is located in.
    /// </summary>
    public Room Room { get; }

    /// <summary>
    /// Gets a value indicating whether or not the door is fully closed.
    /// </summary>
    public bool IsFullyClosed
    {
        get => ExactState is 0;
    }

    /// <summary>
    /// Gets a value indicating whether the door is fully open.
    /// </summary>
    public bool IsFullyOpen
    {
        get => ExactState is 1;
    }

    /// <summary>
    /// Gets a value indicating whether or not the door is currently moving.
    /// </summary>
    public bool IsMoving
    {
        get => ExactState is not (0 or 1);
    }

    /// <summary>
    /// Gets a value indicating the precise state of the door, from <c>0-1</c>. A value of <c>0</c> indicates the door is fully closed, while a value of <c>1</c> indicates the door is fully open. Values in-between represent the door's animation progress.
    /// </summary>
    public float ExactState
    {
        get => Base.GetExactState();
    }

    /// <summary>
    /// Gets or sets a value indicating whether the door is open.
    /// </summary>
    public bool IsOpen
    {
        get => Base.IsConsideredOpen();
        set => Base.NetworkTargetState = value;
    }

    /// <summary>
    /// Gets a value indicating whether or not this door is a gate.
    /// </summary>
    public bool IsGate
    {
        get => Base is PryableDoor;
    }

    /// <summary>
    /// Gets a value indicating whether or not this door is a checkpoint door.
    /// </summary>
    public bool IsCheckpoint
    {
        get => Base is CheckpointDoor;
    }

    /// <summary>
    /// Gets a value indicating whether or not this door is an elevator door.
    /// </summary>
    public bool IsElevator
    {
        get => Base is ElevatorDoor;
    }

    /// <summary>
    /// Gets a value indicating whether or not this door requires a keycard to open.
    /// </summary>
    public bool IsKeycardDoor
    {
        get => RequiredPermissions.RequiredPermissions != Interactables.Interobjects.DoorUtils.KeycardPermissions.None;
    }

    /// <summary>
    /// Gets or sets the door's position.
    /// </summary>
    public Vector3 Position
    {
        get => GameObject.transform.position;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            GameObject.transform.position = value;
            NetworkServer.Spawn(GameObject);
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not SCP-106 can walk through the door.
    /// </summary>
    public bool AllowsScp106
    {
        get => Base is IScp106PassableDoor door && door.IsScp106Passable;
    }

    /// <summary>
    /// Gets a value indicating whether or not the door is locked.
    /// </summary>
    public bool IsLocked
    {
        get => DoorLockType > 0;
    }

    /// <summary>
    /// Gets or the door lock type.
    /// </summary>
    public DoorLockType DoorLockType
    {
        get => (DoorLockType)Base.NetworkActiveLocks;
    }

    /// <summary>
    /// Gets a value indicating whether or not this door is breakable.
    /// </summary>
    public bool IsBreakable
    {
        get => Base is IDamageableDoor dDoor && !dDoor.IsDestroyed;
    }

    /// <summary>
    /// Gets a value indicating whether or not this door is broken.
    /// </summary>
    public bool IsBroken
    {
        get => Base is IDamageableDoor dDoor && dDoor.IsDestroyed;
    }

    /// <summary>
    /// Gets a value indicating whether or not this door is ignoring lockdown.
    /// </summary>
    public bool IgnoresLockdowns
    {
        get => Base is INonInteractableDoor nonInteractableDoor && nonInteractableDoor.IgnoreLockdowns;
    }

    /// <summary>
    /// Gets a value indicating whether or not this door is ignoring remoteAdmin commands.
    /// </summary>
    public bool IgnoresRemoteAdmin
    {
        get => Base is INonInteractableDoor nonInteractableDoor && nonInteractableDoor.IgnoreRemoteAdmin;
    }

    /// <summary>
    /// Gets the door's Instance ID.
    /// </summary>
    public int InstanceId
    {
        get => Base.GetInstanceID();
    }

    /// <summary>
    /// Gets a nametag of a door.
    /// </summary>
    public DoorNametagExtension Nametag
    {
        get => Base.GetComponent<DoorNametagExtension>();
    }

    /// <summary>
    /// Gets the name of this door.
    /// </summary>
    public string Name
    {
        get => Nametag == null ? GameObject.name.GetBefore(' ') : Nametag.GetName.RemoveBracketsOnEndOfName();
    }

    /// <summary>
    /// Gets or sets the required permissions to open the door.
    /// </summary>
    public DoorPermissions RequiredPermissions
    {
        get => Base.RequiredPermissions;
        set => Base.RequiredPermissions = value;
    }

    /// <summary>
    /// Gets or sets the max health of the door. No effect if the door cannot be broken.
    /// </summary>
    public float MaxHealth
    {
        get => Base is BreakableDoor breakable ? breakable._maxHealth : float.NaN;
        set
        {
            if (Base is BreakableDoor breakable)
                breakable._maxHealth = value;
        }
    }

    /// <summary>
    /// Gets or sets the door's remaining health. No effect if the door cannot be broken.
    /// </summary>
    public float Health
    {
        get => Base is BreakableDoor breakable ? breakable.RemainingHealth : float.NaN;
        set
        {
            if (Base is BreakableDoor breakable)
                breakable.RemainingHealth = value;
        }
    }

    /// <summary>
    /// Gets or sets the damage types this door ignores, if it is breakable.
    /// </summary>
    public DoorDamageType IgnoredDamageTypes
    {
        get => Base is BreakableDoor breakable ? breakable._ignoredDamageSources : DoorDamageType.None;
        set
        {
            if (Base is BreakableDoor breakable)
                breakable._ignoredDamageSources = value;
        }
    }

    /// <summary>
    /// Gets or sets the door's rotation.
    /// </summary>
    public Quaternion Rotation
    {
        get => GameObject.transform.rotation;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            GameObject.transform.rotation = value;
            NetworkServer.Spawn(GameObject);
        }
    }

    /// <summary>
    /// Gets or sets the size scale of the door.
    /// </summary>
    public Vector3 Scale
    {
        get => GameObject.transform.localScale;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            GameObject.transform.localScale = value;
            NetworkServer.Spawn(GameObject);
        }
    }

    /// <summary>
    /// Gets the door's <see cref="ZoneType"/>.
    /// </summary>
    public ZoneType Zone
    {
        get => Room?.Zone ?? ZoneType.Unspecified;
    }

    /// <summary>
    /// Gets the door object associated with a specific <see cref="DoorVariant"/>, or creates a new one if there isn't one.
    /// </summary>
    /// <param name="doorVariant">The base-game <see cref="DoorVariant"/>.</param>
    /// <returns>A <see cref="Door"/> wrapper object.</returns>
    public static Door Get(DoorVariant doorVariant) => doorVariant != null ? (DoorVariantToDoor.TryGetValue(doorVariant, out var door)
        ? door
        : new Door(doorVariant, doorVariant.GetComponentInParent<Room>())) : null;

    /// <summary>
    /// Gets a <see cref="Door"/> given the specified name.
    /// </summary>
    /// <param name="name">The name to search for.</param>
    /// <returns>The <see cref="Door"/> with the given name or <see langword="null"/> if not found.</returns>
    public static Door Get(string name)
    {
        DoorNametagExtension.NamedDoors.TryGetValue(name, out var nameExtension);
        return nameExtension is null ? null : Get(nameExtension.TargetDoor);
    }

    /// <summary>
    /// Gets the door object associated with a specific <see cref="UnityEngine.GameObject"/>, or creates a new one if there isn't one.
    /// </summary>
    /// <param name="gameObject">The base-game <see cref="UnityEngine.GameObject"/>.</param>
    /// <returns>The <see cref="Door"/> with the given name or <see langword="null"/> if not found.</returns>
    public static Door Get(GameObject gameObject) => gameObject is null ? null : Get(gameObject.GetComponentInChildren<DoorVariant>());

    /// <summary>
    /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> filtered based on a predicate.
    /// </summary>
    /// <param name="predicate">The condition to satify.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains elements that satify the condition.</returns>
    public static IEnumerable<Door> Get(Func<Door, bool> predicate) => List.Where(predicate);

    /// <summary>
    /// Gets a <see cref="Door"/> given the specified <see cref="DoorType"/>.
    /// </summary>
    /// <param name="doorType">The <see cref="DoorType"/> to search for.</param>
    /// <returns>The <see cref="Door"/> with the given <see cref="DoorType"/> or <see langword="null"/> if not found.</returns>
    public static Door Get(DoorType doorType) => List.FirstOrDefault(x => x.Type == doorType);

    /// <summary>
    /// Returns the closest <see cref="Door"/> to the given <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The <see cref="Vector3">position</see> to find the closest door to.</param>
    /// <param name="distance">The distance between the door and the point.</param>
    /// <returns>The door closest to the provided position.</returns>
    public static Door GetClosest(Vector3 position, out float distance)
    {
        var doorToReturn = List.OrderBy(door => Vector3.Distance(position, door.Position)).FirstOrDefault();

        distance = Vector3.Distance(position, doorToReturn.Position);
        return doorToReturn;
    }

    /// <summary>
    /// Gets a random <see cref="Door"/>.
    /// </summary>
    /// <param name="type">Filters by <see cref="ZoneType"/>.</param>
    /// <param name="onlyUnbroken">A value indicating whether it filters broken doors.</param>
    /// <returns><see cref="Door"/> object.</returns>
    public static Door Random(ZoneType type = ZoneType.Unspecified, bool onlyUnbroken = false)
    {
        var doors = onlyUnbroken || type is not ZoneType.Unspecified ? Get(x => (x.Room is null || x.Room.Zone.HasFlag(type) || type == ZoneType.Unspecified) && (!x.IsBroken || !onlyUnbroken)).ToList() : DoorVariantToDoor.Values.ToList();
        return doors[UnityEngine.Random.Range(0, doors.Count)];
    }

    /// <summary>
    /// Locks all <see cref="Door">doors</see> given the specified <see cref="ZoneType"/>.
    /// </summary>
    /// <param name="duration">The duration of the lockdown.</param>
    /// <param name="zoneType">The <see cref="ZoneType"/> to affect.</param>
    /// <param name="lockType">The specified <see cref="Enums.DoorLockType"/>.</param>
    public static void LockAll(float duration, ZoneType zoneType = ZoneType.Unspecified, DoorLockType lockType = DoorLockType.Regular079)
    {
        foreach (var door in Get(door => zoneType is not ZoneType.Unspecified && door.Zone.HasFlag(zoneType)))
        {
            door.IsOpen = false;
            door.ChangeLock(lockType);
            Timing.CallDelayed(duration, () => door.Unlock());
        }
    }

    /// <summary>
    /// Locks all <see cref="Door">doors</see> given the specified <see cref="ZoneType"/>.
    /// </summary>
    /// <param name="duration">The duration of the lockdown.</param>
    /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
    /// <param name="lockType">The specified <see cref="Enums.DoorLockType"/>.</param>
    public static void LockAll(float duration, IEnumerable<ZoneType> zoneTypes, DoorLockType lockType = DoorLockType.Regular079)
    {
        foreach (var zone in zoneTypes)
            LockAll(duration, zone, lockType);
    }

    /// <summary>
    /// Locks all <see cref="Door">doors</see> in the facility.
    /// </summary>
    /// <param name="duration">The duration of the lockdown.</param>
    /// <param name="lockType">The specified <see cref="Enums.DoorLockType"/>.</param>
    public static void LockAll(float duration, DoorLockType lockType = DoorLockType.Regular079)
    {
        foreach (var door in List)
        {
            door.IsOpen = false;
            door.ChangeLock(lockType);
            Timing.CallDelayed(duration, () => door.Unlock());
        }
    }

    /// <summary>
    /// Unlocks all <see cref="Door">doors</see> in the facility.
    /// </summary>
    public static void UnlockAll()
    {
        foreach (var door in List)
            door.Unlock();
    }

    /// <summary>
    /// Unlocks all <see cref="Door">doors</see> in the facility.
    /// </summary>
    /// <param name="zoneType">The <see cref="ZoneType"/> to affect.</param>
    public static void UnlockAll(ZoneType zoneType) => UnlockAll(door => door.Zone.HasFlag(zoneType));

    /// <summary>
    /// Unlocks all <see cref="Door">doors</see> in the facility.
    /// </summary>
    /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
    public static void UnlockAll(IEnumerable<ZoneType> zoneTypes) => UnlockAll(door => zoneTypes.Contains(door.Zone));

    /// <summary>
    /// Unlocks all <see cref="Door">doors</see> in the facility.
    /// </summary>
    /// <param name="predicate">The condition to satify.</param>
    public static void UnlockAll(Func<Door, bool> predicate)
    {
        foreach (var door in Get(predicate))
            door.Unlock();
    }

    /// <summary>
    /// Breaks the specified door. No effect if the door cannot be broken, or if it is already broken.
    /// </summary>
    /// <param name="type">The <see cref="DoorDamageType"/> to apply to the door.</param>
    /// <returns><see langword="true"/> if the door was broken, <see langword="false"/> if it was unable to be broken, or was already broken before.</returns>
    public bool BreakDoor(DoorDamageType type = DoorDamageType.ServerCommand)
    {
        if (Base is IDamageableDoor dmg && !dmg.IsDestroyed)
        {
            dmg.ServerDamage(ushort.MaxValue, type);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Damages the door. No effect if the door cannot be broken.
    /// </summary>
    /// <param name="amount">The amount of damage to deal.</param>
    /// <param name="type">The damage type to use.</param>
    /// <returns><see langword="true"/> if the door was damaged.</returns>
    public bool DamageDoor(float amount, DoorDamageType type = DoorDamageType.ServerCommand) => Base is BreakableDoor breakable && breakable.ServerDamage(amount, type);

    /// <summary>
    /// Tries to pry the door open. No effect if the door cannot be pried.
    /// </summary>
    /// <returns><see langword="true"/> if the door was able to be pried open.</returns>
    public bool TryPryOpen() => Base is PryableDoor pryable && pryable.TryPryGate(null);

    /// <summary>
    /// Tries to pry the door open. No effect if the door cannot be pried.
    /// </summary>
    /// <returns><see langword="true"/> if the door was able to be pried open.</returns>
    /// <param name="player">The amount of damage to deal.</param>
    public bool TryPryOpen(Player player) => Base is PryableDoor pryable && pryable.TryPryGate(player.ReferenceHub);

    /// <summary>
    /// Change the door lock with the given lock type.
    /// </summary>
    /// <param name="lockType">The <see cref="Enums.DoorLockType"/> to use.</param>
    public void ChangeLock(DoorLockType lockType)
    {
        if (lockType == DoorLockType.None)
        {
            Base.NetworkActiveLocks = 0;
        }
        else
        {
            var locks = DoorLockType;
            if (locks.HasFlag(lockType))
                locks &= ~lockType;
            else
                locks |= lockType;

            Base.NetworkActiveLocks = (ushort)locks;
        }

        DoorEvents.TriggerAction(Base, IsLocked ? DoorAction.Locked : DoorAction.Unlocked, null);
    }

    /// <summary>
    /// Locks all active locks on the door, and then reverts back any changes after a specified length of time.
    /// </summary>
    /// <param name="time">The amount of time that must pass before unlocking the door.</param>
    /// <param name="lockType">The <see cref="Enums.DoorLockType"/> of the lockdown.</param>
    public void Lock(float time, DoorLockType lockType)
    {
        ChangeLock(lockType);
        Unlock(time, lockType);
    }

    /// <summary>
    /// Unlocks and clears all active locks on the door.
    /// </summary>
    public void Unlock() => ChangeLock(DoorLockType.None);

    /// <summary>
    /// Unlocks and clears all active locks on the door after a specified length of time.
    /// </summary>
    /// <param name="time">The amount of time that must pass before unlocking the door.</param>
    /// <param name="flagsToUnlock">The <see cref="Enums.DoorLockType"/> of the lockdown.</param>
    public void Unlock(float time, DoorLockType flagsToUnlock) => DoorScheduledUnlocker.UnlockLater(Base, time, (DoorLockReason)flagsToUnlock);

    /// <summary>
    /// Returns the Door in a human-readable format.
    /// </summary>
    /// <returns>A string containing Door-related data.</returns>
    public override string ToString() => $"{Type} ({Zone}) [{Room}] *{DoorLockType}* |{Health}/{MaxHealth}| ={RequiredPermissions.RequiredPermissions}= -{IgnoredDamageTypes}-";

    /// <summary>
    /// Gets the door object associated with a specific <see cref="DoorVariant"/>, or creates a new one if there isn't one.
    /// </summary>
    /// <param name="doorVariant">The base-game <see cref="DoorVariant"/>.</param>
    /// <param name="room">The <see cref="Room"/> this door is in.</param>
    /// <remarks>The 'room' parameter is only used if a new door wrapper needs to be created.</remarks>
    /// <returns>A <see cref="Door"/> wrapper object.</returns>
    internal static Door Get(DoorVariant doorVariant, Room room) => DoorVariantToDoor.ContainsKey(doorVariant)
        ? DoorVariantToDoor[doorVariant]
        : new Door(doorVariant, room);

    public DoorType GetDoorType()
    {
        if (Nametag is null)
        {
            var doorName = GameObject.name.GetBefore(' ');
            return doorName switch
            {
                "LCZ" => Room?.Type switch
                {
                    RoomType.LczCheckpointA or RoomType.LczCheckpointB or RoomType.HczEzCheckpointA
                        or RoomType.HczEzCheckpointB => Get(Base.GetComponentInParent<CheckpointDoor>())?.Type ?? DoorType.LightContainmentDoor,
                    RoomType.LczAirlock => (Base.GetComponentInParent<AirlockController>() != null) ? DoorType.Airlock : DoorType.LightContainmentDoor,
                    _ => DoorType.LightContainmentDoor,
                },
                "HCZ" => DoorType.HeavyContainmentDoor,
                "EZ" => DoorType.EntranceDoor,
                "Prison" => DoorType.PrisonDoor,
                "914" => DoorType.Scp914Door,
                "Intercom" => Room?.Type switch
                {
                    RoomType.HczEzCheckpointA => DoorType.CheckpointArmoryA,
                    RoomType.HczEzCheckpointB => DoorType.CheckpointArmoryB,
                    _ => DoorType.UnknownDoor,
                },
                "Unsecured" => Room?.Type switch
                {
                    RoomType.EzCheckpointHallway => DoorType.CheckpointGate,
                    RoomType.Hcz049 => DoorType.Scp049Gate,
                    _ => DoorType.UnknownGate,
                },
                "Elevator" => (Base as ElevatorDoor)?.Group switch
                {
                    ElevatorGroup.Nuke => DoorType.ElevatorNuke,
                    ElevatorGroup.Scp049 => DoorType.ElevatorScp049,
                    ElevatorGroup.GateB => DoorType.ElevatorGateB,
                    ElevatorGroup.GateA => DoorType.ElevatorGateA,
                    ElevatorGroup.LczA01 or ElevatorGroup.LczA02 => DoorType.ElevatorLczA,
                    ElevatorGroup.LczB01 or ElevatorGroup.LczB02 => DoorType.ElevatorLczB,
                    _ => DoorType.UnknownElevator,
                },
                _ => DoorType.UnknownDoor,
            };
        }

        return Name.RemoveBracketsOnEndOfName() switch
        {
            // Doors contains the DoorNameTagExtension component
            "CHECKPOINT_LCZ_A" => DoorType.CheckpointLczA,
            "CHECKPOINT_LCZ_B" => DoorType.CheckpointLczB,
            "CHECKPOINT_EZ_HCZ_A" => DoorType.CheckpointEzHczA,
            "CHECKPOINT_EZ_HCZ_B" => DoorType.CheckpointEzHczB,
            "106_PRIMARY" => DoorType.Scp106Primary,
            "106_SECONDARY" => DoorType.Scp106Secondary,
            "106_BOTTOM" => DoorType.Scp106Bottom,
            "ESCAPE_PRIMARY" => DoorType.EscapePrimary,
            "ESCAPE_SECONDARY" => DoorType.EscapeSecondary,
            "INTERCOM" => DoorType.Intercom,
            "NUKE_ARMORY" => DoorType.NukeArmory,
            "LCZ_ARMORY" => DoorType.LczArmory,
            "SURFACE_NUKE" => DoorType.NukeSurface,
            "HID" => DoorType.HID,
            "HCZ_ARMORY" => DoorType.HczArmory,
            "096" => DoorType.Scp096,
            "049_ARMORY" => DoorType.Scp049Armory,
            "914" => DoorType.Scp914Gate,
            "GATE_A" => DoorType.GateA,
            "079_FIRST" => DoorType.Scp079First,
            "GATE_B" => DoorType.GateB,
            "079_SECOND" => DoorType.Scp079Second,
            "SERVERS_BOTTOM" => DoorType.ServersBottom,
            "173_CONNECTOR" => DoorType.Scp173Connector,
            "LCZ_WC" => DoorType.LczWc,
            "HID_RIGHT" => DoorType.HIDRight,
            "HID_LEFT" => DoorType.HIDLeft,
            "173_ARMORY" => DoorType.Scp173Armory,
            "173_GATE" => DoorType.Scp173Gate,
            "GR18" => DoorType.GR18Gate,
            "SURFACE_GATE" => DoorType.SurfaceGate,
            "330" => DoorType.Scp330,
            "330_CHAMBER" => DoorType.Scp330Chamber,
            "GR18_INNER" => DoorType.GR18Inner,
            "939_CRYO" => DoorType.Scp939Cryo,

            // Doors spawned by the DoorSpawnPoint component
            "LCZ_CAFE" => DoorType.LczCafe,
            "173_BOTTOM" => DoorType.Scp173Bottom,

            // Doors contains the Door component,
            // also gameobject names
            "LightContainmentDoor" => DoorType.LightContainmentDoor,
            "EntrDoor" => DoorType.EntranceDoor,
            _ => DoorType.UnknownDoor,
        };
    }
}