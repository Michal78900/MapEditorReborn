namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Extensions;
    using Features.Serializable;
    using Interactables.Interobjects;
    using MEC;
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
            {
                // Primitive = Primitive.Get(primitiveObjectToy);
                Primitive = primitiveObjectToy;
                Rigidbody = gameObject.AddComponent<Rigidbody>();
                Rigidbody.isKinematic = true;
            }

            Primitive.MovementSmoothing = 60;
            _prevScale = transform.localScale;

            gameObject.AddComponent<BoxCollider>().size = Vector3.zero;

            ForcedRoomType = primitiveSerializable.RoomType == RoomType.Unknown ? FindRoom().Type : primitiveSerializable.RoomType;
            UpdateObject();

            if (spawn)
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
                block.Properties["Color"].ToString(), -1f);

            if (TryGetComponent(out PrimitiveObjectToy primitiveObjectToy))
            {
                // Primitive = Primitive.Get(primitiveObjectToy);
                Primitive = primitiveObjectToy;
                Rigidbody = gameObject.AddComponent<Rigidbody>();
                Rigidbody.isKinematic = true;
            }

            UpdateObject();

            return this;
        }

        /// <summary>
        /// The base <see cref="PrimitiveSerializable"/>.
        /// </summary>
        public PrimitiveSerializable Base;

        public PrimitiveObjectToy Primitive { get; private set; }

        public Rigidbody Rigidbody { get; private set; }

        /// <inheritdoc cref="IDestructible.NetworkId"/>
        public uint NetworkId => Primitive.netId;

        /// <inheritdoc cref="IDestructible.CenterOfMass"/>
        public Vector3 CenterOfMass => transform.position;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            Primitive.UpdatePositionServer();
            Primitive.NetworkPrimitiveType = Base.PrimitiveType;
            Primitive.NetworkMaterialColor = GetColorFromString(Base.Color);

            Rigidbody.isKinematic = true;
            _reamainingHp = Base.Health;
            _destroyed = false;

            if (!IsSchematicBlock && _prevScale != transform.localScale)
            {
                _prevScale = transform.localScale;
                base.UpdateObject();
            }
        }

        public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos)
        {
            if (_reamainingHp == -1f)
                return false;

            _reamainingHp -= damage;

            if (_reamainingHp <= 0f)
            {
                if (!_destroyed)
                {
                    Rigidbody.isKinematic = false;
                    _destroyed = true;
                    var door = Instantiate(Enums.ObjectType.LczDoor.GetObjectByMode().GetComponent<BreakableDoor>());
                    door.transform.localScale = Vector3.zero;
                    door.transform.position = transform.position + Vector3.down;
                    NetworkServer.Spawn(door.gameObject);
                    door.Network_destroyed = true;

                    Timing.CallDelayed(0.25f, () => NetworkServer.Destroy(door.gameObject));
                }

                if (handler is AttackerDamageHandler attacker)
                    Rigidbody.AddForceAtPosition(Player.Get(attacker.Attacker.Hub).CameraTransform.forward * damage * 10f, exactHitPos);
            }

            return true;
        }

        private IEnumerator<float> Decay()
        {
            yield return Timing.WaitForSeconds(1f);

            while (Primitive.NetworkMaterialColor.a > 0)
            {
                Primitive.NetworkMaterialColor = new Color(Primitive.NetworkMaterialColor.r, Primitive.NetworkMaterialColor.g, Primitive.NetworkMaterialColor.b, Primitive.NetworkMaterialColor.a - 0.05f);
                yield return Timing.WaitForSeconds(0.1f);
            }

            Destroy();
        }

        private float _reamainingHp;
        private bool _destroyed;
        private Vector3 _prevScale;
    }
}
