namespace MapEditorReborn.Events.Handlers.Internal
{
    using System.Linq;
    using API.Enums;
    using API.Extensions;
    using API.Features.Objects;
    using API.Features.Serializable;
    using Exiled.API.Features;
    using MEC;
    using UnityEngine;

    using static API.API;

    /// <summary>
    /// A tool to easily handle the ToolGun behavior.
    /// </summary>
    internal static class ToolGunHandler
    {
        /// <summary>
        /// Spawns a general <see cref="MapEditorObject"/>.
        /// Used by the ToolGun.
        /// </summary>
        /// <param name="position">The postition of the spawned object.</param>
        /// <param name="mode">The current <see cref="ObjectType"/>.</param>
        internal static void SpawnObject(Vector3 position, ObjectType mode)
        {
            GameObject gameObject = Object.Instantiate(mode.GetObjectByMode(), position, Quaternion.identity);
            gameObject.transform.rotation = GetRelativeRotation(Vector3.zero, Map.FindParentRoom(gameObject));

            switch (mode)
            {
                case ObjectType.LczDoor:
                case ObjectType.HczDoor:
                case ObjectType.EzDoor:
                    {
                        gameObject.AddComponent<DoorObject>().Init(new DoorSerializable());
                        break;
                    }

                case ObjectType.WorkStation:
                    {
                        gameObject.AddComponent<WorkstationObject>().Init(new WorkstationSerializable());
                        break;
                    }

                case ObjectType.ItemSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 0.1f;
                        gameObject.AddComponent<ItemSpawnPointObject>().Init(new ItemSpawnPointSerializable());
                        break;
                    }

                case ObjectType.PlayerSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 0.25f;
                        gameObject.AddComponent<PlayerSpawnPointObject>().Init(new PlayerSpawnPointSerializable());
                        break;
                    }

                case ObjectType.RagdollSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 1.5f;
                        gameObject.AddComponent<RagdollSpawnPointObject>().Init(new RagdollSpawnPointSerializable());
                        break;
                    }

                case ObjectType.DummySpawnPoint:
                    {
                        break;
                    }

                case ObjectType.SportShootingTarget:
                case ObjectType.DboyShootingTarget:
                case ObjectType.BinaryShootingTarget:
                    {
                        gameObject.AddComponent<ShootingTargetObject>().Init(new ShootingTargetSerializable());
                        break;
                    }

                case ObjectType.Primitive:
                    {
                        gameObject.transform.position += Vector3.up * 0.5f;
                        gameObject.AddComponent<PrimitiveObject>().Init(new PrimitiveSerializable());
                        break;
                    }

                case ObjectType.LightSource:
                    {
                        gameObject.transform.position += Vector3.up * 0.5f;
                        gameObject.AddComponent<LightSourceObject>().Init(new LightSourceSerializable());
                        break;
                    }

                case ObjectType.RoomLight:
                    {
                        gameObject.transform.position += Vector3.up * 0.25f;
                        gameObject.AddComponent<RoomLightObject>().Init(new RoomLightSerializable());
                        break;
                    }

                case ObjectType.Teleporter:
                    {
                        gameObject.transform.position += Vector3.up;
                        gameObject.AddComponent<TeleportControllerObject>().Init(new TeleportSerializable());
                        break;
                    }

                case ObjectType.PedestalLocker:
                case ObjectType.LargeGunLocker:
                case ObjectType.RifleRackLocker:
                case ObjectType.MiscLocker:
                case ObjectType.MedkitLocker:
                case ObjectType.AdrenalineLocker:
                    {
                        gameObject.AddComponent<LockerObject>().Init(new LockerSerializable(), true);
                        break;
                    }
            }

            if (gameObject.TryGetComponent(out MapEditorObject mapObject))
            {
                SpawnedObjects.Add(mapObject);

                if (Config.ShowIndicatorOnSpawn)
                    Timing.CallDelayed(0.1f, () => mapObject.UpdateIndicator());
            }
        }

        /// <summary>
        /// Tries getting <see cref="MapEditorObject"/> through Raycasting.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> that is used in raycasting.</param>
        /// <param name="mapObject">The found <see cref="MapEditorObject"/>. May be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="MapEditorObject"/> was found, otherwise <see langword="false"/>.</returns>
        internal static bool TryGetMapObject(Player player, out MapEditorObject mapObject)
        {
            Vector3 forward = player.CameraTransform.forward;
            if (Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
            {
                mapObject = hit.collider.GetComponentInParent<MapEditorObject>();

                if (mapObject != null)
                {
                    if (mapObject.TryGetComponent(out IndicatorObject indicator) && indicator != null)
                    {
                        mapObject = indicator.AttachedMapEditorObject;
                        return true;
                    }

                    if (mapObject.transform.root.TryGetComponent(out SchematicObject schematicObject) && schematicObject != null)
                    {
                        mapObject = schematicObject;
                        return true;
                    }
                }

                foreach (Vector3 pos in RoomLightObject.FlickerableLightsPositions)
                {
                    float sqrDistance = (pos - hit.point).sqrMagnitude;

                    if (sqrDistance <= 2.5f)
                    {
                        mapObject = SpawnedObjects.FirstOrDefault(x => x is RoomLightObject lightComp && lightComp.RoomType == Map.FindParentRoom(hit.collider.gameObject).Type);
                        break;
                    }
                }

                return mapObject != null;
            }

            mapObject = null;
            return false;
        }

        /// <summary>
        /// Copies the <see cref="MapEditorObject"/>.
        /// </summary>
        /// <param name="player">The player that copies the object.</param>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> to copy.</param>
        internal static void CopyObject(Player player, MapEditorObject mapObject)
        {
            if (mapObject != null && SpawnedObjects.Contains(mapObject))
            {
                if (!player.SessionVariables.ContainsKey(CopiedObjectSessionVarName))
                {
                    player.SessionVariables.Add(CopiedObjectSessionVarName, mapObject);
                }
                else
                {
                    player.SessionVariables[CopiedObjectSessionVarName] = mapObject;
                }

                player.ShowHint("Object properties have been copied to the ToolGun.");
            }
            else if (player.SessionVariables.ContainsKey(CopiedObjectSessionVarName))
            {
                player.SessionVariables.Remove(CopiedObjectSessionVarName);
                player.ShowHint("ToolGun has been reseted to default settings.");
            }
        }

        /// <summary>
        /// Selects the <see cref="MapEditorObject"/>.
        /// </summary>
        /// <param name="player">The player that selects the object.</param>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> to select.</param>
        /// <returns><see langword="true"/> if the object was selected; otherwise, <see langword="false"/>.</returns>
        internal static bool SelectObject(Player player, MapEditorObject mapObject)
        {
            if (mapObject != null && (SpawnedObjects.Contains(mapObject) || mapObject is TeleportObject))
            {
                player.ShowGameObjectHint(mapObject);
                mapObject.prevOwner = player;

                if (!player.SessionVariables.ContainsKey(SelectedObjectSessionVarName))
                {
                    player.SessionVariables.Add(SelectedObjectSessionVarName, mapObject);
                }
                else
                {
                    player.SessionVariables[SelectedObjectSessionVarName] = mapObject;
                }

                return true;
            }
            else if (player.TryGetSessionVariable(SelectedObjectSessionVarName, out mapObject))
            {
                mapObject.prevOwner = null;
                player.SessionVariables.Remove(SelectedObjectSessionVarName);
                player.ShowHint("Object has been unselected");
            }

            return false;
        }

        /// <summary>
        /// Deletes the <see cref="MapEditorObject"/>.
        /// </summary>
        /// <param name="player">The player that deletes the object.</param>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> to delete.</param>
        internal static unsafe void DeleteObject(Player player, MapEditorObject mapObject)
        {
            MapEditorObject indicator = mapObject.AttachedIndicator;
            if (indicator != null)
            {
                SpawnedObjects.Remove(indicator);
                indicator.Destroy();

                if (mapObject is TeleportObject teleport)
                {
                    if (teleport.IsEntrance)
                    {
                        foreach (var exit in teleport.Controller.ExitTeleports)
                        {
                            SpawnedObjects.Remove(exit.AttachedIndicator);
                            exit.AttachedIndicator.Destroy();
                            exit.Destroy();
                        }
                    }
                    else
                    {
                        if (teleport.Controller.ExitTeleports.Count == 1)
                        {
                            SpawnedObjects.Remove(teleport.AttachedIndicator);
                            teleport.AttachedIndicator.Destroy();
                            teleport.Destroy();

                            SpawnedObjects.Remove(teleport.Controller.EntranceTeleport.AttachedIndicator);
                            teleport.Controller.EntranceTeleport.AttachedIndicator.Destroy();
                            teleport.Controller.EntranceTeleport.Destroy();

                            return;
                        }
                    }
                }
            }

            if (player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject selectedObject) && selectedObject == mapObject)
            {
                player.SessionVariables.Remove(SelectedObjectSessionVarName);
                player.ShowHint(string.Empty, 0.1f);
            }

            player.RemoteAdminMessage(mapObject.ToString());

            if (mapObject.transform.parent != null && mapObject.transform.parent.TryGetComponent(out MapEditorObject mapEditorObject))
                mapObject = mapEditorObject;

            SpawnedObjects.Remove(mapObject);
            mapObject.Destroy();
        }

        /// <summary>
        /// Gets a <see cref="string"/> which represents the ToolGun mode.
        /// </summary>
        /// <param name="player">The owner of the ToolGun.</param>
        /// <param name="isAiming">A value indicating whether the owner is aiming down.</param>
        /// <param name="flashlightEnabled">A value indicating whether the flashlight is enabled.</param>
        /// <returns>The corresponding ToolGun mode string.</returns>
        internal static string GetToolGunModeText(Player player, bool isAiming, bool flashlightEnabled) => isAiming ? flashlightEnabled ? Translation.ModeSelecting : Translation.ModeCopying : flashlightEnabled ? $"{Translation.ModeCreating}\n<b>({ToolGuns[player.CurrentItem.Serial]})</b>" : Translation.ModeDeleting;

        private static readonly Translation Translation = MapEditorReborn.Singleton.Translation;
        private static readonly Config Config = MapEditorReborn.Singleton.Config;
    }
}
