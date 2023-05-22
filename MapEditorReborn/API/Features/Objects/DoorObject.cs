// -----------------------------------------------------------------------
// <copyright file="DoorObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using MapEditorReborn.Exiled.Enums;
using MapEditorReborn.Exiled.Features;

namespace MapEditorReborn.API.Features.Objects;

using Features;
using Interactables.Interobjects.DoorUtils;
using Serializable;
using static API;

/// <summary>
/// Component added to spawned DoorObject. Is is used for easier idendification of the object and it's variables.
/// </summary>
public class DoorObject : MapEditorObject
{
    private void Awake()
    {
        Door = Door.Get(GetComponent<DoorVariant>());
    }

    /// <summary>
    /// Initializes the <see cref="DoorObject"/>.
    /// </summary>
    /// <param name="doorSerializable">The <see cref="DoorSerializable"/> to initialize.</param>
    /// <returns>Instance of this component.</returns>
    public DoorObject Init(DoorSerializable doorSerializable)
    {
        Base = doorSerializable;
        Base.DoorType = Door.GetDoorType();
        prevType = Base.DoorType;

        ForcedRoomType = doorSerializable.RoomType != RoomType.Unknown ? doorSerializable.RoomType : FindRoom().Type;
        UpdateObject();

        return this;
    }

    /// <summary>
    /// The config-base of the object containing all of it's properties.
    /// </summary>
    public DoorSerializable Base;

    public Door Door { get; private set; }

    /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
    public override void UpdateObject()
    {
        if (prevType != Base.DoorType)
        {
            SpawnedObjects[SpawnedObjects.IndexOf(this)] = ObjectSpawner.SpawnDoor(Base, Position, Rotation);
            Destroy();

            return;
        }

        prevType = Base.DoorType;
        Door.IsOpen = Base.IsOpen;
        Door.ChangeLock(Base.IsLocked ? DoorLockType.SpecialDoorFeature : DoorLockType.None);
        Door.RequiredPermissions.RequiredPermissions = Base.KeycardPermissions;
        Door.IgnoredDamageTypes = Base.IgnoredDamageSources;
        Door.MaxHealth = Base.DoorHealth;
        Door.Health = Base.DoorHealth;

        base.UpdateObject();
    }

    private DoorType prevType;
}