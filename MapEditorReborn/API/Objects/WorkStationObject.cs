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

        /// <summary>
        /// Gets or sets the workstation's position.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the workstation's rotation.
        /// </summary>
        public Vector3 Rotation { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the workstation's scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;

        /// <summary>
        /// Gets or sets a value indicating whether the player can interact with the workstation.
        /// </summary>
        public bool IsInteractable { get; set; } = true;
    }
}
