namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    [Serializable]
    public class LightSourceObject
    {
        public LightSourceObject()
        {
        }

        public string Color { get; set; } = "white";

        public float Intensity { get; set; } = 1f;

        public float Range { get; set; } = 1f;

        public bool Shadows { get ; set; } = true;

        public Vector3 Position { get; set; } = Vector3.zero;

        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
