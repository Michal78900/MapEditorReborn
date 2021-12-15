namespace MapEditorReborn.API
{
    using AdminToys;
    using Exiled.API.Enums;
    using Mirror;
    using UnityEngine;

    public class LightSourceComponent : MapEditorObject
    {
        public LightSourceComponent Init(LightSourceObject lightSourceObject, bool spawn = true)
        {
            Base = lightSourceObject;
            light = GetComponent<LightSourceToy>();
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
