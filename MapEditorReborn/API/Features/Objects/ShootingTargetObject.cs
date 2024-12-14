// -----------------------------------------------------------------------
// <copyright file="ShootingTargetObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Features.Toys;
    using Serializable;
    using UnityEngine;
    using static API;

    /// <summary>
    /// Component added to a ShootingTargetObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class ShootingTargetObject : MapEditorObject
    {
        private Transform _transform;
        private ShootingTarget _shootingTarget;
        private ShootingTargetToy _exiledShootingTargetToy;
        private ShootingTargetType _prevType;

        private void Awake()
        {
            _transform = transform;
            _shootingTarget = GetComponent<ShootingTarget>();
        }

        /// <summary>
        /// Initializes the <see cref="ShootingTargetObject"/>.
        /// </summary>
        /// <param name="shootingTargetSerializable">The <see cref="ShootingTargetSerializable"/> to instantiate.</param>
        /// <returns>Instance of this component.</returns>
        public ShootingTargetObject Init(ShootingTargetSerializable shootingTargetSerializable)
        {
            Base = shootingTargetSerializable;

            ShootingTargetToy.MovementSmoothing = 60;
            Base.TargetType = ShootingTargetToy.Type;
            _prevType = ShootingTargetToy.Type;

            ForcedRoomType = shootingTargetSerializable.RoomType != RoomType.Unknown ? shootingTargetSerializable.RoomType : FindRoom().Type;
            UpdateObject();
            _shootingTarget.enabled = false;

            return this;
        }

        /// <summary>s
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public ShootingTargetSerializable Base;

        /// <summary>
        /// Gets EXILED ShootingTargetToy object.
        /// </summary>
        public ShootingTargetToy ShootingTargetToy => _exiledShootingTargetToy ??= ShootingTargetToy.Get(_shootingTarget);

        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject()
        {
            if (_prevType != Base.TargetType)
            {
                SpawnedObjects[SpawnedObjects.IndexOf(this)] = ObjectSpawner.SpawnShootingTarget(Base, Position, Rotation);
                ShootingTargetToy.Destroy();
                return;
            }

            _prevType = Base.TargetType;

            UpdateTransformProperties();
        }

        private void UpdateTransformProperties()
        {
            _shootingTarget.NetworkPosition = _transform.position;
            _shootingTarget.NetworkRotation = _transform.rotation;
            _shootingTarget.NetworkScale = _transform.root != _transform ? Vector3.Scale(_transform.localScale, _transform.root.localScale) : _transform.localScale;
            base.UpdateObject();
        }
    }
}