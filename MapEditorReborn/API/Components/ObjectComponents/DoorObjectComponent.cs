namespace MapEditorReborn.API
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Component added to spawned DoorObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class DoorObjectComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoorObjectComponent"/> class.
        /// </summary>
        /// <param name="doorObject">The <see cref="DoorObject"/> to initialize.</param>
        /// <returns>Instance of this compoment.</returns>
        public DoorObjectComponent Init(DoorObject doorObject)
        {
            Base = doorObject;
            door = Door.Get(GetComponent<DoorVariant>());
            Base.DoorType = door.Type;

            UpdateObject();

            return this;
        }

        public DoorObject Base;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            door.IsOpen = Base.IsOpen;
            door.ChangeLock(Base.IsLocked ? DoorLockType.SpecialDoorFeature : DoorLockType.None);
            door.RequiredPermissions.RequiredPermissions = Base.KeycardPermissions;
            door.IgnoredDamageTypes = Base.IgnoredDamageSources;
            door.MaxHealth = Base.DoorHealth;

            base.UpdateObject();
        }

        private Door door;
    }
}
