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

        /// <inheritdoc cref="ItemSpawnPointObject()"/>
        public ItemSpawnPointObject(string item, Vector3 position, Vector3 rotation, RoomType roomType, string attachmentsCode, int spawnChance, uint numberOfItems)
        {
            Item = item;
            Position = position;
            Rotation = rotation;
            RoomType = roomType;

            AttachmentsCode = attachmentsCode;
            SpawnChance = spawnChance;
            NumberOfItems = numberOfItems;
        }

        /// <summary>
        /// Gets the name of the item that will be spawned (supports CustomItems).
        /// </summary>
        public string Item { get; private set; } = "KeycardJanitor";

        /// <summary>
        /// Gets the ItemSpawnPoint's position.
        /// </summary>
        public Vector3 Position { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the ItemSpawnPoint's rotation.
        /// </summary>
        public Vector3 Rotation { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn pos and rotation of the object.
        /// </summary>
        public RoomType RoomType { get; private set; } = RoomType.Unknown;

        /// <summary>
        /// Gets the attachments of the item (if the item is a weapon).
        /// </summary>
        public string AttachmentsCode { get; private set; } = "-1";

        /// <summary>
        /// Gets the spawn chance of the item.
        /// </summary>
        public int SpawnChance { get; private set; } = 100;

        /// <summary>
        /// Gets the number of the spawned items, if the <see cref="SpawnChance"/> succeeds.
        /// </summary>
        public uint NumberOfItems { get; private set; } = 1;
    }
}
