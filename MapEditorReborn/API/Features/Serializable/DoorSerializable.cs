namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using Enums;
    using Exiled.API.Enums;
    using Interactables.Interobjects.DoorUtils;

    using KeycardPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions;

    /// <summary>
    /// Represents <see cref="DoorVariant"/> used by the plugin to spawn and save doors to a file.
    /// </summary>
    [Serializable]
    public class DoorSerializable : SerializableObject
    {
        /// <summary>
        /// Gets or sets the door <see cref="DoorType"/>.
        /// </summary>
        public DoorType DoorType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the door is opened or not.
        /// </summary>
        public bool IsOpen { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the door is locked or not.
        /// </summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the door has keycard permissions or not.
        /// </summary>
        public KeycardPermissions KeycardPermissions { get; set; }

        /// <summary>
        /// Gets or sets <see cref="DoorDamageType"/> ignored by the door.
        /// </summary>
        public DoorDamageType IgnoredDamageSources { get; set; } = DoorDamageType.Weapon;

        /// <summary>
        /// Gets or sets health of the door.
        /// </summary>
        public float DoorHealth { get; set; } = 150f;

        public LockOnEvent LockOnEvent { get; set; } = LockOnEvent.None;
    }
}