namespace MapEditorReborn.API
{
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Mirror;
    using UnityEngine;

    public class PrimitiveObjectComponent : MapEditorObject
    {
        public PrimitiveObjectComponent Init(PrimitiveObject primitiveObject, bool spawn = true)
        {
            Base = primitiveObject;
            primitive = GetComponent<PrimitiveObjectToy>();
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
            Vector3 netPos = primitive.NetworkPosition;
            primitive.UpdatePositionServer();
            // MovePosition(transform.position - netPos);
            primitive.NetworkPrimitiveType = Base.PrimitiveType;
            primitive.NetworkMaterialColor = GetColorFromString(Base.Color);

            if (prevScale != transform.localScale)
            {
                prevScale = transform.localScale;
                base.UpdateObject();
            }
        }

        private void MovePosition(Vector3 positionChange)
        {
            foreach (Player player in Player.List)
            {
                if (Physics.Raycast(player.Position - Vector3.up, Vector3.down, out RaycastHit hit, 1f))
                {
                    if (hit.collider.GetComponentInParent<PrimitiveObjectComponent>()?.gameObject == gameObject)
                    {
                        player.Position += positionChange * 2f;
                    }
                }
            }
        }

        private Vector3 prevScale;
        private PrimitiveObjectToy primitive;
    }
}
