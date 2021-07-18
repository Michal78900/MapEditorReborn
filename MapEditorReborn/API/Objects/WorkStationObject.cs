namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="WorkStation"/> used by the plugin to spawn and save workstations to a file.
    /// </summary>
    [Serializable]
    public class WorkStationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkStationObject"/> class.
        /// </summary>
        public WorkStationObject()
        {
        }

        /// <inheritdoc cref="WorkStationObject()"/>
        public WorkStationObject(Vector3 position, Vector3 rotation, Vector3 scale, RoomType roomType)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            RoomType = roomType;
        }

        /// <summary>
        /// Gets or sets the workstation's position.
        /// </summary>
        // public Vector3 Position { get; set; } = Vector3.zero;
        public SerializableVector3 Position { get; set; } = SerializableVector3.Zero;

        /// <summary>
        /// Gets or sets the workstation's rotation.
        /// </summary>
        // public Vector3 Rotation { get; set; } = Vector3.zero;
        public SerializableVector3 Rotation { get; set; } = SerializableVector3.Zero;

        /// <summary>
        /// Gets or sets the workstation's scale.
        /// </summary>
        // public Vector3 Scale { get; set; } = Vector3.one;
        public SerializableVector3 Scale { get; set; } = SerializableVector3.One;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
