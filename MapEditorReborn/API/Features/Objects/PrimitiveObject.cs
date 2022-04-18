// -----------------------------------------------------------------------
// <copyright file="PrimitiveObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections.Generic;
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Features.Toys;
    using Features.Serializable;
    using MEC;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// The component added to <see cref="PrimitiveSerializable"/>.
    /// </summary>
    public class PrimitiveObject : MapEditorObject
    {
        private Collider _collider;
        private MeshCollider _meshCollider;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _collider = gameObject.GetComponent<Collider>();
            _meshCollider = gameObject.GetComponent<MeshCollider>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveObject"/> class.
        /// </summary>
        /// <param name="primitiveSerializable">The required <see cref="PrimitiveSerializable"/>.</param>
        /// <param name="spawn">A value indicating whether the component should be spawned.</param>
        /// <returns>The initialized <see cref="PrimitiveObject"/> instance.</returns>
        public PrimitiveObject Init(PrimitiveSerializable primitiveSerializable)
        {
            Base = primitiveSerializable;

            if (TryGetComponent(out PrimitiveObjectToy primitiveObjectToy))
                Primitive = Primitive.Get(primitiveObjectToy);

            Primitive.MovementSmoothing = 60;
            _prevScale = transform.localScale;

            ForcedRoomType = primitiveSerializable.RoomType == RoomType.Unknown ? FindRoom().Type : primitiveSerializable.RoomType;

            UpdateObject();
            NetworkServer.Spawn(gameObject);

            return this;
        }

        public PrimitiveObject Init(SchematicBlockData block)
        {
            IsSchematicBlock = true;

            gameObject.name = block.Name;
            gameObject.transform.localPosition = block.Position;
            gameObject.transform.localEulerAngles = block.Rotation;
            gameObject.transform.localScale = block.Scale;

            Base = new PrimitiveSerializable(
                (PrimitiveType)Enum.Parse(typeof(PrimitiveType), block.Properties["PrimitiveType"].ToString()),
                block.Properties["Color"].ToString());

            if (TryGetComponent(out PrimitiveObjectToy primitiveObjectToy))
                Primitive = Primitive.Get(primitiveObjectToy);

            Primitive.MovementSmoothing = 60;

            UpdateObject();

            return this;
        }

        /// <summary>
        /// The base <see cref="PrimitiveSerializable"/>.
        /// </summary>
        public PrimitiveSerializable Base;

        public Primitive Primitive { get; private set; }

        public Rigidbody Rigidbody
        {
            get
            {
                if (_rigidbody is not null)
                    return _rigidbody;

                if (TryGetComponent(out _rigidbody))
                    return _rigidbody;

                return _rigidbody = gameObject.AddComponent<Rigidbody>();
            }
        }

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            Primitive.Base.UpdatePositionServer();
            Primitive.Type = Base.PrimitiveType;
            Primitive.Color = GetColorFromString(Base.Color);

            if (!IsSchematicBlock && _prevScale != transform.localScale)
            {
                _prevScale = transform.localScale;
                base.UpdateObject();
            }
        }

        private IEnumerator<float> Decay()
        {
            yield return Timing.WaitForSeconds(1f);

            while (Primitive.Color.a > 0)
            {
                Primitive.Color = new Color(Primitive.Color.r, Primitive.Color.g, Primitive.Color.b, Primitive.Color.a - 0.05f);
                yield return Timing.WaitForSeconds(0.1f);
            }

            Destroy();
        }

        private Vector3 _prevScale;
    }
}
