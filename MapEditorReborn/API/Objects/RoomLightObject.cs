namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;

    /// <summary>
    /// Represents <see cref="Methods.LightControllerObj"/> used by the plugin to spawn and save LightControllers to a file.
    /// </summary>
    [Serializable]
    public class RoomLightObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomLightObject"/> class.
        /// </summary>
        public RoomLightObject()
        {
        }

        /// <summary>
        /// Gets or sets the RoomLight's color.
        /// </summary>
        public string Color { get; set; } = "red";

        /// <summary>
        /// Gets or sets the RoomLight's color shift speed. If set to 0, the light won't shift at all (static light).
        /// </summary>
        public float ShiftSpeed { get; set; } = 0f;

        /// <summary>
        /// Gets or sets a value indicating whether the RoomLight should only work, when the Alpha Warhead is activated.
        /// </summary>
        public bool OnlyWarheadLight { get; set; } = false;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
