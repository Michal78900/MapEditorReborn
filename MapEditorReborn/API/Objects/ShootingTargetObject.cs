namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

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
        public ShootingTargetObject(string targetType, Vector3 position, Vector3 rotation, Vector3 scale, RoomType roomType)
        {
            TargetType = targetType;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            RoomType = roomType;
        }

        public string TargetType { get; private set; } = "Sport";

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
