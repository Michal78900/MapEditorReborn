namespace MapEditorReborn.API.Extensions
{
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using UnityEngine;

    public static class DoorExtensions
    {
        /// <summary>
        /// Gets the <see cref="DoorType"/> given a specified <see cref="Door"/>.
        /// </summary>
        /// <param name="door">The <see cref="Door"/> to check."/>.</param>
        /// <returns>The corresponding <see cref="DoorType"/> of the specified <see cref="Door"/>.</returns>
        public static DoorType GetDoorTypeByName(this Door door)
        {
            switch (door.Base.gameObject.name)
            {
                case "LCZ BreakableDoor(Clone)":
                    return DoorType.LightContainmentDoor;

                case "HCZ BreakableDoor(Clone)":
                    return DoorType.HeavyContainmentDoor;

                case "EZ BreakableDoor(Clone)":
                    return DoorType.EntranceDoor;

                default:
                    return DoorType.UnknownDoor;
            }
        }

        /// <summary>
        /// Gets the <see cref="GameObject"/> prefab given a specified <see cref="DoorType"/>.
        /// </summary>
        /// <param name="doorType">The specified <see cref="DoorType"/>.</param>
        /// <returns>The corresponding <see cref="GameObject"/>.</returns>
        public static GameObject GetDoorObjectByType(this DoorType doorType)
        {
            switch (doorType)
            {
                case DoorType.LightContainmentDoor:
                    return ObjectType.LczDoor.GetObjectByMode();

                case DoorType.HeavyContainmentDoor:
                    return ObjectType.HczDoor.GetObjectByMode();

                case DoorType.EntranceDoor:
                    return ObjectType.EzDoor.GetObjectByMode();

                default:
                    return null;
            }
        }
    }
}
