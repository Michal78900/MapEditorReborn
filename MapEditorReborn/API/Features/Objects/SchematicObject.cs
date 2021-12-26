namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    [Serializable]
    public class SchematicObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchematicObject"/> class.
        /// </summary>
        public SchematicObject()
        {
        }

        public SchematicObject(string schematicName)
        {
            SchematicName = schematicName;
        }

        public string SchematicName { get; set; } = "None";

        public Vector3 Position { get; set; } = Vector3.zero;

        public Vector3 Rotation { get; set; } = Vector3.zero;

        public Vector3 Scale { get; set; } = Vector3.one;

        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
