namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using Exiled.API.Enums;

    /// <summary>
    /// A tool used to spawn and save LightControllers to a file.
    /// </summary>
    [Serializable]
    public class RoomLightObject
    {
        /// <summary>
        /// Gets or sets the <see cref="RoomLightObject"/>'s color.
        /// </summary>
        public string Color { get; set; } = "red";

        /// <summary>
        /// Gets or sets the <see cref="RoomLightObject"/>'s color shift speed.
        /// <para>If set to 0, the light won't shift at all (static light).</para>
        /// </summary>
        public float ShiftSpeed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RoomLightObject"/> should only work.
        /// <para>This applies when the Alpha Warhead is activated only.</para>
        /// </summary>
        public bool OnlyWarheadLight { get; set; } = false;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the <see cref="RoomLightObject"/>.
        /// </summary>
        public RoomType RoomType { get; set; }
    }
}
