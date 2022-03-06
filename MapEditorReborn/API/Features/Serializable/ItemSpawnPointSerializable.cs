namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A tool used to spawn and save ItemSpawnpoints to a file.
    /// </summary>
    [Serializable]
    public class ItemSpawnPointSerializable
    {
        /// <summary>
        /// Gets or sets the name of the item that will be spawned.
        /// <para><see cref="Exiled.CustomItems.API.Features.CustomItem"/> is supported.</para>
        /// </summary>
        public string Item { get; set; } = "KeycardJanitor";

        /// <summary>
        /// Gets or sets the attachments of the item.
        /// <para>This works for <see cref="Exiled.API.Features.Items.Firearm"/> only.</para>
        /// </summary>
        public string AttachmentsCode { get; set; } = "-1";

        /// <summary>
        /// Gets or sets the spawn chance of the <see cref="Exiled.API.Features.Items.Item"/>.
        /// </summary>
        public int SpawnChance { get; set; } = 100;

        /// <summary>
        /// Gets or sets the number of the spawned items, if the <see cref="SpawnChance"/> succeeds.
        /// </summary>
        public uint NumberOfItems { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether the spawned <see cref="Exiled.API.Features.Items.Item"/> should be affected by gravity.
        /// </summary>
        public bool UseGravity { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the spawned <see cref="Exiled.API.Features.Items.Item"/> can be picked up.
        /// </summary>
        public bool CanBePickedUp { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="ItemSpawnPointSerializable"/>'s position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ItemSpawnPointSerializable"/>'s rotation.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ItemSpawnPointSerializable"/>'s scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Enums.RoomType"/> which is used to determine the spawn position and rotation of the <see cref="ItemSpawnPointSerializable"/>.
        /// </summary>
        public RoomType RoomType { get; set; }
    }
}
