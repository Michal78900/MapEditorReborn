namespace MapEditorReborn.API
{
    using Exiled.API.Enums;
    using System;
    using UnityEngine;

    [Serializable]
    public class RagdollSpawnPointObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RagdollSpawnPointObject"/> class.
        /// </summary>
        public RagdollSpawnPointObject()
        {
        }

        public RagdollSpawnPointObject(string name, RoleType roleType, Vector3 position, Vector3 rotation, RoomType roomType)
        {
            Name = name;
            RoleType = roleType;
            Position = position;
            Rotation = rotation;
            RoomType = roomType;
        }

        public string Name { get; private set; } = string.Empty;

        public RoleType RoleType { get; private set; } = RoleType.ClassD;

        public SerializableVector3 Position { get; private set; } = SerializableVector3.Zero;

        public SerializableVector3 Rotation { get; private set; } = SerializableVector3.Zero;

        public RoomType RoomType { get; private set; } = RoomType.Unknown;
    }
}
