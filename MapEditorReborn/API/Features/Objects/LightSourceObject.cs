namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A tool used to easily handle light sources.
    /// </summary>
    [Serializable]
    public class LightSourceObject
    {
        /// <summary>
        /// Gets or sets the <see cref="LightSourceObject"/>'s color.
        /// </summary>
        public string Color { get; set; } = "white";

        /// <summary>
        /// Gets or sets the <see cref="LightSourceObject"/>'s intensity.
        /// </summary>
        public float Intensity { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the <see cref="LightSourceObject"/>'s range.
        /// </summary>
        public float Range { get; set; } = 1f;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="LightSourceObject"/>'s shadows are enabled.
        /// </summary>
        public bool Shadows { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="LightSourceObject"/>'s position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the <see cref="LightSourceObject"/>.
        /// </summary>
        public RoomType RoomType { get; set; }
    }
}
