// -----------------------------------------------------------------------
// <copyright file="DoorObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;
    using Extensions;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;
    using MEC;
    using Mirror;
    using RelativePositioning;
    using Serializable;
    using static API;
    using BreakableDoor = Interactables.Interobjects.BreakableDoor;

    /// <summary>
    /// Component added to spawned DoorObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class DoorObject : MapEditorObject
    {
        private void Awake()
        {
            Door = Door.Get(GetComponent<DoorVariant>());
            _netIdWaypoint = GetComponent<NetIdWaypoint>();
        }

        /// <summary>
        /// Initializes the <see cref="DoorObject"/>.
        /// </summary>
        /// <param name="doorSerializable">The <see cref="DoorSerializable"/> to initialize.</param>
        /// <returns>Instance of this component.</returns>
        public virtual DoorObject Init(DoorSerializable doorSerializable)
        {
            Base = doorSerializable;
            Base.DoorType = Door.GetDoorType();
            _prevType = Base.DoorType;
            _remainingHealth = Base.DoorHealth;

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
            if (_prevType != Base.DoorType)
            {
                SpawnedObjects[SpawnedObjects.IndexOf(this)] = ObjectSpawner.SpawnDoor(Base, Position, Rotation);
                Destroy();

                return;
            }

            _prevType = Base.DoorType;
            Door.IsOpen = Base.IsOpen;
            Door.ChangeLock(Base.IsLocked ? DoorLockType.SpecialDoorFeature : DoorLockType.None);
            Door.RequiredPermissions.RequiredPermissions = Base.KeycardPermissions;
            if (Door is Exiled.API.Features.Doors.BreakableDoor breakableDoor)
            {
                breakableDoor.IgnoredDamage = Base.IgnoredDamageSources;
                breakableDoor.MaxHealth = Base.DoorHealth;
                breakableDoor.Health = Base.DoorHealth;
                _remainingHealth = Base.DoorHealth;
            }

            _netIdWaypoint.SetPosition();

            base.UpdateObject();
        }

        public void BreakDoor()
        {
            if (Door.Base is BreakableDoor breakableDoor)
            {
                breakableDoor.Network_destroyed = true;
                return;
            }

            if (Door.Base is PryableDoor pryableDoor)
            {
                pryableDoor.RpcPryGate();
                Timing.CallDelayed(1.8f, () => NetworkServer.UnSpawn(gameObject));
            }
        }

        private DoorType _prevType;
        private NetIdWaypoint _netIdWaypoint;
        internal float _remainingHealth;
    }
}