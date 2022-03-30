// -----------------------------------------------------------------------
// <copyright file="TeleportSerializable.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A tool used to spawn and save Teleports to a file.
    /// </summary>
    [Serializable]
    public class TeleportSerializable : SerializableObject
    {
        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="ExitTeleporterSerializable"/>.
        /// </summary>
        public List<ExitTeleporterSerializable> ExitTeleporters { get; set; } = new List<ExitTeleporterSerializable>()
        {
            new ExitTeleporterSerializable(),
        };

        /// <summary>
        /// Gets or sets the <see cref="TeleportSerializable"/>'s teleport cooldown.
        /// </summary>
        public float TeleportCooldown { get; set; } = 10f;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="TeleportSerializable"/> can teleport in both ways (exit will behave like the entrance).
        /// </summary>
        public bool BothWayMode { get; set; } = false;

        /// <summary>
        /// Gets or sets the <see cref="Enums.TeleportFlags"/>.
        /// </summary>
        public TeleportFlags TeleportFlags { get; set; } = TeleportFlags.Player;

        /// <summary>
        /// Gets or sets the <see cref="Enums.LockOnEvent"/>.
        /// </summary>
        public LockOnEvent LockOnEvent { get; set; } = LockOnEvent.None;

        /// <summary>
        /// Gets or sets the teleport flags.
        /// </summary>
        [YamlIgnore]
        public override Vector3 Rotation { get; set; }
    }
}
