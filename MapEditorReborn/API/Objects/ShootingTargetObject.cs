namespace MapEditorReborn.API
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

        /// <inheritdoc cref="ShootingTargetObject()"/>
        public ShootingTargetObject(ShootingTargetType targetType, Vector3 position, Vector3 rotation, Vector3 scale, RoomType roomType)
        {
            TargetType = targetType;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            RoomType = roomType;
        }

        /// <summary>
        /// Gets the shooting target's type.
        /// </summary>
        public ShootingTargetType TargetType { get; private set; } = ShootingTargetType.Sport;

        /// <summary>
        /// Gets the shooting target's position.
        /// </summary>
        public Vector3 Position { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the shooting target's rotation.
        /// </summary>
        public Vector3 Rotation { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the shooting target's scale.
        /// </summary>
        public Vector3 Scale { get; private set; } = Vector3.one;

        /// <summary>
        /// Gets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; private set; } = RoomType.Unknown;
    }
}
