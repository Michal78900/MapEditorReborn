namespace MapEditorReborn.API.Features.Serializable
{
    using Exiled.API.Enums;
    using UnityEngine;

    public abstract class SerializableObject
    {
        /// <summary>
        /// Gets or sets the objects's position.
        /// </summary>
        public virtual Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the objects's rotation.
        /// </summary>
        public virtual Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the objects's scale.
        /// </summary>
        public virtual Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the object.
        /// </summary>
        public virtual RoomType RoomType { get; set; } = RoomType.Unknown;
    }
}
