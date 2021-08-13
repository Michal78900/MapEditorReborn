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
        /// Gets the door <see cref="DoorType"/>.
        /// </summary>
        public DoorType DoorType { get; private set; }

        /// <summary>
        /// Gets the door's position.
        /// </summary>
        public Vector3 Position { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the door's rotation.
        /// </summary>
        public Vector3 Rotation { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the door's scale.
        /// </summary>
        public Vector3 Scale { get; private set; } = Vector3.one;

        /// <summary>
        /// Gets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; private set; } = RoomType.Unknown;

        /// <summary>
        /// Gets a value indicating whether the door is opened or not.
        /// </summary>
        public bool IsOpen { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the door is locked or not.
        /// </summary>
        public bool IsLocked { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the door has keycard permissions or not.
        /// </summary>
        public KeycardPermissions KeycardPermissions { get; private set; } = KeycardPermissions.None;

        /// <summary>
        /// Gets <see cref="DoorDamageType"/> ignored by the door.
        /// </summary>
        public DoorDamageType IgnoredDamageSources { get; private set; } = DoorDamageType.Weapon;

        /// <summary>
        /// Gets health of the door.
        /// </summary>
        public float DoorHealth { get; private set; } = 150f;

        /// <summary>
        /// Gets a value indicating whether the door will open automatically on warhead activation or not.
        /// </summary>
        public bool OpenOnWarheadActivation { get; private set; } = false;
    }
}