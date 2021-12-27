namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A tool used to spawn and save Workstations to a file.
    /// </summary>
    [Serializable]
    public class WorkStationObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether the player can interact with the <see cref="WorkStationObject"/>.
        /// </summary>
        public bool IsInteractable { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="WorkStationObject"/>'s position.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the <see cref="WorkStationObject"/>'s rotation.
        /// </summary>
        public Vector3 Rotation { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the <see cref="WorkStationObject"/>' scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the <see cref="WorkStationObject"/>.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
