namespace MapEditorReborn.API
{
    using AdminToys;
    using Exiled.API.Enums;
    using Mirror;

    public class PrimitiveObjectComponent : MapEditorObject
    {
        public PrimitiveObjectComponent Init(PrimitiveObject primitiveObject)
        {
            Base = primitiveObject;
            primitive = GetComponent<PrimitiveObjectToy>();

            ForcedRoomType = primitiveObject.RoomType == RoomType.Unknown ? FindRoom().Type : primitiveObject.RoomType;
            NetworkServer.Spawn(gameObject);
            UpdateObject();

            return this;
        }

        public PrimitiveObject Base;

        public override void UpdateObject()
        {
            if (primitive.Scale != transform.localScale)
                base.UpdateObject();

            primitive.UpdatePositionServer();
            primitive.NetworkPrimitiveType = Base.PrimitiveType;
            primitive.NetworkMaterialColor = GetColorFromString(Base.Color);
        }

        private PrimitiveObjectToy primitive;
    }
}
