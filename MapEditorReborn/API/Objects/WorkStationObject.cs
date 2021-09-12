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
        public WorkStationObject(Vector3 position, Vector3 rotation, Vector3 scale, RoomType roomType, bool isInteractable)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            RoomType = roomType;

            IsInteractable = isInteractable;
        }

        /// <summary>
        /// Gets the workstation's position.
        /// </summary>
        public Vector3 Position { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the workstation's rotation.
        /// </summary>
        public Vector3 Rotation { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the workstation's scale.
        /// </summary>
        public Vector3 Scale { get; private set; } = Vector3.one;

        /// <summary>
        /// Gets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; private set; } = RoomType.Unknown;

        /// <summary>
        /// Gets a value indicating whether the player can interact with the workstation.
        /// </summary>
        public bool IsInteractable { get; private set; } = true;
    }
}
