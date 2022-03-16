namespace MapEditorReborn.API.Features.Objects
{
    using Exiled.API.Enums;
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
                Door = Door.Get(doorVariant);

            Base.DoorType = Door.GetDoorTypeByName();
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
}
