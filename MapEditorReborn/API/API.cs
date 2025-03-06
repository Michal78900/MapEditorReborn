// -----------------------------------------------------------------------
// <copyright file="API.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using AdminToys;
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Extensions;
    using Features;
    using Features.Objects;
    using Features.Serializable;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// A class which exposes all useful properties and methods to be used in other projects.
    /// </summary>
    public static class API
	{
		/// <summary>
		/// Gets the name of a variable used for selecting the objects.
		/// </summary>
		public const string SelectedObjectSessionVarName = "MapEditorReborn_SelectedObject";

		/// <summary>
		/// Gets the name of a variable used for copying the objects.
		/// </summary>
		public const string CopiedObjectSessionVarName = "MapEditorReborn_CopiedObject";

		/// <summary>
		/// The base schematic.
		/// </summary>
		internal static MapSchematic? MapSchematic;

		/// <summary>
		/// Gets or sets currently loaded <see cref="MapSchematic"/>.
		/// </summary>
		public static MapSchematic? CurrentLoadedMap
		{
			get => MapSchematic;
			set => MapUtils.LoadMap(value);
		}

		private static ReadOnlyCollection<RoomType>? _spawnedRoomTypes;
		private static ReadOnlyCollection<RoomType>? _nonDistinctRoomTypes;

		/// <summary>
		/// Gets the readonly list of <see cref="RoomType"/> that spawned this round.
		/// </summary>
		public static ReadOnlyCollection<RoomType> SpawnedRoomTypes =>
			_spawnedRoomTypes; // ??= Room.List.Select(x => x.Type).Distinct().ToList().AsReadOnly();

		/// <summary>
		/// Gets the readonly list of <see cref="RoomType"/> that spawned this round AND are not distinct (there are multiple rooms of the same type).
		/// </summary>
		public static ReadOnlyCollection<RoomType> NonDistinctRoomTypes =>
			_nonDistinctRoomTypes; // ??= SpawnedRoomTypes.Where(roomType => Room.List.Count(x => x.Type == roomType) > 1).ToList().AsReadOnly();

		internal static void PopulateRoomTypeLists()
		{
			List<RoomType> spawnedRoomTypes = new();
			List<RoomType> nonDistinctRoomTypes = new();

			foreach (Room room in Room.List)
			{
				RoomType type = room.Type;
				if (!spawnedRoomTypes.Contains(type))
					spawnedRoomTypes.Add(type);
			}

			foreach (RoomType spawnedRoomType in spawnedRoomTypes)
			{
				if (Room.List.Count(x => x.Type == spawnedRoomType) > 1)
					nonDistinctRoomTypes.Add(spawnedRoomType);
			}

			_spawnedRoomTypes = spawnedRoomTypes.AsReadOnly();
			_nonDistinctRoomTypes = nonDistinctRoomTypes.AsReadOnly();
		}

		/// <summary>
		/// Gets the dictionary that stores currently selected <see cref="ObjectType"/> by <see cref="InventorySystem.Items.ItemBase.ItemSerial"/>.
		/// </summary>
		internal static Dictionary<ushort, ObjectType> ToolGuns { get; private set; } = new();

		internal static Dictionary<ushort, GravityGunMode> GravityGuns { get; private set; } = new();

		internal static HashSet<ushort> PickupsLocked { get; private set; } = new();

		internal static Dictionary<ushort, int> PickupsUsesLeft { get; private set; } = new();

		/// <summary>
		/// Gets the <see cref="List{T}"/> containing objects that are a part of currently loaded <see cref="MapSchematic"/>.
		/// </summary>
		public static List<MapEditorObject> SpawnedObjects { get; } = new();

		/// <summary>
		/// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all <see cref="ObjectType"/> and <see cref="GameObject"/> pairs.
		/// </summary>
		public static ReadOnlyDictionary<ObjectType, GameObject> ObjectPrefabs { get; internal set; }

		internal static void GetObjectPrefabs()
		{
			Dictionary<ObjectType, GameObject> objectList = new(21);
			// DoorSpawnpoint[] doorList = Object.FindObjectsOfType<DoorSpawnpoint>();

			objectList.Add(ObjectType.LczDoor, NetworkClient.prefabs.Values.First(x => x.name.Contains("LCZ")));
			objectList.Add(ObjectType.HczDoor, NetworkClient.prefabs.Values.First(x => x.name.Contains("HCZ")));
			objectList.Add(ObjectType.EzDoor, NetworkClient.prefabs.Values.First(x => x.name.Contains("EZ")));

			objectList.Add(ObjectType.WorkStation, NetworkClient.prefabs.Values.First(x => x.name.Contains("Work Station")));

			objectList.Add(ObjectType.ItemSpawnPoint, new GameObject("ItemSpawnPointObject"));
			objectList.Add(ObjectType.PlayerSpawnPoint, new GameObject("PlayerSpawnPointObject"));
			objectList.Add(ObjectType.RagdollSpawnPoint, new GameObject("RagdollSpawnPointObject"));
			objectList.Add(ObjectType.DummySpawnPoint, new GameObject("DummySpawnPointObject"));

			foreach (GameObject gameObject in NetworkClient.prefabs.Values)
			{
				if (gameObject.TryGetComponent(out PrimitiveObjectToy _))
				{
					objectList.Add(ObjectType.Primitive, gameObject);
					continue;
				}

				if (gameObject.TryGetComponent(out LightSourceToy _))
				{
					objectList.Add(ObjectType.LightSource, gameObject);
					continue;
				}

				if (gameObject.TryGetComponent(out ShootingTarget _))
				{
					switch (gameObject.name)
					{
						case "sportTargetPrefab":
							objectList.Add(ObjectType.SportShootingTarget, gameObject);
							continue;

						case "dboyTargetPrefab":
							objectList.Add(ObjectType.DboyShootingTarget, gameObject);
							continue;

						case "binaryTargetPrefab":
							objectList.Add(ObjectType.BinaryShootingTarget, gameObject);
							continue;
					}
				}
			}

			objectList.Add(ObjectType.RoomLight, new GameObject("LightControllerObject"));

			GameObject teleportPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
			teleportPrefab.name = "TeleportObject";
			objectList.Add(ObjectType.Teleporter, teleportPrefab);

			objectList.Add(ObjectType.PedestalLocker, NetworkClient.prefabs.Values.First(x => x.name == "Scp500PedestalStructure Variant"));
			objectList.Add(ObjectType.LargeGunLocker, NetworkClient.prefabs.Values.First(x => x.name == "LargeGunLockerStructure"));
			objectList.Add(ObjectType.RifleRackLocker, NetworkClient.prefabs.Values.First(x => x.name == "RifleRackStructure"));
			objectList.Add(ObjectType.MiscLocker, NetworkClient.prefabs.Values.First(x => x.name == "MiscLocker"));
			objectList.Add(ObjectType.MedkitLocker, NetworkClient.prefabs.Values.First(x => x.name == "RegularMedkitStructure"));
			objectList.Add(ObjectType.AdrenalineLocker, NetworkClient.prefabs.Values.First(x => x.name == "AdrenalineMedkitStructure"));

			ObjectPrefabs = new ReadOnlyDictionary<ObjectType, GameObject>(objectList);
		}

		/// <summary>
		/// Gets a random <see cref="Room"/> from the <see cref="RoomType"/>.
		/// </summary>
		/// <param name="type">The <see cref="RoomType"/> from which the room should be chosen.</param>
		/// <returns>A random <see cref="Room"/> that has <see cref="Room.Type"/> of the argument.</returns>
		public static Room? GetRandomRoom(RoomType type)
		{
			if (type == RoomType.Unknown)
				return null;

			List<Room> validRooms = Room.List.Where(x => x.Type == type).ToList();

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
	}
}