namespace MapEditorReborn.API.Features
{
    using Components;
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Extensions;
    using Objects;
    using Serializable;
    using System;
    using UnityEngine;

    using static API;

    /// <summary>
    /// A tool used to easily handle spawn behaviors of the objects.
    /// </summary>
    public static class ObjectSpawner
    {
        /// <summary>
        /// Spawns a <see cref="MapEditorObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="MapEditorObject"/> type.</typeparam>
        /// <param name="args">The arguments to pass.</param>
        /// <returns>The spawned <see cref="MapEditorObject"/> as <typeparamref name="T"/></returns>
        public static T SpawnObject<T>(params object[] args)
            where T : MapEditorObject
        {
            Type type = typeof(T);
            if (type == typeof(SchematicObject))
            {
                SchematicSerializable schematicObject;
                if (args[0] is SchematicSerializable serializable)
                    schematicObject = serializable;
                else
                    schematicObject = new(args[0] as string);
                Vector3? forcedPosition = args[1] as Vector3?;
                Quaternion? forcedRotation = args[2] as Quaternion?;
                Vector3? forcedScale = args[3] as Vector3?;

                if (args[4] is not SchematicObjectDataList data)
                {
                    data = MapUtils.GetSchematicDataByName(schematicObject.SchematicName);

                    if (data == null)
                        return null;
                }

                Room room = null;

                if (schematicObject.RoomType != RoomType.Unknown)
                    room = GetRandomRoom(schematicObject.RoomType);

                GameObject gameObject = new($"CustomSchematic-{schematicObject.SchematicName}")
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

                return schematicObjectComponent as T;
            }
            else if (type == typeof(LockerObject))
            {
                LockerSerializable locker = args[0] as LockerSerializable;
                Vector3? forcedPosition = args[1] as Vector3?;
                Quaternion? forcedRotation = args[2] as Quaternion?;
                Vector3? forcedScale = args[3] as Vector3?;
                Room room = GetRandomRoom(locker.RoomType);
                GameObject gameObject = UnityEngine.Object.Instantiate(locker.LockerType.GetLockerObjectByType(), forcedPosition ?? GetRelativePosition(locker.Position, room), forcedRotation ?? GetRelativeRotation(locker.Rotation, room));
                gameObject.transform.localScale = forcedScale ?? locker.Scale;
                return gameObject.AddComponent<LockerObject>().Init(locker) as T;
            }
            else if (type == typeof(TeleportControllerObject))
                return UnityEngine.Object.Instantiate(ObjectType.Teleporter.GetObjectByMode()).AddComponent<TeleportControllerObject>().Init(args[0] as TeleportSerializable) as T;
            else if (type == typeof(LightSourceObject))
            {
                LightSourceSerializable lightSourceObject = args[0] as LightSourceSerializable;
                Vector3? forcedPosition = args[1] as Vector3?;
                Room room = GetRandomRoom(lightSourceObject.RoomType);
                GameObject gameObject = UnityEngine.Object.Instantiate(ObjectType.LightSource.GetObjectByMode(), forcedPosition ?? GetRelativePosition(lightSourceObject.Position, room), Quaternion.identity);
                return gameObject.AddComponent<LightSourceObject>().Init(lightSourceObject) as T;
            }
            else if (type == typeof(RoomLightObject))
                return UnityEngine.Object.Instantiate(ObjectType.RoomLight.GetObjectByMode()).AddComponent<RoomLightObject>().Init(args[0] as RoomLightSerializable) as T;
            else if (type == typeof(PrimitiveObject))
            {
                PrimitiveSerializable primitiveObject = args[0] as PrimitiveSerializable;
                Vector3? forcedPosition = args[1] as Vector3?;
                Quaternion? forcedRotation = args[2] as Quaternion?;
                Vector3? forcedScale = args[3] as Vector3?;
                Room room = GetRandomRoom(primitiveObject.RoomType);
                GameObject gameObject = UnityEngine.Object.Instantiate(ObjectType.Primitive.GetObjectByMode(), forcedPosition ?? GetRelativePosition(primitiveObject.Position, room), forcedRotation ?? GetRelativeRotation(primitiveObject.Rotation, room));
                gameObject.transform.localScale = forcedScale ?? primitiveObject.Scale;
                return gameObject.AddComponent<PrimitiveObject>().Init(primitiveObject) as T;
            }
            else if (type == typeof(ShootingTargetObject))
            {
                ShootingTargetSerializable shootingTarget = args[0] as ShootingTargetSerializable;
                Vector3? forcedPosition = args[1] as Vector3?;
                Quaternion? forcedRotation = args[2] as Quaternion?;
                Vector3? forcedScale = args[3] as Vector3?;
                Room room = GetRandomRoom(shootingTarget.RoomType);
                GameObject gameObject = UnityEngine.Object.Instantiate(shootingTarget.TargetType.GetShootingTargetObjectByType(), forcedPosition ?? GetRelativePosition(shootingTarget.Position, room), forcedRotation ?? GetRelativeRotation(shootingTarget.Rotation, room));
                gameObject.transform.localScale = forcedScale ?? shootingTarget.Scale;
                gameObject.AddComponent<ObjectRotationComponent>().Init(shootingTarget.Rotation);
                return gameObject.AddComponent<ShootingTargetObject>().Init(shootingTarget) as T;
            }
            else if (type == typeof(RagdollSpawnPointObject))
            {
                RagdollSpawnPointSerializable ragdollSpawnPoint = args[0] as RagdollSpawnPointSerializable;
                Vector3? forcedPosition = args[1] as Vector3?;
                Quaternion? forcedRotation = args[2] as Quaternion?;
                Vector3? forcedScale = args[3] as Vector3?;
                Room room = GetRandomRoom(ragdollSpawnPoint.RoomType);
                GameObject gameObject = UnityEngine.Object.Instantiate(ObjectType.RagdollSpawnPoint.GetObjectByMode(), forcedPosition ?? GetRelativePosition(ragdollSpawnPoint.Position, room), forcedRotation ?? GetRelativeRotation(ragdollSpawnPoint.Rotation, room));
                gameObject.AddComponent<ObjectRotationComponent>().Init(ragdollSpawnPoint.Rotation);
                return gameObject.AddComponent<RagdollSpawnPointObject>().Init(ragdollSpawnPoint) as T;
            }
            else if (type == typeof(PlayerSpawnPointObject))
            {
                PlayerSpawnPointSerializable playerSpawnPoint = args[0] as PlayerSpawnPointSerializable;
                Vector3? forcedPosition = args[1] as Vector3?;
                Room room = GetRandomRoom(playerSpawnPoint.RoomType);
                GameObject gameObject = UnityEngine.Object.Instantiate(ObjectType.PlayerSpawnPoint.GetObjectByMode(), forcedPosition ?? GetRelativePosition(playerSpawnPoint.Position, room), Quaternion.identity);
                return gameObject.AddComponent<PlayerSpawnPointObject>().Init(playerSpawnPoint) as T;
            }
            else if (type == typeof(ItemSpawnPointObject))
            {
                ItemSpawnPointSerializable itemSpawnPoint = args[0] as ItemSpawnPointSerializable;
                Vector3? forcedPosition = args[1] as Vector3?;
                Quaternion? forcedRotation = args[2] as Quaternion?;
                Vector3? forcedScale = args[3] as Vector3?;
                Room room = GetRandomRoom(itemSpawnPoint.RoomType);
                GameObject gameObject = UnityEngine.Object.Instantiate(ObjectType.ItemSpawnPoint.GetObjectByMode(), forcedPosition ?? GetRelativePosition(itemSpawnPoint.Position, room), forcedRotation ?? GetRelativeRotation(itemSpawnPoint.Rotation, room));
                gameObject.transform.localScale = forcedScale ?? itemSpawnPoint.Scale;
                gameObject.AddComponent<ObjectRotationComponent>().Init(itemSpawnPoint.Rotation);
                return gameObject.AddComponent<ItemSpawnPointObject>().Init(itemSpawnPoint) as T;
            }
            else if (type == typeof(WorkstationObject))
            {
                WorkstationSerializable workStation = args[0] as WorkstationSerializable;
                Vector3? forcedPosition = args[1] as Vector3?;
                Quaternion? forcedRotation = args[2] as Quaternion?;
                Vector3? forcedScale = args[3] as Vector3?;
                Room room = GetRandomRoom(workStation.RoomType);
                GameObject gameObject = UnityEngine.Object.Instantiate(ObjectType.WorkStation.GetObjectByMode(), forcedPosition ?? GetRelativePosition(workStation.Position, room), forcedRotation ?? GetRelativeRotation(workStation.Rotation, room));
                gameObject.transform.localScale = forcedScale ?? workStation.Scale;
                gameObject.AddComponent<ObjectRotationComponent>().Init(workStation.Rotation);
                return gameObject.AddComponent<WorkstationObject>().Init(workStation) as T;
            }
            else if (type == typeof(DoorObject))
            {
                DoorSerializable door = args[0] as DoorSerializable;
                Vector3? forcedPosition = args[1] as Vector3?;
                Quaternion? forcedRotation = args[2] as Quaternion?;
                Vector3? forcedScale = args[3] as Vector3?;
                Room room = GetRandomRoom(door.RoomType);
                GameObject gameObject = UnityEngine.Object.Instantiate(door.DoorType.GetDoorObjectByType(), forcedPosition ?? GetRelativePosition(door.Position, room), forcedRotation ?? GetRelativeRotation(door.Rotation, room));
                gameObject.transform.localScale = forcedScale ?? door.Scale;
                gameObject.AddComponent<ObjectRotationComponent>().Init(door.Rotation);
                return gameObject.AddComponent<DoorObject>().Init(door) as T;
            }
            else
                return null;
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

            return prefab switch
            {
                DoorObject door => SpawnObject<DoorObject>(new DoorSerializable().CopyProperties(door.Base), position, rotation, scale),
                WorkstationObject workStation => SpawnObject<WorkstationObject>(new WorkstationSerializable().CopyProperties(workStation.Base), position, rotation, scale),
                ItemSpawnPointObject itemSpawnPoint => SpawnObject<ItemSpawnPointObject>(new ItemSpawnPointSerializable().CopyProperties(itemSpawnPoint.Base), position, rotation, scale),
                PlayerSpawnPointObject playerSpawnPoint => SpawnObject<PlayerSpawnPointObject>(new PlayerSpawnPointSerializable().CopyProperties(playerSpawnPoint.Base), position),
                RagdollSpawnPointObject ragdollSpawnPoint => SpawnObject<RagdollSpawnPointObject>(new RagdollSpawnPointSerializable().CopyProperties(ragdollSpawnPoint.Base), position, rotation),
                ShootingTargetObject shootingTarget => SpawnObject<ShootingTargetObject>(new ShootingTargetSerializable().CopyProperties(shootingTarget.Base), position, rotation, scale),
                PrimitiveObject primitive => SpawnObject<PrimitiveObject>(new PrimitiveSerializable().CopyProperties(primitive.Base), position + (Vector3.up * 0.5f), rotation, scale),
                LockerObject locker => SpawnObject<LockerObject>(new LockerSerializable().CopyProperties(locker.Base), position, rotation, scale),
                SchematicObject schematic => SpawnObject<SchematicObject>(new SchematicSerializable().CopyProperties(schematic.Base), position, rotation, scale),
                _ => null,
            };
        }
    }
}
