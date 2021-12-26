namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using Interactables.Interobjects.DoorUtils;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="DoorVariant"/> used by the plugin to spawn and save doors to a file.
    /// </summary>
    [Serializable]
    public class DoorObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoorObject"/> class.
        /// </summary>
        public DoorObject()
        {
        }

        /// <inheritdoc cref="DoorObject()"/>
        public DoorObject(DoorType doorType, Vector3 position, Vector3 rotation, Vector3 scale, RoomType roomType, bool isOpen, bool isLocked, KeycardPermissions keycardPermissions, DoorDamageType doorDamageType, float doorHealth, bool openOnWarheadActivation)
        {
            DoorType = doorType;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            RoomType = roomType;

            IsOpen = isOpen;
            IsLocked = isLocked;
            KeycardPermissions = keycardPermissions;
            IgnoredDamageSources = doorDamageType;
            DoorHealth = doorHealth;
            OpenOnWarheadActivation = openOnWarheadActivation;
        }

        /// <summary>
        /// Gets or sets the door <see cref="DoorType"/>.
        /// </summary>
        public DoorType DoorType { get; set; }

        /// <summary>
        /// Gets or sets the door's position.
        /// </summary>
        // public Vector3 Position { get; set; } = Vector3.zero;
        public SerializableVector3 Position { get; set; } = SerializableVector3.Zero;

        /// <summary>
        /// Gets or sets the door's rotation.
        /// </summary>
        // public Vector3 Rotation { get; set; } = Vector3.zero;
        public SerializableVector3 Rotation { get; set; } = SerializableVector3.Zero;

        /// <summary>
        /// Gets or sets the door's scale.
        /// </summary>
        // public Vector3 Scale { get; set; } = Vector3.one;
        public SerializableVector3 Scale { get; set; } = SerializableVector3.One;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;

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
        public KeycardPermissions KeycardPermissions { get; set; } = KeycardPermissions.None;

        /// <summary>
        /// Gets or sets <see cref="DoorDamageType"/> ignored by the door.
        /// </summary>
        public DoorDamageType IgnoredDamageSources { get; set; } = DoorDamageType.Weapon;

        /// <summary>
        /// Gets or sets health of the door.
        /// </summary>
        public float DoorHealth { get; set; } = 150f;

        /// <summary>
        /// Gets or sets a value indicating whether the door will open automatically on warhead activation or not.
        /// </summary>
        public bool OpenOnWarheadActivation { get; set; } = false;
    }
}