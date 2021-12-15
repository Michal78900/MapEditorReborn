namespace MapEditorReborn.API
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using UnityEngine;
    using YamlDotNet.Serialization;

    [Serializable]
    public class PrimitiveObject
    {
        public PrimitiveObject()
        {
        }

        public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Cube;

        public string Color { get; set; } = "red";

        public Vector3 Position { get; set; } = Vector3.zero;

        public Vector3 Rotation { get; set; } = Vector3.zero;

        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;

        public List<AnimationFrame> AnimationFrames = new List<AnimationFrame>();
    }
}
