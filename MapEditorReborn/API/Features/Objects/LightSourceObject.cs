namespace MapEditorReborn.API.Features.Objects
{
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Features.Toys;
    using Features.Serializable;
    using Mirror;

    /// <summary>
    /// The component added to <see cref="LightSourceSerializable"/>.
    /// </summary>
    public class LightSourceObject : MapEditorObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightSourceObject"/> class.
        /// </summary>
        /// <param name="lightSourceSerializable">The required <see cref="LightSourceSerializable"/>.</param>
        /// <param name="spawn">A value indicating whether the component should be spawned.</param>
        /// <returns>The initialized <see cref="LightSourceObject"/> instance.</returns>
        public LightSourceObject Init(LightSourceSerializable lightSourceSerializable, bool spawn = true)
        {
            Base = lightSourceSerializable;

            if (TryGetComponent(out LightSourceToy lightSourceToy))
                Light = Light.Get(lightSourceToy);

            Light.MovementSmoothing = 60;

            ForcedRoomType = lightSourceSerializable.RoomType != RoomType.Unknown ? lightSourceSerializable.RoomType : FindRoom().Type;
            UpdateObject();

            if (spawn)
                NetworkServer.Spawn(gameObject);

            return this;
        }

        public LightSourceObject Init(SchematicBlockData block)
        {
            IsSchematicBlock = true;

            Base = new LightSourceSerializable(block.Properties["Color"].ToString(), float.Parse(block.Properties["Intensity"].ToString()), float.Parse(block.Properties["Range"].ToString()), bool.Parse(block.Properties["Shadows"].ToString()));

            if (TryGetComponent(out LightSourceToy lightSourceToy))
                Light = Light.Get(lightSourceToy);

            gameObject.name = block.Name;
            gameObject.transform.localPosition = block.Position;

            UpdateObject();

            return this;
        }

        /// <summary>
        /// The base <see cref="LightSourceSerializable"/>.
        /// </summary>
        public LightSourceSerializable Base;

        public Light Light { get; private set; }

        /// <inheritdoc cref="MapEditorObject.IsRotatable"/>
        public override bool IsRotatable => false;

        /// <inheritdoc cref="MapEditorObject.IsScalable"/>
        public override bool IsScalable => false;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            if (!IsSchematicBlock)
            {
                Light.Position = transform.position;
                Light.Color = GetColorFromString(Base.Color);
                Light.Intensity = Base.Intensity;
                Light.Range = Base.Range;
                Light.ShadowEmission = Base.Shadows;
            }
            else
            {
                Light.Base._light.color = GetColorFromString(Base.Color);
                Light.Base._light.intensity = Base.Intensity;
                Light.Base._light.range = Base.Range;
                Light.Base._light.shadows = Base.Shadows ? UnityEngine.LightShadows.Soft : UnityEngine.LightShadows.None;
            }

            Light.Base.UpdatePositionServer();
        }
    }
}
