namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using AdminToys;
    using Exiled.API.Enums;
    // using Exiled.API.Features.Toys;
    using Features.Objects;
    using Mirror;

    /// <summary>
    /// The component added to <see cref="LightSourceObject"/>.
    /// </summary>
    public class LightSourceComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightSourceComponent"/> class.
        /// </summary>
        /// <param name="lightSourceObject">The required <see cref="LightSourceObject"/>.</param>
        /// <param name="spawn">A value indicating whether the component should be spawned.</param>
        /// <returns>The initialized <see cref="LightSourceComponent"/> instance.</returns>
        public LightSourceComponent Init(LightSourceObject lightSourceObject, bool spawn = true)
        {
            Base = lightSourceObject;

            // if (TryGetComponent(out LightSourceToy lightSourceToy))
            // light = Light.Get(lightSourceToy);

            // light.MovementSmoothing = 60;

            if (TryGetComponent(out LightSourceToy lightSourceToy))
                light = lightSourceToy;

            light.NetworkMovementSmoothing = 60;

            ForcedRoomType = lightSourceObject.RoomType != RoomType.Unknown ? lightSourceObject.RoomType : FindRoom().Type;
            UpdateObject();

            if (spawn)
                NetworkServer.Spawn(gameObject);

            return this;
        }

        /// <summary>
        /// The base <see cref="LightSourceObject"/>.
        /// </summary>
        public LightSourceObject Base;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            // light.Position = transform.position;
            // light.Color = GetColorFromString(Base.Color);
            // light.Intensity = Base.Intensity;
            // light.Range = Base.Range;
            // light.ShadowEmission = Base.Shadows;

            light.NetworkPosition = transform.position;
            light.NetworkLightColor = GetColorFromString(Base.Color);
            light.NetworkLightIntensity = Base.Intensity;
            light.NetworkLightRange = Base.Range;
            light.NetworkLightShadows = Base.Shadows;
        }

        // private Light light;

        private LightSourceToy light;
    }
}
