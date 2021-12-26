namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="Exiled.API.Features.ShootingTarget"/> used by the plugin to spawn and save ShootingTargets to a file.
    /// </summary>
    [Serializable]
    public class ShootingTargetObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShootingTargetObject"/> class.
        /// </summary>
        public ShootingTargetObject()
        {
        }

        /// <summary>
        /// Gets or sets the shooting target's type.
        /// </summary>
        public ShootingTargetType TargetType { get; set; } = ShootingTargetType.Sport;

        /// <summary>
        /// Gets or sets a value indicating whether shooting target is functional (ex. plays CASSIE on shot).
        /// </summary>
        public bool IsFunctional { get; set; } = true;

        /// <summary>
        /// Gets or sets the shooting target's position.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the shooting target's rotation.
        /// </summary>
        public Vector3 Rotation { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the shooting target's scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
