namespace MapEditorReborn.API
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Extensions;
    using Features;
    using Features.Objects;
    using Features.Serializable;
    using UnityEngine;

    /// <summary>
    /// A class which exposes all useful properties and methods to be used in other projects.
    /// </summary>
    public static class API
    {
        /// <summary>
        /// Gets or sets a random <see cref="Room"/> from the <see cref="RoomType"/>.
        /// </summary>
        /// <param name="type">The <see cref="RoomType"/> from which the room should be choosen.</param>
        /// <returns>A random <see cref="Room"/> that has <see cref="Room.Type"/> of the argument.</returns>
        public static Room GetRandomRoom(RoomType type)
        {
            if (type == RoomType.Unknown)
                return null;

            List<Room> validRooms = Map.Rooms.Where(x => x.Type == type).ToList();

            // return validRooms[Random.Range(0, validRooms.Count)];
            return validRooms.First();
        }

        /// <summary>
        /// Gets or sets a position relative to the <see cref="Room"/>.
        /// </summary>
        /// <param name="position">The object position.</param>
        /// <param name="room">The <see cref="Room"/> whose <see cref="Transform"/> will be used.</param>
        /// <returns>Global position relative to the <see cref="Room"/>. If the <see cref="Room.Type"/> is equal to <see cref="RoomType.Surface"/> the <paramref name="position"/> will be retured with no changes.</returns>
        public static Vector3 GetRelativePosition(Vector3 position, Room room) => room.Type == RoomType.Surface ? position : room.transform.TransformPoint(position);

        /// <summary>
        /// Gets or sets a rotation relative to the <see cref="Room"/>.
        /// </summary>
        /// <param name="rotation">The object rotation.</param>
        /// <param name="room">The <see cref="Room"/> whose <see cref="Transform"/> will be used.</param>
        /// <returns>Global rotation relative to the <see cref="Room"/>. If the <see cref="Room.Type"/> is equal to <see cref="RoomType.Surface"/> the <paramref name="rotation"/> will be retured with no changes.</returns>
        public static Quaternion GetRelativeRotation(Vector3 rotation, Room room)
        {
            if (rotation.x == -1f)
                rotation.x = Random.Range(0f, 360f);

            if (rotation.y == -1f)
                rotation.y = Random.Range(0f, 360f);

            if (rotation.z == -1f)
                rotation.z = Random.Range(0f, 360f);

            if (room == null)
                return Quaternion.Euler(rotation);

            return room.Type == RoomType.Surface ? Quaternion.Euler(rotation) : room.transform.rotation * Quaternion.Euler(rotation);
        }

        /// <summary>
        /// Tries to get a <see cref="Vector3"/> out of 3 strings.
        /// </summary>
        /// <param name="x">The x axis.</param>
        /// <param name="y">The y axis.</param>
        /// <param name="z">The z axis.</param>
        /// <param name="vector">The resolved <see cref="Vector3"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="Vector3"/> was successfully resolved, otherwise <see langword="false"/>.</returns>
        public static bool TryGetVector(string x, string y, string z, out Vector3 vector)
        {
            vector = Vector3.zero;

            if (!x.TryParseToFloat(out float xValue) || !y.TryParseToFloat(out float yValue) || !z.TryParseToFloat(out float zValue))
                return false;

            vector = new Vector3(xValue, yValue, zValue);
            return true;
        }

        /// <summary>
        /// Gets the readonly list of <see cref="RoomType"/> that spawned this round.
        /// </summary>
        public static ReadOnlyCollection<RoomType> SpawnedRoomTypes
        {
            get
            {
                if (_roomTypes == null)
                {
                    _roomTypes = new List<RoomType>(Map.Rooms.Select(x => x.Type)).AsReadOnly();
                    _roomTypes.Distinct();
                }

                return _roomTypes;
            }
        }

        /// <summary>
        /// Gets the name of a variable used for selecting the objects.
        /// </summary>
        public const string SelectedObjectSessionVarName = "MapEditorReborn_SelectedObject";

        /// <summary>
        /// Gets the name of a variable used for copying the objects.
        /// </summary>
        public const string CopiedObjectSessionVarName = "MapEditorReborn_CopiedObject";

        /// <summary>
        /// Gets or sets currently loaded <see cref="MapSchematic"/>.
        /// </summary>
        public static MapSchematic CurrentLoadedMap
        {
            get => _mapSchematic;
            set => MapUtils.LoadMap(value);
        }

        /// <summary>
        /// Gets the <see cref="List{T}"/> containing objects that are a part of currently loaded <see cref="MapSchematic"/>.
        /// </summary>
        public static List<MapEditorObject> SpawnedObjects { get; } = new List<MapEditorObject>();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all <see cref="ObjectType"/> and <see cref="GameObject"/> pairs.
        /// </summary>
        public static ReadOnlyDictionary<ObjectType, GameObject> ObjectPrefabs { get; internal set; }

        /// <summary>
        /// The dictionary that stores currently selected <see cref="ObjectType"/> by <see cref="InventorySystem.Items.ItemBase.ItemSerial"/>.
        /// </summary>
        internal static Dictionary<ushort, ObjectType> ToolGuns = new Dictionary<ushort, ObjectType>();

        internal static List<ushort> GravityGuns = new List<ushort>();

        /// <summary>
        /// The base schematic.
        /// </summary>
        internal static MapSchematic _mapSchematic;

        internal static ReadOnlyCollection<RoomType> _roomTypes;
    }
}
