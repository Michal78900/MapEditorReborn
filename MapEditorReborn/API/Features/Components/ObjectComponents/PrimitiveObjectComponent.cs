namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using AdminToys;
    using Exiled.API.Enums;
    using Features.Objects;
    using Mirror;
    using UnityEngine;

    public class PrimitiveObjectComponent : MapEditorObject
    {
        public PrimitiveObjectComponent Init(PrimitiveObject primitiveObject, bool spawn = true)
        {
            Base = primitiveObject;

            if (TryGetComponent(out PrimitiveObjectToy primitiveObjectToy))
                primitive = primitiveObjectToy;

            primitive.NetworkMovementSmoothing = 60;

            prevScale = transform.localScale;

            ForcedRoomType = primitiveObject.RoomType == RoomType.Unknown ? FindRoom().Type : primitiveObject.RoomType;
            UpdateObject();

            if (spawn)
                NetworkServer.Spawn(gameObject);

            return this;
        }

        public PrimitiveObject Base;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            primitive.UpdatePositionServer();
            primitive.NetworkPrimitiveType = Base.PrimitiveType;
            primitive.NetworkMaterialColor = GetColorFromString(Base.Color);

            if (prevScale != transform.localScale)
            {
                prevScale = transform.localScale;
                base.UpdateObject();
            }
        }

        private Vector3 prevScale;
        private PrimitiveObjectToy primitive;
    }
}
