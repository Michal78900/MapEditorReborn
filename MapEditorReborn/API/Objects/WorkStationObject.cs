namespace MapEditorReborn.API
{
    using System;
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
        public WorkStationObject(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
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
    }
}
