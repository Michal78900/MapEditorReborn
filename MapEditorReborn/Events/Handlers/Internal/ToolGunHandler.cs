// -----------------------------------------------------------------------
// <copyright file="ToolGunHandler.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.Handlers.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using API.Enums;
    using API.Extensions;
    using API.Features;
    using API.Features.Objects;
    using API.Features.Serializable;
    using Configs;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using UnityEngine;
    using static API.API;

    /// <summary>
    /// A tool to easily handle the ToolGun behavior.
    /// </summary>
    internal static class ToolGunHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnAimingDownSight(AimingDownSightEventArgs)"/>
        internal static void OnAimingDownSight(AimingDownSightEventArgs ev)
        {
            if (!ev.Player.CurrentItem.IsToolGun() || (ev.Player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null))
                return;

            ev.Player.ShowHint(GetToolGunModeText(ev.Player, ev.AdsIn, ev.Player.HasFlashlightModuleEnabled), 1f);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs)"/>
        internal static void OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs ev)
        {
            if (ev.Player == null ||
                (ev.Firearm.FlashlightEnabled && ev.NewState) ||
                (!ev.Firearm.FlashlightEnabled && !ev.NewState) ||
                !ev.Player.CurrentItem.IsToolGun() ||
                (ev.Player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) &&
                 mapObject != null))
                return;

            ev.Player.ShowHint(GetToolGunModeText(ev.Player, ev.Player.IsAimingDownWeapon, ev.NewState), 1f);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnUnloadingWeapon(UnloadingWeaponEventArgs)"/>
        internal static void OnUnloadingWeapon(UnloadingWeaponEventArgs ev)
        {
            if (!ev.Firearm.IsToolGun())
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDroppingItem(DroppingItemEventArgs)"/>
        internal static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!ev.Item.IsToolGun() || !ev.IsThrown)
                return;

            ev.IsAllowed = false;
            ToolGuns[ev.Player.CurrentItem.Serial]++;

            if ((int)ToolGuns[ev.Player.CurrentItem.Serial] > ObjectPrefabs.Count - 1)
            {
                ToolGuns[ev.Player.CurrentItem.Serial] = 0;
            }

            ObjectType mode = ToolGuns[ev.Player.CurrentItem.Serial];

            ev.Player.ClearBroadcasts();
            ev.Player.Broadcast(1, !ev.Player.IsAimingDownWeapon && ev.Player.HasFlashlightModuleEnabled ? $"{Translation.ModeCreating}\n<b>({mode})</b>" : $"<b>{mode}</b>");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnShooting(ShootingEventArgs)"/>
        internal static void OnShooting(ShootingEventArgs ev)
        {
            if (!ev.Player.CurrentItem.IsToolGun())
                return;

            ev.IsAllowed = false;

            // Creating an object
            if (ev.Player.HasFlashlightModuleEnabled && !ev.Player.IsAimingDownWeapon)
            {
                Vector3 forward = ev.Player.CameraTransform.forward;
                if (Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
                {
                    ObjectType mode = ToolGuns[ev.Player.CurrentItem.Serial];

                    if (mode == ObjectType.RoomLight)
                    {
                        Room colliderRoom = Room.FindParentRoom(hit.collider.gameObject);
                        if (SpawnedObjects.FirstOrDefault(x => x is RoomLightObject light && light.ForcedRoomType == colliderRoom.Type) != null)
                        {
                            ev.Player.ShowHint("There can be only one Light Controller per one room type!");
                            return;
                        }
                    }

                    if (ev.Player.TryGetSessionVariable(CopiedObjectSessionVarName, out MapEditorObject prefab) && prefab != null)
                    {
                        SpawnedObjects.Add(ObjectSpawner.SpawnPropertyObject(hit.point, prefab));

                        if (MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn)
                            SpawnedObjects.Last().UpdateIndicator();
                    }
                    else
                    {
                        SpawnObject(hit.point, mode);
                    }
                }

                return;
            }

            if (TryGetMapObject(ev.Player, out MapEditorObject mapObject))
            {
                // Deleting the object
                if (!ev.Player.HasFlashlightModuleEnabled && !ev.Player.IsAimingDownWeapon)
                {
                    DeleteObject(ev.Player, mapObject);
                    return;
                }
            }

            // Copying to the ToolGun
            if (!ev.Player.HasFlashlightModuleEnabled && ev.Player.IsAimingDownWeapon)
            {
                CopyObject(ev.Player, mapObject);
                return;
            }

            // Selecting the object
            if (ev.Player.HasFlashlightModuleEnabled && ev.Player.IsAimingDownWeapon)
            {
                SelectObject(ev.Player, mapObject);
            }
        }

        /// <summary>
        /// Spawns a general <see cref="MapEditorObject"/>.
        /// Used by the ToolGun.
        /// </summary>
        /// <param name="position">The position of the spawned object.</param>
        /// <param name="mode">The current <see cref="ObjectType"/>.</param>
        internal static GameObject SpawnObject(Vector3 position, ObjectType mode)
        {
            GameObject gameObject = Object.Instantiate(mode.GetObjectByMode(), position, Quaternion.identity);
            gameObject.transform.rotation = GetRelativeRotation(Vector3.zero, Room.FindParentRoom(gameObject));

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
                        gameObject.AddComponent<TeleportObject>().Init(new SerializableTeleport(), true);
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

            return gameObject;
        }

        /// <summary>
        /// Tries getting <see cref="MapEditorObject"/> through Raycasting.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> that is used in raycasting.</param>
        /// <param name="mapObject">The found <see cref="MapEditorObject"/>. May be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="MapEditorObject"/> was found, otherwise <see langword="false"/>.</returns>
        internal static bool TryGetMapObject(Player player, out MapEditorObject mapObject)
        {
            List<Collider> tempColliders = NorthwoodLib.Pools.ListPool<Collider>.Shared.Rent();
            foreach (AdminToys.PrimitiveObjectToy primitive in Object.FindObjectsOfType<AdminToys.PrimitiveObjectToy>())
            {
                if (primitive.TryGetComponent(out Collider collider) && !collider.enabled)
                    collider.enabled = true;

                tempColliders.Add(collider);
            }

            Vector3 forward = player.CameraTransform.forward;
            if (Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
            {
                mapObject = hit.collider.GetComponentInParent<MapEditorObject>();

                if (mapObject != null)
                {
                    if (mapObject.TryGetComponent(out IndicatorObject indicator) && indicator != null)
                    {
                        if (indicator.AttachedMapEditorObject is TeleportObject)
                        {
                            // Select the whole schematic, not a single teleport.
                            if (indicator.AttachedMapEditorObject.IsSchematicBlock)
                                mapObject = indicator.AttachedMapEditorObject.GetComponentInParent<SchematicObject>();
                        }
                        else
                        {
                            mapObject = indicator.AttachedMapEditorObject;
                        }

                        return true;
                    }

                    // Don't return teleports with no indicator enabled.
                    if (mapObject is TeleportObject && mapObject.AttachedIndicator == null)
                    {
                        mapObject = null;
                        return false;
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
                        mapObject = SpawnedObjects.FirstOrDefault(x => x is RoomLightObject lightComp && lightComp.RoomType == Room.FindParentRoom(hit.collider.gameObject).Type);
                        break;
                    }
                }

                return mapObject != null;
            }

            foreach (Collider collider in tempColliders)
                collider.enabled = false;

            NorthwoodLib.Pools.ListPool<Collider>.Shared.Return(tempColliders);

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

            if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out mapObject))
                return false;

            mapObject.prevOwner = null;
            player.SessionVariables.Remove(SelectedObjectSessionVarName);
            player.ShowHint("Object has been unselected");

            return false;
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
        private static string GetToolGunModeText(Player player, bool isAiming, bool flashlightEnabled) => isAiming ? flashlightEnabled ? Translation.ModeSelecting : Translation.ModeCopying : flashlightEnabled ? $"{Translation.ModeCreating}\n<b>({ToolGuns[player.CurrentItem.Serial]})</b>" : Translation.ModeDeleting;

        private static readonly Translation Translation = MapEditorReborn.Singleton.Translation;
        private static readonly Config Config = MapEditorReborn.Singleton.Config;
    }
}
