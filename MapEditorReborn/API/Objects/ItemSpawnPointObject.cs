namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="Handler.ItemSpawnPointObj"/> used by the plugin to spawn and save ItemSpawnPoint to a file.
    /// </summary>
    [Serializable]
    public class ItemSpawnPointObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSpawnPointObject"/> class.
        /// </summary>
        public ItemSpawnPointObject()
        {
        }

        /// <inheritdoc cref="ItemSpawnPointObject()"/>
        public ItemSpawnPointObject(string item, Vector3 position, Vector3 rotation, RoomType roomType)
        {
            Item = item;
            Position = position;
            Rotation = rotation;
            RoomType = roomType;
        }

        /// <summary>
        /// Gets or sets the name of the item that will be spawned (supports CustomItems).
        /// </summary>
        public string Item { get; set; } = "KeycardJanitor";

        /// <summary>
        /// Gets or sets the ItemSpawnPoint's position.
        /// </summary>
        // public Vector3 Position { get; set; } = Vector3.zero;
        public SerializableVector3 Position { get; set; } = SerializableVector3.Zero;

        /// <summary>
        /// Gets or sets the ItemSpawnPoint's rotation.
        /// </summary>
        // public Vector3 Rotation { get; set; } = Vector3.zero;
        public SerializableVector3 Rotation { get; set; } = SerializableVector3.Zero;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;

        /// <summary>
        /// Gets or sets the spawn chance of the item.
        /// </summary>
        public int SpawnChance { get; set; } = 100;
    }
}
