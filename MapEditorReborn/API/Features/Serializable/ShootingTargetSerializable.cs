namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using Exiled.API.Enums;

    /// <summary>
    /// A tool used to spawn and save ShootingTargets to a file.
    /// </summary>
    [Serializable]
    public class ShootingTargetSerializable : SerializableObject
    {
        /// <summary>
        /// Gets or sets the <see cref="ShootingTargetSerializable"/>'s <see cref="ShootingTargetType"/>.
        /// </summary>
        public ShootingTargetType TargetType { get; set; } = ShootingTargetType.Sport;

        /// <summary>
        /// Gets or sets a value indicating whether shooting target is functional.
        /// <para>Example: plays CASSIE on shot.</para>
        /// </summary>
        public bool IsFunctional { get; set; } = true;
    }
}
