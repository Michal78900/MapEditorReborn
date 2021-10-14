namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;

    /// <summary>
    /// Represents <see cref="Handler.LightControllerObj"/> used by the plugin to spawn and save LightControllers to a file.
    /// </summary>
    [Serializable]
    public class LightControllerObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightControllerObject"/> class.
        /// </summary>
        public LightControllerObject()
        {
        }

        /// <summary>
        /// Gets or sets the LightController's red color.
        /// </summary>
        public float Red { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the LightController's green color.
        /// </summary>
        public float Green { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the LightController's blue color.
        /// </summary>
        public float Blue { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the LightController's alpha.
        /// </summary>
        public float Alpha { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the LightController's color shift speed. If set to 0, the light won't shift at all (static light).
        /// </summary>
        public float ShiftSpeed { get; set; } = 0f;

        /// <summary>
        /// Gets or sets a value indicating whether the LightController should only work, when the Alpha Warhead is activated.
        /// </summary>
        public bool OnlyWarheadLight { get; set; } = false;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
