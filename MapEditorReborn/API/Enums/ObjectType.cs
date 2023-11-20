// -----------------------------------------------------------------------
// <copyright file="ObjectType.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Enums
{
    /// <summary>
    /// All available object identifiers.
    /// </summary>
    public enum ObjectType
    {
        /// <summary>
        /// Represents Light Containment Zone door.
        /// </summary>
        LczDoor = 0,

        /// <summary>
        /// Represents Heavy Containment Zone door.
        /// </summary>
        HczDoor = 1,

        /// <summary>
        /// Represents Entrance Zone door.
        /// </summary>
        EzDoor = 2,

        /// <summary>
        /// Represents WorkStation.
        /// </summary>
        WorkStation = 3,

        /// <summary>
        /// Represents ItemSpawnPoint.
        /// </summary>
        ItemSpawnPoint = 4,

        /// <summary>
        /// Represents SportShootingTarget.
        /// </summary>
        SportShootingTarget = 5,

        /// <summary>
        /// Represents DboyShootingTarget.
        /// </summary>
        DboyShootingTarget = 6,

        /// <summary>
        /// Represents BinaryShootingTarget.
        /// </summary>
        BinaryShootingTarget = 7,

        /// <summary>
        /// Represents Primitive.
        /// </summary>
        Primitive = 8,

        /// <summary>
        /// Represents LightSource.
        /// </summary>
        LightSource = 9,

        /// <summary>
        /// Represents RoomLight.
        /// </summary>
        RoomLight = 10,

        /// <summary>
        /// Represents Teleporter.
        /// </summary>
        Teleporter = 11,

        PedestalLocker = 12,

        LargeGunLocker = 13,

        RifleRackLocker = 14,

        MiscLocker = 15,

        MedkitLocker = 16,

        AdrenalineLocker = 17
    }
}