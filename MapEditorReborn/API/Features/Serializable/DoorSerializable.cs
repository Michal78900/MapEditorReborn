﻿namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using Enums;
    using Exiled.API.Enums;
    using Interactables.Interobjects.DoorUtils;
    using UnityEngine;

    using KeycardPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions;

    /// <summary>
    /// Represents <see cref="DoorVariant"/> used by the plugin to spawn and save doors to a file.
    /// </summary>
    [Serializable]
    public class DoorSerializable
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

        /// <summary>
        /// Gets or sets the door's position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the door's rotation.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the door's scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}