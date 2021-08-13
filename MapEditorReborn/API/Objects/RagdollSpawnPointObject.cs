namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
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

        /// <inheritdoc cref="RagdollSpawnPointObject()"/>
        public RagdollSpawnPointObject(string name, RoleType roleType, string damageType, Vector3 position, Vector3 rotation, RoomType roomType)
        {
            Name = name;
            RoleType = roleType;
            DamageType = damageType;
            Position = position;
            Rotation = rotation;
            RoomType = roomType;
        }

        public string Name { get; private set; } = string.Empty;

        public RoleType RoleType { get; private set; } = RoleType.ClassD;

        public string DamageType { get; private set; } = "None";

        public Vector3 Position { get; private set; } = Vector3.zero;

        public Vector3 Rotation { get; private set; } = Vector3.zero;

        public RoomType RoomType { get; private set; } = RoomType.Unknown;
    }
}
