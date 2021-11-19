namespace MapEditorReborn.API
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Component added to spawned LightControllerObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class LightControllerComponent : MapEditorObject
    {
        /// <summary>
        /// Instantiates the <see cref="LightControllerComponent"/>.
        /// </summary>
        /// <param name="lightControllerObject">The <see cref="LightControllerObject"/> used for instantiating the object.</param>
        /// <returns>Instance of this compoment.</returns>
        public LightControllerComponent Init(LightControllerObject lightControllerObject)
        {
            Base = lightControllerObject;

            if (Base.RoomType == RoomType.Unknown)
                Base.RoomType = RoomType;

            ForcedRoomType = lightControllerObject.RoomType != RoomType.Unknown ? lightControllerObject.RoomType : FindRoom().Type;
            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public LightControllerObject Base;

        /// <summary>
        /// List of attached <see cref="FlickerableLightController"/> objects.
        /// </summary>
        public List<FlickerableLightController> LightControllers = new List<FlickerableLightController>();

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            OnDestroy();

            Color color = new Color(Base.Red / 255f, Base.Green / 255f, Base.Blue / 255f, Base.Alpha);

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
            currentColor.a = Base.Alpha;

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

            LightControllers.Clear();
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
    }
}
