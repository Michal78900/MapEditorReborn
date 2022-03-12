namespace MapEditorReborn.API.Features
{
    using Components;
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Extensions;
    using Objects;
    using Serializable;
    using UnityEngine;

    using static API;

    /// <summary>
    /// A tool used to easily handle spawn behaviors of the objects.
    /// </summary>
    public static class ObjectSpawner
    {
        /// <summary>
        /// Spawns a door.
        /// </summary>
        /// <param name="door">The <see cref="DoorSerializable"/> which is used to spawn a door.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>The spawned <see cref="DoorObject"/>.</returns>
        public static DoorObject SpawnDoor(DoorSerializable door, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(door.RoomType);
            GameObject gameObject = Object.Instantiate(door.DoorType.GetDoorObjectByType(), forcedPosition ?? GetRelativePosition(door.Position, room), forcedRotation ?? GetRelativeRotation(door.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? door.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(door.Rotation);

            return gameObject.AddComponent<DoorObject>().Init(door);
        }

        /// <summary>
        /// Spawns a workstation.
        /// </summary>
        /// <param name="workStation">The <see cref="WorkstationSerializable"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>The spawned <see cref="WorkstationObject"/>.</returns>
        public static WorkstationObject SpawnWorkStation(WorkstationSerializable workStation, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(workStation.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.WorkStation.GetObjectByMode(), forcedPosition ?? GetRelativePosition(workStation.Position, room), forcedRotation ?? GetRelativeRotation(workStation.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? workStation.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(workStation.Rotation);

            return gameObject.AddComponent<WorkstationObject>().Init(workStation);
        }

        /// <summary>
        /// Spawns a ItemSpawnPoint.
        /// </summary>
        /// <param name="itemSpawnPoint">The <see cref="ItemSpawnPointSerializable"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>The spawned <see cref="ItemSpawnPointObject"/>.</returns>
        public static ItemSpawnPointObject SpawnItemSpawnPoint(ItemSpawnPointSerializable itemSpawnPoint, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(itemSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.ItemSpawnPoint.GetObjectByMode(), forcedPosition ?? GetRelativePosition(itemSpawnPoint.Position, room), forcedRotation ?? GetRelativeRotation(itemSpawnPoint.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? itemSpawnPoint.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(itemSpawnPoint.Rotation);

            return gameObject.AddComponent<ItemSpawnPointObject>().Init(itemSpawnPoint);
        }

        /// <summary>
        /// Spawns a PlayerSpawnPoint.
        /// </summary>
        /// <param name="playerSpawnPoint">The <see cref="PlayerSpawnPointSerializable"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <returns>The spawned <see cref="PlayerSpawnPointObject"/>.</returns>
        public static PlayerSpawnPointObject SpawnPlayerSpawnPoint(PlayerSpawnPointSerializable playerSpawnPoint, Vector3? forcedPosition = null)
        {
            Room room = GetRandomRoom(playerSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.PlayerSpawnPoint.GetObjectByMode(), forcedPosition ?? GetRelativePosition(playerSpawnPoint.Position, room), Quaternion.identity);

            return gameObject.AddComponent<PlayerSpawnPointObject>().Init(playerSpawnPoint);
        }

        /// <summary>
        /// Spawns a RagdollSpawnPoint.
        /// </summary>
        /// <param name="ragdollSpawnPoint">The <see cref="RagdollSpawnPointSerializable"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <returns>The spawned <see cref="RagdollSpawnPointObject"/>.</returns>
        public static RagdollSpawnPointObject SpawnRagdollSpawnPoint(RagdollSpawnPointSerializable ragdollSpawnPoint, Vector3? forcedPosition = null, Quaternion? forcedRotation = null)
        {
            Room room = GetRandomRoom(ragdollSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.RagdollSpawnPoint.GetObjectByMode(), forcedPosition ?? GetRelativePosition(ragdollSpawnPoint.Position, room), forcedRotation ?? GetRelativeRotation(ragdollSpawnPoint.Rotation, room));

            gameObject.AddComponent<ObjectRotationComponent>().Init(ragdollSpawnPoint.Rotation);

            return gameObject.AddComponent<RagdollSpawnPointObject>().Init(ragdollSpawnPoint);
        }

        /// <summary>
        /// Spawns a dummy spawnpoint.
        /// </summary>
        /// <returns>The spawned <see cref="MapEditorObject"/>.</returns>
        public static MapEditorObject SpawnDummySpawnPoint() => null;

        /// <summary>
        /// Spawns a ShootingTarget.
        /// </summary>
        /// <param name="shootingTarget">The <see cref="ShootingTargetSerializable"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>The spawned <see cref="ShootingTargetObject"/>.</returns>
        public static ShootingTargetObject SpawnShootingTarget(ShootingTargetSerializable shootingTarget, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(shootingTarget.RoomType);
            GameObject gameObject = Object.Instantiate(shootingTarget.TargetType.GetShootingTargetObjectByType(), forcedPosition ?? GetRelativePosition(shootingTarget.Position, room), forcedRotation ?? GetRelativeRotation(shootingTarget.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? shootingTarget.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(shootingTarget.Rotation);

            return gameObject.AddComponent<ShootingTargetObject>().Init(shootingTarget);
        }

        /// <summary>
        /// Spawns a <see cref="PrimitiveSerializable"/>.
        /// </summary>
        /// <param name="primitiveObject">The <see cref="PrimitiveSerializable"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>The spawned <see cref="PrimitiveObject"/>.</returns>
        public static PrimitiveObject SpawnPrimitive(PrimitiveSerializable primitiveObject, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(primitiveObject.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.Primitive.GetObjectByMode(), forcedPosition ?? GetRelativePosition(primitiveObject.Position, room), forcedRotation ?? GetRelativeRotation(primitiveObject.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? primitiveObject.Scale;

            return gameObject.AddComponent<PrimitiveObject>().Init(primitiveObject);
        }

        /// <summary>
        /// Spawns a LightController.
        /// </summary>
        /// <param name="lightController">The <see cref="RoomLightSerializable"/> to spawn.</param>
        /// <returns>The spawned <see cref="MapEditorObject"/>.</returns>
        public static RoomLightObject SpawnRoomLight(RoomLightSerializable lightController) => Object.Instantiate(ObjectType.RoomLight.GetObjectByMode()).AddComponent<RoomLightObject>().Init(lightController);

        /// <summary>
        /// Spawns a <see cref="LightSourceSerializable"/>.
        /// </summary>
        /// <param name="lightSourceObject">The <see cref="LightSourceSerializable"/> to spawn.</param>
        /// <param name="forcedPosition">The specified position.</param>
        /// <returns>The spawned <see cref="LightSourceObject"/>.</returns>
        public static LightSourceObject SpawnLightSource(LightSourceSerializable lightSourceObject, Vector3? forcedPosition = null)
        {
            Room room = GetRandomRoom(lightSourceObject.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.LightSource.GetObjectByMode(), forcedPosition ?? GetRelativePosition(lightSourceObject.Position, room), Quaternion.identity);

            return gameObject.AddComponent<LightSourceObject>().Init(lightSourceObject);
        }

        /// <summary>
        /// Spawns a Teleporter.
        /// </summary>
        /// <param name="teleport">The <see cref="TeleportSerializable"/> to spawn.</param>
        /// <returns>The spawned <see cref="MapEditorObject"/>.</returns>
        public static TeleportControllerObject SpawnTeleport(TeleportSerializable teleport) => Object.Instantiate(ObjectType.Teleporter.GetObjectByMode()).AddComponent<TeleportControllerObject>().Init(teleport);

        public static LockerObject SpawnLocker(LockerSerializable locker, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(locker.RoomType);
            GameObject gameObject = Object.Instantiate(locker.LockerType.GetLockerObjectByType(), forcedPosition ?? GetRelativePosition(locker.Position, room), forcedRotation ?? GetRelativeRotation(locker.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? locker.Scale;

            return gameObject.AddComponent<LockerObject>().Init(locker);
        }

        /// <summary>
        /// Spawns a <see cref="SchematicObject"/>.
        /// </summary>
        /// <param name="schematicName">The schematic's name.</param>
        /// <param name="position">The schematic's position.</param>
        /// <param name="rotation">The schematic's rotation.</param>
        /// <param name="scale">The schematic' scale.</param>
        /// <param name="data">The schematic data.</param>
        /// <returns>The spawned <see cref="SchematicObject"/>.</returns>
        public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Quaternion? rotation = null, Vector3? scale = null, SchematicObjectDataList data = null)
        {
            return SpawnSchematic(new SchematicSerializable(schematicName), position, rotation, scale, data);
        }

        /// <summary>
        /// Spawns a Schematic.
        /// </summary>
        /// <param name="schematicObject">The <see cref="SchematicSerializable"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <param name="data">The schematic data.</param>
        /// <returns>The spawned <see cref="SchematicObject"/>.</returns>
        public static SchematicObject SpawnSchematic(SchematicSerializable schematicObject, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null, SchematicObjectDataList data = null)
        {
            if (data == null)
            {
                data = MapUtils.GetSchematicDataByName(schematicObject.SchematicName);

                if (data == null)
                    return null;
            }

            Room room = null;

            if (schematicObject.RoomType != RoomType.Unknown)
                room = GetRandomRoom(schematicObject.RoomType);

            GameObject gameObject = new GameObject($"CustomSchematic-{schematicObject.SchematicName}")
            {
                layer = 2,
            };

            gameObject.transform.position = forcedPosition ?? GetRelativePosition(schematicObject.Position, room);
            gameObject.transform.rotation = forcedRotation ?? GetRelativeRotation(schematicObject.Rotation, room);

            SchematicObject schematicObjectComponent = gameObject.AddComponent<SchematicObject>().Init(schematicObject, data);
            gameObject.transform.localScale = forcedScale ?? schematicObject.Scale;

            var ev = new Events.EventArgs.SchematicSpawnedEventArgs(schematicObjectComponent, schematicObject.SchematicName);
            Events.Handlers.Schematic.OnSchematicSpawned(ev);

            Patches.OverridePositionPatch.ResetValues();

            return schematicObjectComponent;
        }

        /// <summary>
        /// Spawns a copy of selected object by a ToolGun.
        /// </summary>
        /// <param name="position">Position of spawned property object.</param>
        /// <param name="prefab">The <see cref="GameObject"/> from which the copy will be spawned.</param>
        /// <returns>The spawned <see cref="MapEditorObject"/>.</returns>
        public static MapEditorObject SpawnPropertyObject(Vector3 position, MapEditorObject prefab)
        {
            Quaternion rotation = prefab.transform.rotation;
            Vector3 scale = prefab.transform.localScale;

            switch (prefab)
            {
                case DoorObject door:
                    return SpawnDoor(new DoorSerializable().CopyProperties(door.Base), position, rotation, scale);

                case WorkstationObject workStation:
                    return SpawnWorkStation(new WorkstationSerializable().CopyProperties(workStation.Base), position, rotation, scale);

                case ItemSpawnPointObject itemSpawnPoint:
                    return SpawnItemSpawnPoint(new ItemSpawnPointSerializable().CopyProperties(itemSpawnPoint.Base), position, rotation, scale);

                case PlayerSpawnPointObject playerSpawnPoint:
                    return SpawnPlayerSpawnPoint(new PlayerSpawnPointSerializable().CopyProperties(playerSpawnPoint.Base), position);

                case RagdollSpawnPointObject ragdollSpawnPoint:
                    return SpawnRagdollSpawnPoint(new RagdollSpawnPointSerializable().CopyProperties(ragdollSpawnPoint.Base), position, rotation);

                case ShootingTargetObject shootingTarget:
                    return SpawnShootingTarget(new ShootingTargetSerializable().CopyProperties(shootingTarget.Base), position, rotation, scale);

                case PrimitiveObject primitive:
                    return SpawnPrimitive(new PrimitiveSerializable().CopyProperties(primitive.Base), position + (Vector3.up * 0.5f), rotation, scale);

                case LockerObject locker:
                    return SpawnLocker(new LockerSerializable().CopyProperties(locker.Base), position, rotation, scale);

                case SchematicObject schematic:
                    return SpawnSchematic(new SchematicSerializable().CopyProperties(schematic.Base), position, rotation, scale);

                default:
                    return null;
            }
        }
    }
}
