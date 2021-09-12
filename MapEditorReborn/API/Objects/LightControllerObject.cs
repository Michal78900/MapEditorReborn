namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

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

        /// <inheritdoc cref="LightControllerObject()"/>
        public LightControllerObject(Color color, float shiftSpeed, bool onlyWarheadLight, RoomType roomType)
        {
            Red = color.r;
            Green = color.g;
            Blue = color.b;
            Alpha = color.a;

            ShiftSpeed = shiftSpeed;

            OnlyWarheadLight = onlyWarheadLight;
            RoomType = roomType;
        }

        /// <summary>
        /// Gets the LightController's red color.
        /// </summary>
        public float Red { get; private set; } = 0f;

        /// <summary>
        /// Gets the LightController's green color.
        /// </summary>
        public float Green { get; private set; } = 0f;

        /// <summary>
        /// Gets the LightController's blue color.
        /// </summary>
        public float Blue { get; private set; } = 0f;

        /// <summary>
        /// Gets the LightController's alpha.
        /// </summary>
        public float Alpha { get; private set; } = 0f;

        /// <summary>
        /// Gets the LightController's color shift speed. If set to 0, the light won't shift at all (static light).
        /// </summary>
        public float ShiftSpeed { get; private set; } = 0f;

        /// <summary>
        /// Gets a value indicating whether the LightController should only work, when the Alpha Warhead is activated.
        /// </summary>
        public bool OnlyWarheadLight { get; private set; } = false;

        /// <summary>
        /// Gets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; private set; } = RoomType.Unknown;
    }
}
