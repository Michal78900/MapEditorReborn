namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A tool used to spawn and save ShootingTargets to a file.
    /// </summary>
    [Serializable]
    public class ShootingTargetObject
    {
        /// <summary>
        /// Gets or sets the <see cref="ShootingTargetObject"/>'s <see cref="ShootingTargetType"/>.
        /// </summary>
        public ShootingTargetType TargetType { get; set; } = ShootingTargetType.Sport;

        /// <summary>
        /// Gets or sets a value indicating whether shooting target is functional.
        /// <para>Example: plays CASSIE on shot</para>
        /// </summary>
        public bool IsFunctional { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="ShootingTargetObject"/>'s position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ShootingTargetObject"/>'s rotation.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ShootingTargetObject"/>' scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the <see cref="ShootingTargetObject"/>.
        /// </summary>
        public RoomType RoomType { get; set; }
    }
}
