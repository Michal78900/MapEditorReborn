// -----------------------------------------------------------------------
// <copyright file="DoorExtensions.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Extensions
{
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;
    using UnityEngine;

    /// <summary>
    /// A set of useful extensions to easily interact with doors.
    /// </summary>
    public static class DoorExtensions
    {
        /// <summary>
        /// Gets the <see cref="DoorType"/> given a specified <see cref="Door"/>.
        /// </summary>
        /// <param name="door">The <see cref="Door"/> to check.</param>
        /// <returns>The corresponding <see cref="DoorType"/> of the specified <see cref="Door"/>.</returns>
        public static DoorType GetDoorType(this Door door) => door.GameObject.name.GetDoorTypeByName();

        /// <summary>
        /// Gets the <see cref="DoorType"/> given a specified name.
        /// </summary>
        /// <param name="name">The name to check."/>.</param>
        /// <returns>The corresponding <see cref="DoorType"/>.</returns>
        public static DoorType GetDoorTypeByName(this string name) => name.Replace("(Clone)", string.Empty) switch
        {
            "LCZ BreakableDoor" => DoorType.LightContainmentDoor,
            "HCZ BreakableDoor" => DoorType.HeavyContainmentDoor,
            "EZ BreakableDoor" => DoorType.EntranceDoor,
            _ => DoorType.UnknownDoor,
        };

        /// <summary>
        /// Gets the <see cref="GameObject"/> prefab given a specified <see cref="DoorType"/>.
        /// </summary>
        /// <param name="doorType">The specified <see cref="DoorType"/>.</param>
        /// <returns>The corresponding <see cref="GameObject"/>.</returns>
        public static GameObject GetDoorObjectByType(this DoorType doorType) => doorType switch
        {
            DoorType.LightContainmentDoor => ObjectType.LczDoor.GetObjectByMode(),
            DoorType.HeavyContainmentDoor => ObjectType.HczDoor.GetObjectByMode(),
            DoorType.EntranceDoor => ObjectType.EzDoor.GetObjectByMode(),
            _ => null,
        };
    }
}
