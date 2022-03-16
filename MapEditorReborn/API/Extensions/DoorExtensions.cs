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
            return door.Base.gameObject.name switch
            {
                "LCZ BreakableDoor(Clone)" => DoorType.LightContainmentDoor,
                "HCZ BreakableDoor(Clone)" => DoorType.HeavyContainmentDoor,
                "EZ BreakableDoor(Clone)" => DoorType.EntranceDoor,
                _ => DoorType.UnknownDoor,
            };
        }

        /// <summary>
        /// Gets the <see cref="GameObject"/> prefab given a specified <see cref="DoorType"/>.
        /// </summary>
        /// <param name="doorType">The specified <see cref="DoorType"/>.</param>
        /// <returns>The corresponding <see cref="GameObject"/>.</returns>
        public static GameObject GetDoorObjectByType(this DoorType doorType)
        {
            return doorType switch
            {
                DoorType.LightContainmentDoor => ObjectType.LczDoor.GetObjectByMode(),
                DoorType.HeavyContainmentDoor => ObjectType.HczDoor.GetObjectByMode(),
                DoorType.EntranceDoor => ObjectType.EzDoor.GetObjectByMode(),
                _ => null,
            };
        }
    }
}
