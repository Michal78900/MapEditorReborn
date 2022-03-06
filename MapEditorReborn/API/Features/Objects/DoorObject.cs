namespace MapEditorReborn.API.Features.Objects
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Extensions;
    using Features;
    using Features.Serializable;
    using Interactables.Interobjects.DoorUtils;

    using static API;

    /// <summary>
    /// Component added to spawned DoorObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class DoorObject : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="DoorObject"/>.
        /// </summary>
        /// <param name="doorSerializable">The <see cref="DoorSerializable"/> to initialize.</param>
        /// <returns>Instance of this compoment.</returns>
        public DoorObject Init(DoorSerializable doorSerializable)
        {
            Base = doorSerializable;

            if (TryGetComponent(out DoorVariant doorVariant))
                door = Door.Get(doorVariant);

            Base.DoorType = door.GetDoorTypeByName();
            prevBase.CopyProperties(Base);

            ForcedRoomType = doorSerializable.RoomType != RoomType.Unknown ? doorSerializable.RoomType : FindRoom().Type;
            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public DoorSerializable Base;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            if (prevBase.DoorType != Base.DoorType)
            {
                SpawnedObjects[SpawnedObjects.IndexOf(this)] = ObjectSpawner.SpawnDoor(Base, transform.position, transform.rotation);
                Destroy();

                return;
            }

            prevBase.CopyProperties(Base);
            door.IsOpen = Base.IsOpen;
            door.ChangeLock(Base.IsLocked ? DoorLockType.SpecialDoorFeature : DoorLockType.None);
            door.RequiredPermissions.RequiredPermissions = Base.KeycardPermissions;
            door.IgnoredDamageTypes = Base.IgnoredDamageSources;
            door.MaxHealth = Base.DoorHealth;
            door.Health = Base.DoorHealth;

            base.UpdateObject();
        }

        private Door door;
        private DoorSerializable prevBase = new DoorSerializable();
    }
}
