// -----------------------------------------------------------------------
// <copyright file="LightSourceObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using AdminToys;
    using Exiled.API.Enums;
    using Mirror;
    using Serializable;
    using UnityEngine;
    using Light = Exiled.API.Features.Toys.Light;

    /// <summary>
    /// The component added to <see cref="LightSourceSerializable"/>.
    /// </summary>
    public class LightSourceObject : MapEditorObject
    {
        private Transform _transform;
        private LightSourceToy _lightSourceToy;
        private Light _exiledLight;

        private void Awake()
        {
            _transform = transform;
            _lightSourceToy = GetComponent<LightSourceToy>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightSourceObject"/> class.
        /// </summary>
        /// <param name="lightSourceSerializable">The required <see cref="LightSourceSerializable"/>.</param>
        /// <param name="spawn">A value indicating whether the component should be spawned.</param>
        /// <returns>The initialized <see cref="LightSourceObject"/> instance.</returns>
        public LightSourceObject Init(LightSourceSerializable lightSourceSerializable, bool spawn = true)
        {
            Base = lightSourceSerializable;
            Light.MovementSmoothing = 60;

            ForcedRoomType = lightSourceSerializable.RoomType != RoomType.Unknown ? lightSourceSerializable.RoomType : FindRoom().Type;
            UpdateObject();

            if (spawn)
                NetworkServer.Spawn(gameObject);

            // IsStatic = false;

            return this;
        }

        public override MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new(block);
            Light.MovementSmoothing = 60;

            UpdateObject();
            // IsStatic = true;
            // _lightSourceToy.enabled = false;

            return this;
        }

        /// <summary>
        /// The base <see cref="LightSourceSerializable"/>.
        /// </summary>
        public LightSourceSerializable Base;

        /// <summary>
        /// Gets EXILED Light object
        /// </summary>
        public Light Light
        {
            get
            {
                if (_exiledLight is null)
                    _exiledLight = Light.Get(_lightSourceToy);

                return _exiledLight;
            }
        }

        /*
        public bool IsStatic
        {
            get => _isStatic;
            set
            {
                _lightSourceToy.enabled = !value;
                Light.MovementSmoothing = (byte)(value ? 0 : 60);
                _isStatic = value;
            }
        }
        */

        /// <inheritdoc cref="MapEditorObject.IsRotatable"/>
        public override bool IsRotatable => false;

        /// <inheritdoc cref="MapEditorObject.IsScalable"/>
        public override bool IsScalable => false;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            if (!IsSchematicBlock)
            {
                Light.Position = _transform.position;
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
                Light.Base._light.shadows = Base.Shadows ? LightShadows.Soft : LightShadows.None;
            }

            UpdateTransformProperties();
        }

        /*
        private void LateUpdate()
        {
            if (IsSchematicBlock)
                UpdateTransformProperties();
        }
        */

        private void UpdateTransformProperties()
        {
            _lightSourceToy.NetworkPosition = _transform.position;
        }

        private bool _isStatic;
    }
}