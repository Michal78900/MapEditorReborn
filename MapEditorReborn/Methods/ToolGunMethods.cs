namespace MapEditorReborn
{
    using System.Linq;
    using API;
    using API.Enums;
    using API.Extensions;
    using Exiled.API.Features;
    using MEC;
    using UnityEngine;

    public static partial class Methods
    {
        /// <summary>
        /// Spawns a general <see cref="MapEditorObject"/>.
        /// Used by the ToolGun.
        /// </summary>
        /// <param name="position">The postition of the spawned object.</param>
        /// <param name="mode">The current <see cref="ToolGunMode"/>.</param>
        internal static void SpawnObject(Vector3 position, ToolGunMode mode)
        {
            GameObject gameObject = Object.Instantiate(mode.GetObjectByMode(), position, Quaternion.identity);
            gameObject.transform.rotation = GetRelativeRotation(Vector3.zero, Map.FindParentRoom(gameObject));

            switch (mode)
            {
                case ToolGunMode.LczDoor:
                case ToolGunMode.HczDoor:
                case ToolGunMode.EzDoor:
                    {
                        gameObject.AddComponent<DoorObjectComponent>().Init(new DoorObject());
                        break;
                    }

                case ToolGunMode.WorkStation:
                    {
                        gameObject.AddComponent<WorkStationObjectComponent>().Init(new WorkStationObject());
                        break;
                    }

                case ToolGunMode.ItemSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 0.1f;
                        gameObject.AddComponent<ItemSpawnPointComponent>().Init(new ItemSpawnPointObject());
                        break;
                    }

                case ToolGunMode.PlayerSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 0.25f;
                        gameObject.AddComponent<PlayerSpawnPointComponent>().Init(new PlayerSpawnPointObject());
                        break;
                    }

                case ToolGunMode.RagdollSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 1.5f;
                        gameObject.AddComponent<RagdollSpawnPointComponent>().Init(new RagdollSpawnPointObject());
                        break;
                    }

                case ToolGunMode.DummySpawnPoint:
                    {
                        break;
                    }

                case ToolGunMode.SportShootingTarget:
                case ToolGunMode.DboyShootingTarget:
                case ToolGunMode.BinaryShootingTarget:
                    {
                        gameObject.AddComponent<ShootingTargetComponent>().Init(new ShootingTargetObject());
                        break;
                    }

                case ToolGunMode.Primitive:
                    {
                        gameObject.transform.position += Vector3.up * 0.5f;
                        gameObject.AddComponent<PrimitiveObjectComponent>().Init(new PrimitiveObject());
                        break;
                    }

                case ToolGunMode.LightSource:
                    {
                        gameObject.transform.position += Vector3.up * 0.5f;
                        gameObject.AddComponent<LightSourceComponent>().Init(new LightSourceObject());
                        break;
                    }

                case ToolGunMode.RoomLight:
                    {
                        gameObject.transform.position += Vector3.up * 0.25f;
                        gameObject.AddComponent<RoomLightComponent>().Init(new RoomLightObject());
                        break;
                    }

                case ToolGunMode.Teleporter:
                    {
                        gameObject.transform.position += Vector3.up;
                        gameObject.AddComponent<TeleportControllerComponent>().Init(new TeleportObject());
                        break;
                    }
            }

            MapEditorObject mapObject = gameObject.GetComponent<MapEditorObject>();
            SpawnedObjects.Add(mapObject);

            if (Config.ShowIndicatorOnSpawn)
                Timing.CallDelayed(0.1f, () => mapObject.UpdateIndicator());
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

                IndicatorObjectComponent indicator = mapObject?.GetComponent<IndicatorObjectComponent>();
                if (indicator != null)
                {
                    mapObject = indicator.AttachedMapEditorObject;
                    return true;
                }

                SchematicBlockComponent schematicBlock = mapObject?.GetComponent<SchematicBlockComponent>();
                if (schematicBlock != null)
                {
                    mapObject = schematicBlock.AttachedSchematic;
                    return true;
                }

                if (mapObject == null)
                {
                    foreach (Vector3 pos in RoomLightComponent.FlickerableLightsPositions)
                    {
                        float sqrDistance = (pos - hit.point).sqrMagnitude;

                        if (sqrDistance <= 2.5f)
                        {
                            mapObject = SpawnedObjects.FirstOrDefault(x => x is RoomLightComponent lightComp && lightComp.RoomType == Map.FindParentRoom(hit.collider.gameObject).Type);
                            break;
                        }
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
        internal static void SelectObject(Player player, MapEditorObject mapObject)
        {
            if (mapObject != null && (SpawnedObjects.Contains(mapObject) || mapObject is TeleportComponent))
            {
                player.ShowGameObjectHint(mapObject);

                if (!player.SessionVariables.ContainsKey(SelectedObjectSessionVarName))
                {
                    player.SessionVariables.Add(SelectedObjectSessionVarName, mapObject);
                }
                else
                {
                    player.SessionVariables[SelectedObjectSessionVarName] = mapObject;
                }
            }
            else if (player.SessionVariables.ContainsKey(SelectedObjectSessionVarName))
            {
                player.SessionVariables.Remove(SelectedObjectSessionVarName);
                player.ShowHint("Object has been unselected");
            }
        }

        /// <summary>
        /// Deletes the <see cref="MapEditorObject"/>.
        /// </summary>
        /// <param name="player">The player that deletes the object.</param>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> to delete.</param>
        internal static void DeleteObject(Player player, MapEditorObject mapObject)
        {
            MapEditorObject indicator = mapObject.AttachedIndicator;
            if (indicator != null)
            {
                SpawnedObjects.Remove(indicator);
                indicator.Destroy();

                if (mapObject is TeleportComponent teleport)
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

            if (mapObject.transform.parent != null)
                mapObject = mapObject.transform.parent.GetComponent<MapEditorObject>();

            SpawnedObjects.Remove(mapObject);
            mapObject.Destroy();
        }

        internal static string GetToolGunModeText(Player player, bool isAiming, bool flashlightEnabled) => isAiming ? flashlightEnabled ? Translation.ModeSelecting : Translation.ModeCopying : flashlightEnabled ? $"{Translation.ModeCreating}\n<b>({ToolGuns[player.CurrentItem.Serial]})</b>" : Translation.ModeDeleting;
    }
}
