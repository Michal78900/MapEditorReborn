namespace MapEditorReborn.API
{
    using Exiled.API.Features;
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Component added to spawned DoorObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class DoorObjectComponent : MapEditorObject
    {
        /// <inheritdoc cref="Door.Type"/>
        public Exiled.API.Enums.DoorType DoorType => door.GetDoorTypeByName();

        /// <inheritdoc cref="Door.IsOpen"/>
        public bool IsOpen => door.IsOpen;

        /// <inheritdoc cref="Door.IsLocked"/>
        public bool IsLocked => (int)door.DoorLockType == 64;

        /// <inheritdoc cref="Door.RequiredPermissions"/>
        public KeycardPermissions DoorPermissions => door.RequiredPermissions.RequiredPermissions;

        /// <inheritdoc cref="Door.IgnoredDamageTypes"/>
        public DoorDamageType IgnoredDamageTypes => door.IgnoredDamageTypes;

        /// <inheritdoc cref="Door.MaxHealth"/>
        public float MaxHealth => door.MaxHealth;

        /// <summary>
        /// When set to <see langword="false"/> the door won't open on warhead activation.
        /// </summary>
        public bool OpenOnWarheadActivation = false;

        private void Awake() => door = Door.Get(gameObject.GetComponent<DoorVariant>());

        private Door door;
    }
}
