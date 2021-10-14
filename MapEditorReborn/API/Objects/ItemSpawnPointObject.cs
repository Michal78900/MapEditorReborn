namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// Represents <see cref="Handler.ItemSpawnPointObj"/> used by the plugin to spawn and save ItemSpawnPoints to a file.
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

        /// <summary>
        /// Gets or sets the name of the item that will be spawned (supports CustomItems).
        /// </summary>
        public string Item { get; set; } = "KeycardJanitor";

        /// <summary>
        /// Gets or sets the ItemSpawnPoint's position.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the ItemSpawnPoint's rotation.
        /// </summary>
        public Vector3 Rotation { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the ItemSpawnPoint's scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; set; } = RoomType.Unknown;

        /// <summary>
        /// Gets or sets the attachments of the item (if the item is a weapon).
        /// </summary>
        public string AttachmentsCode { get; set; } = "-1";

        /// <summary>
        /// Gets or sets the spawn chance of the item.
        /// </summary>
        public int SpawnChance { get; set; } = 100;

        /// <summary>
        /// Gets or sets the number of the spawned items, if the <see cref="SpawnChance"/> succeeds.
        /// </summary>
        public uint NumberOfItems { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether spawned item should be affected by gravity..
        /// </summary>
        public bool UseGravity { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether spawned item can be picked up.
        /// </summary>
        public bool CanBePickedUp { get; set; } = true;
    }
}
