namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Features.Toys;
    using Features.Serializable;
    using Mirror;
    using PlayerStatsSystem;
    using UnityEngine;

    /// <summary>
    /// The component added to <see cref="PrimitiveSerializable"/>.
    /// </summary>
    public class PrimitiveObject : MapEditorObject, IDestructible
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveObject"/> class.
        /// </summary>
        /// <param name="primitiveSerializable">The required <see cref="PrimitiveSerializable"/>.</param>
        /// <param name="spawn">A value indicating whether the component should be spawned.</param>
        /// <returns>The initialized <see cref="PrimitiveObject"/> instance.</returns>
        public PrimitiveObject Init(PrimitiveSerializable primitiveSerializable, bool spawn = true)
        {
            Base = primitiveSerializable;

            if (TryGetComponent(out PrimitiveObjectToy primitiveObjectToy))
                Primitive = Primitive.Get(primitiveObjectToy);

            Primitive.MovementSmoothing = 60;

            _prevScale = transform.localScale;

            ForcedRoomType = primitiveSerializable.RoomType == RoomType.Unknown ? FindRoom().Type : primitiveSerializable.RoomType;
            UpdateObject();

            if (spawn)
                NetworkServer.Spawn(gameObject);

            gameObject.AddComponent<BoxCollider>().size = Vector3.zero;

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

            UpdateObject();

            return this;
        }

        /// <summary>
        /// The base <see cref="PrimitiveSerializable"/>.
        /// </summary>
        public PrimitiveSerializable Base;

        public Primitive Primitive { get; private set; }

        /// <inheritdoc cref="IDestructible.NetworkId"/>
        public uint NetworkId => Primitive.Base.netId;

        /// <inheritdoc cref="IDestructible.CenterOfMass"/>
        public Vector3 CenterOfMass => transform.position;

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

        public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos)
        {
            return true;
        }

        private float _reamainingHp = 100;

        private Vector3 _prevScale;
    }
}
