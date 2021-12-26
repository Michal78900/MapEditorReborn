namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using AdminToys;
    using Exiled.API.Enums;
    using Features.Objects;
    using Mirror;

    public class LightSourceComponent : MapEditorObject
    {
        public LightSourceComponent Init(LightSourceObject lightSourceObject, bool spawn = true)
        {
            Base = lightSourceObject;

            if (TryGetComponent(out LightSourceToy lightSourceToy))
                light = lightSourceToy;

            light.NetworkMovementSmoothing = 60;

            ForcedRoomType = lightSourceObject.RoomType != RoomType.Unknown ? lightSourceObject.RoomType : FindRoom().Type;
            UpdateObject();

            if (spawn)
                NetworkServer.Spawn(gameObject);

            return this;
        }

        public LightSourceObject Base;

        public override void UpdateObject()
        {
            light.NetworkPosition = transform.position;
            light.NetworkLightColor = GetColorFromString(Base.Color);
            light.NetworkLightIntensity = Base.Intensity;
            light.NetworkLightRange = Base.Range;
            light.NetworkLightShadows = Base.Shadows;
        }

        private LightSourceToy light;
    }
}
