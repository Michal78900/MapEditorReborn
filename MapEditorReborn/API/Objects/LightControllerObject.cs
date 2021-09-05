namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;

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
        public LightControllerObject(float red, float green, float blue, float alpha, bool onlyWarheadLight, RoomType roomType)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;

            OnlyWarheadLight = onlyWarheadLight;
            RoomType = roomType;
        }

        public float Red { get; private set; } = 0f;

        public float Green { get; private set; } = 0f;

        public float Blue { get; private set; } = 0f;

        public float Alpha { get; private set; } = 0f;

        public bool OnlyWarheadLight { get; private set; } = false;

        /// <summary>
        /// Gets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; private set; } = RoomType.Unknown;
    }
}
