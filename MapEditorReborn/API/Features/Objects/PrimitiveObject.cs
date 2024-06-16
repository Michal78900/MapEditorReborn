// -----------------------------------------------------------------------
// <copyright file="PrimitiveObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using System.Collections.Generic;
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Features.Toys;
    using MEC;
    using Serializable;
    using UnityEngine;

    /// <summary>
    /// The component added to <see cref="PrimitiveSerializable"/>.
    /// </summary>
    public class PrimitiveObject : MapEditorObject
    {
        private Transform _transform;
        private Rigidbody? _rigidbody;
        private PrimitiveObjectToy _primitiveObjectToy;
        private Primitive? _exiledPrimitive;

        private void Awake()
        {
            _transform = transform;
            _primitiveObjectToy = GetComponent<PrimitiveObjectToy>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveObject"/> class.
        /// </summary>
        /// <param name="primitiveSerializable">The required <see cref="PrimitiveSerializable"/>.</param>
        /// <returns>The initialized <see cref="PrimitiveObject"/> instance.</returns>
        public PrimitiveObject Init(PrimitiveSerializable primitiveSerializable)
        {
            Base = primitiveSerializable;
            // Primitive.MovementSmoothing = 60;

            ForcedRoomType = primitiveSerializable.RoomType == RoomType.Unknown ? FindRoom().Type : primitiveSerializable.RoomType;

            IsStatic = true;
            base.UpdateObject();
            UpdateObject();

            return this;
        }

        public override MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new(block);
            // Primitive.MovementSmoothing = 60;

            UpdateObject();
            IsStatic = Base.Static;

            return this;
        }

        /// <summary>
        /// The base <see cref="PrimitiveSerializable"/>.
        /// </summary>
        public PrimitiveSerializable Base;

        /// <summary>
        /// Gets EXILED Primitive object.
        /// </summary>
        public Primitive Primitive => _exiledPrimitive ??= Primitive.Get(_primitiveObjectToy);

        public Rigidbody Rigidbody
        {
            get
            {
                if (_rigidbody is not null)
                    return _rigidbody;

                if (TryGetComponent(out _rigidbody))
                    return _rigidbody!;

                return _rigidbody = gameObject.AddComponent<Rigidbody>();
            }
        }

        public bool IsStatic
        {
            get => _primitiveObjectToy.NetworkIsStatic;
            set
            {
                _primitiveObjectToy.NetworkIsStatic = value;
                Primitive.MovementSmoothing = (byte)(value ? 0 : 60);
            }
        }

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            UpdateTransformProperties();
            Primitive.Type = Base.PrimitiveType;
            Primitive.Color = GetColorFromString(Base.Color);
            _primitiveObjectToy.NetworkPrimitiveFlags = Base.PrimitiveFlags;

            // if (IsSchematicBlock)
                // return;

            if (_primitiveObjectToy.NetworkIsStatic)
                Timing.RunCoroutine(RefreshStatic());
        }

        private IEnumerator<float> RefreshStatic()
        {
            _primitiveObjectToy.NetworkIsStatic = false;
            yield return Timing.WaitForOneFrame;
            _primitiveObjectToy.NetworkIsStatic = true;
        }

        private void UpdateTransformProperties()
        {
            _primitiveObjectToy.NetworkPosition = _transform.position;
            _primitiveObjectToy.NetworkRotation = new LowPrecisionQuaternion(_transform.rotation);
            _primitiveObjectToy.NetworkScale = _transform.root != _transform ? Vector3.Scale(_transform.localScale, _transform.root.localScale) : _transform.localScale;
        }
    }
}