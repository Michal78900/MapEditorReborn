﻿namespace MapEditorReborn.API.Features
{
    using System.Linq;
    using Components;
    using Components.ObjectComponents;
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Extensions;
    using Objects;
    using Objects.Schematics;
    using UnityEngine;

    using static API;

    public static class ObjectSpawner
    {
        /// <summary>
        /// Spawns a door.
        /// </summary>
        /// <param name="door">The <see cref="DoorObject"/> which is used to spawn a door.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static DoorObjectComponent SpawnDoor(DoorObject door, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(door.RoomType);
            GameObject gameObject = Object.Instantiate(door.DoorType.GetDoorObjectByType(), forcedPosition ?? GetRelativePosition(door.Position, room), forcedRotation ?? GetRelativeRotation(door.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? door.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(door.Rotation);

            return gameObject.AddComponent<DoorObjectComponent>().Init(door);
        }

        /// <summary>
        /// Spawns a workstation.
        /// </summary>
        /// <param name="workStation">The <see cref="WorkStationObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static WorkStationObjectComponent SpawnWorkStation(WorkStationObject workStation, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(workStation.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.WorkStation.GetObjectByMode(), forcedPosition ?? GetRelativePosition(workStation.Position, room), forcedRotation ?? GetRelativeRotation(workStation.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? workStation.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(workStation.Rotation);

            return gameObject.AddComponent<WorkStationObjectComponent>().Init(workStation);
        }

        /// <summary>
        /// Spawns a ItemSpawnPoint.
        /// </summary>
        /// <param name="itemSpawnPoint">The <see cref="ItemSpawnPointObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static ItemSpawnPointComponent SpawnItemSpawnPoint(ItemSpawnPointObject itemSpawnPoint, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(itemSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.ItemSpawnPoint.GetObjectByMode(), forcedPosition ?? GetRelativePosition(itemSpawnPoint.Position, room), forcedRotation ?? GetRelativeRotation(itemSpawnPoint.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? itemSpawnPoint.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(itemSpawnPoint.Rotation);

            return gameObject.AddComponent<ItemSpawnPointComponent>().Init(itemSpawnPoint);
        }

        /// <summary>
        /// Spawns a PlayerSpawnPoint.
        /// </summary>
        /// <param name="playerSpawnPoint">The <see cref="PlayerSpawnPointObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static PlayerSpawnPointComponent SpawnPlayerSpawnPoint(PlayerSpawnPointObject playerSpawnPoint, Vector3? forcedPosition = null)
        {
            Room room = GetRandomRoom(playerSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.PlayerSpawnPoint.GetObjectByMode(), forcedPosition ?? GetRelativePosition(playerSpawnPoint.Position, room), Quaternion.identity);

            return gameObject.AddComponent<PlayerSpawnPointComponent>().Init(playerSpawnPoint);
        }

        /// <summary>
        /// Spawns a RagdollSpawnPoint.
        /// </summary>
        /// <param name="ragdollSpawnPoint">The <see cref="RagdollSpawnPointObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static RagdollSpawnPointComponent SpawnRagdollSpawnPoint(RagdollSpawnPointObject ragdollSpawnPoint, Vector3? forcedPosition = null, Quaternion? forcedRotation = null)
        {
            Room room = GetRandomRoom(ragdollSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.RagdollSpawnPoint.GetObjectByMode(), forcedPosition ?? GetRelativePosition(ragdollSpawnPoint.Position, room), forcedRotation ?? GetRelativeRotation(ragdollSpawnPoint.Rotation, room));

            gameObject.AddComponent<ObjectRotationComponent>().Init(ragdollSpawnPoint.Rotation);

            return gameObject.AddComponent<RagdollSpawnPointComponent>().Init(ragdollSpawnPoint);
        }

        public static MapEditorObject SpawnDummySpawnPoint() => null;

        /// <summary>
        /// Spawns a ShootingTarget.
        /// </summary>
        /// <param name="shootingTarget">The <see cref="ShootingTargetObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static ShootingTargetComponent SpawnShootingTarget(ShootingTargetObject shootingTarget, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(shootingTarget.RoomType);
            GameObject gameObject = Object.Instantiate(shootingTarget.TargetType.GetShootingTargetObjectByType(), forcedPosition ?? GetRelativePosition(shootingTarget.Position, room), forcedRotation ?? GetRelativeRotation(shootingTarget.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? shootingTarget.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(shootingTarget.Rotation);

            return gameObject.AddComponent<ShootingTargetComponent>().Init(shootingTarget);
        }

        public static PrimitiveObjectComponent SpawnPrimitive(PrimitiveObject primitiveObject, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(primitiveObject.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.Primitive.GetObjectByMode(), forcedPosition ?? GetRelativePosition(primitiveObject.Position, room), forcedRotation ?? GetRelativeRotation(primitiveObject.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? primitiveObject.Scale;

            return gameObject.AddComponent<PrimitiveObjectComponent>().Init(primitiveObject);
        }

        /// <summary>
        /// Spawns a LightController.
        /// </summary>
        /// <param name="lightController">The <see cref="RoomLightObject"/> to spawn.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static RoomLightComponent SpawnRoomLight(RoomLightObject lightController) => Object.Instantiate(ObjectType.RoomLight.GetObjectByMode()).AddComponent<RoomLightComponent>().Init(lightController);

        public static LightSourceComponent SpawnLightSource(LightSourceObject lightSourceObject, Vector3? forcedPosition = null)
        {
            Room room = GetRandomRoom(lightSourceObject.RoomType);
            GameObject gameObject = Object.Instantiate(ObjectType.LightSource.GetObjectByMode(), forcedPosition ?? GetRelativePosition(lightSourceObject.Position, room), Quaternion.identity);

            return gameObject.AddComponent<LightSourceComponent>().Init(lightSourceObject);
        }

        /// <summary>
        /// Spawns a Teleporter.
        /// </summary>
        /// <param name="teleport">The <see cref="TeleportObject"/> to spawn.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static TeleportControllerComponent SpawnTeleport(TeleportObject teleport) => Object.Instantiate(ObjectType.Teleporter.GetObjectByMode()).AddComponent<TeleportControllerComponent>().Init(teleport);

        public static SchematicObjectComponent SpawnSchematic(string schematicName, Vector3 position, Quaternion? rotation = null, Vector3? scale = null)
        {
            return SpawnSchematic(new SchematicObject(schematicName), position, rotation, scale);
        }

        /// <summary>
        /// Spawns a Schematic.
        /// </summary>
        /// <param name="schematicObject">The <see cref="SchematicObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>Spawned <see cref="SchematicObject"/>.</returns>
        public static SchematicObjectComponent SpawnSchematic(SchematicObject schematicObject, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            SaveDataObjectList data = MapUtils.GetSchematicDataByName(schematicObject.SchematicName);

            if (data == null)
                return null;

            Room room = null;

            if (schematicObject.RoomType != RoomType.Unknown)
                room = GetRandomRoom(schematicObject.RoomType);

            GameObject gameObject = new GameObject($"CustomSchematic-{schematicObject.SchematicName}");
            gameObject.transform.position = forcedPosition ?? GetRelativePosition(schematicObject.Position, room);
            gameObject.transform.rotation = forcedRotation ?? GetRelativeRotation(schematicObject.Rotation, room);
            gameObject.transform.localScale = forcedScale ?? schematicObject.Scale;

            return gameObject.AddComponent<SchematicObjectComponent>().Init(schematicObject, data);
        }

        /// <summary>
        /// Spawns a copy of selected object by a ToolGun.
        /// </summary>
        /// <param name="position">Position of spawned property object.</param>
        /// <param name="prefab">The <see cref="GameObject"/> from which the copy will be spawned.</param>
        public static void SpawnPropertyObject(Vector3 position, MapEditorObject prefab)
        {
            Quaternion rotation = prefab.transform.rotation;
            Vector3 scale = prefab.transform.localScale;

            switch (prefab)
            {
                case DoorObjectComponent door:
                    {
                        SpawnedObjects.Add(SpawnDoor(new DoorObject().CopyProperties(door.Base), position, rotation, scale));
                        break;
                    }

                case WorkStationObjectComponent workStation:
                    {
                        SpawnedObjects.Add(SpawnWorkStation(new WorkStationObject().CopyProperties(workStation.Base), position, rotation, scale));
                        break;
                    }

                case ItemSpawnPointComponent itemSpawnPoint:
                    {
                        SpawnedObjects.Add(SpawnItemSpawnPoint(new ItemSpawnPointObject().CopyProperties(itemSpawnPoint.Base), position, rotation, scale));
                        break;
                    }

                case PlayerSpawnPointComponent playerSpawnPoint:
                    {
                        SpawnedObjects.Add(SpawnPlayerSpawnPoint(new PlayerSpawnPointObject().CopyProperties(playerSpawnPoint.Base), position));
                        break;
                    }

                case RagdollSpawnPointComponent ragdollSpawnPoint:
                    {
                        SpawnedObjects.Add(SpawnRagdollSpawnPoint(new RagdollSpawnPointObject().CopyProperties(ragdollSpawnPoint.Base), position, rotation));
                        break;
                    }

                case ShootingTargetComponent shootingTarget:
                    {
                        SpawnedObjects.Add(SpawnShootingTarget(new ShootingTargetObject().CopyProperties(shootingTarget.Base), position, rotation, scale));
                        break;
                    }

                case PrimitiveObjectComponent primitive:
                    {
                        SpawnedObjects.Add(SpawnPrimitive(new PrimitiveObject().CopyProperties(primitive.Base), position + (Vector3.up * 0.5f), rotation, scale));
                        break;
                    }

                case SchematicObjectComponent schematic:
                    {
                        SpawnedObjects.Add(SpawnSchematic(new SchematicObject().CopyProperties(schematic.Base), position + Vector3.up, rotation, scale));
                        break;
                    }
            }

            if (MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn)
                SpawnedObjects.Last().UpdateIndicator();
        }
    }
}