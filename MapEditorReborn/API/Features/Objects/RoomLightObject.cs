namespace MapEditorReborn.API.Features.Objects
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Features.Serializable;
    using UnityEngine;

    /// <summary>
    /// Component added to spawned LightControllerObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class RoomLightObject : MapEditorObject
    {
        /// <summary>
        /// Instantiates the <see cref="RoomLightObject"/>.
        /// </summary>
        /// <param name="lightControllerSerializable">The <see cref="RoomLightSerializable"/> used for instantiating the object.</param>
        /// <returns>Instance of this compoment.</returns>
        public RoomLightObject Init(RoomLightSerializable lightControllerSerializable)
        {
            Base = lightControllerSerializable;

            if (Base.RoomType == RoomType.Unknown)
                Base.RoomType = RoomType;

            ForcedRoomType = lightControllerSerializable.RoomType != RoomType.Unknown ? lightControllerSerializable.RoomType : FindRoom().Type;
            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public RoomLightSerializable Base;

        /// <summary>
        /// List of attached <see cref="FlickerableLightController"/> objects.
        /// </summary>
        public readonly List<FlickerableLightController> LightControllers = new List<FlickerableLightController>();

        /// <inheritdoc cref="MapEditorObject.IsRotatable"/>
        public override bool IsRotatable => false;

        /// <inheritdoc cref="MapEditorObject.IsScalable"/>
        public override bool IsScalable => false;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            OnDestroy();

            Color color = GetColorFromString(Base.Color);

            foreach (Room room in Map.Rooms.Where(x => x.Type == ForcedRoomType))
            {
                FlickerableLightController lightController = null;

                if (ForcedRoomType != RoomType.Surface)
                {
                    lightController = room.GetComponentInChildren<FlickerableLightController>();
                }
                else
                {
                    lightController = FindObjectsOfType<FlickerableLightController>().First(x => Map.FindParentRoom(x.gameObject).Type == RoomType.Surface);
                }

                if (lightController != null)
                {
                    LightControllers.Add(lightController);

                    lightController.Network_warheadLightColor = color;
                    lightController.Network_lightIntensityMultiplier = color.a;
                    lightController.Network_warheadLightOverride = !Base.OnlyWarheadLight;
                }
            }

            currentColor = color;
        }

        private void Update()
        {
            if (Base.ShiftSpeed == 0f)
                return;

            currentColor = ShiftHueBy(currentColor, Base.ShiftSpeed * Time.deltaTime);
            currentColor.a = GetColorFromString(Base.Color).a;

            foreach (FlickerableLightController lightController in LightControllers)
            {
                lightController.Network_warheadLightColor = currentColor;
                lightController.Network_lightIntensityMultiplier = currentColor.a;
            }
        }

        private void OnDestroy()
        {
            foreach (FlickerableLightController lightController in LightControllers)
            {
                lightController.Network_warheadLightColor = FlickerableLightController.DefaultWarheadColor;
                lightController.Network_lightIntensityMultiplier = 1f;
                lightController.Network_warheadLightOverride = false;
            }
        }

        // Credits to Killers0992
        private Color ShiftHueBy(Color color, float amount)
        {
            // convert from RGB to HSV
            Color.RGBToHSV(color, out float hue, out float saturation, out float value);

            // shift hue by amount
            hue += amount;

            // convert back to RGB and return the color
            return Color.HSVToRGB(hue, saturation, value);
        }

        private Color currentColor;

        /// <summary>
        /// Registers positions of <see cref="FlickerableLightController"/>s on the map.
        /// </summary>
        internal static void RegisterFlickerableLights()
        {
            FlickerableLightsPositions.Clear();

            foreach (FlickerableLight light in FindObjectsOfType<FlickerableLight>())
            {
                FlickerableLightsPositions.Add(light.transform.position);
            }
        }

        /// <summary>
        /// Positions of <see cref="FlickerableLightController"/>s on the map.
        /// </summary>
        internal static readonly List<Vector3> FlickerableLightsPositions = new List<Vector3>();
    }
}
