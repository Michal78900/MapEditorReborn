namespace MapEditorReborn.Events.Handlers.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using AdminToys;
    using API.Enums;
    using API.Extensions;
    using API.Features;
    using API.Features.Objects;
    using API.Features.Serializable;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Loader;
    using MapGeneration;
    using MapGeneration.Distributors;
    using MEC;
    using Mirror;
    using UnityEngine;

    using static API.API;

    using Config = Config;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Handles mostly EXILED events.
    /// </summary>
    internal static class EventHandler
    {
        internal static void OnVerified(VerifiedEventArgs ev)
        {
            if (Config.CullingSize == Vector3.zero)
                return;

            new GameObject("CullingObject")
            {
                transform =
                {
                    parent = ev.Player.CameraTransform,
                    localPosition = Vector3.zero,
                },
            }.AddComponent<API.Features.Components.CullingComponent>().Init(ev.Player);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnGenerated"/>
        internal static void OnGenerated()
        {
            _roomTypes = null;
            SpawnedObjects.Clear();

            Dictionary<ObjectType, GameObject> objectList = new Dictionary<ObjectType, GameObject>(21);
            DoorSpawnpoint[] doorList = Object.FindObjectsOfType<DoorSpawnpoint>();
            SpawnableStructure[] structureList = Resources.LoadAll<SpawnablesDistributorSettings>(string.Empty)[0].SpawnableStructures;

            objectList.Add(ObjectType.LczDoor, doorList.First(x => x.TargetPrefab.name.Contains("LCZ")).TargetPrefab.gameObject);
            objectList.Add(ObjectType.HczDoor, doorList.First(x => x.TargetPrefab.name.Contains("HCZ")).TargetPrefab.gameObject);
            objectList.Add(ObjectType.EzDoor, doorList.First(x => x.TargetPrefab.name.Contains("EZ")).TargetPrefab.gameObject);

            objectList.Add(ObjectType.WorkStation, structureList[8].gameObject);

            objectList.Add(ObjectType.ItemSpawnPoint, new GameObject("ItemSpawnPointObject"));
            objectList.Add(ObjectType.PlayerSpawnPoint, new GameObject("PlayerSpawnPointObject"));
            objectList.Add(ObjectType.RagdollSpawnPoint, new GameObject("RagdollSpawnPointObject"));
            objectList.Add(ObjectType.DummySpawnPoint, new GameObject("DummySpawnPointObject"));

            foreach (GameObject gameObject in NetworkClient.prefabs.Values)
            {
                if (gameObject.name == "sportTargetPrefab")
                {
                    objectList.Add(ObjectType.SportShootingTarget, gameObject);
                    continue;
                }

                if (gameObject.name == "dboyTargetPrefab")
                {
                    objectList.Add(ObjectType.DboyShootingTarget, gameObject);
                    continue;
                }

                if (gameObject.name == "binaryTargetPrefab")
                {
                    objectList.Add(ObjectType.BinaryShootingTarget, gameObject);
                    continue;
                }

                if (gameObject.TryGetComponent(out PrimitiveObjectToy _))
                {
                    objectList.Add(ObjectType.Primitive, gameObject);
                    continue;
                }

                if (gameObject.TryGetComponent(out LightSourceToy _))
                {
                    objectList.Add(ObjectType.LightSource, gameObject);
                }
            }

            objectList.Add(ObjectType.RoomLight, new GameObject("LightControllerObject"));
            objectList.Add(ObjectType.Teleporter, new GameObject("TeleportControllerObject"));

            objectList.Add(ObjectType.PedestalLocker, structureList[0].gameObject);
            objectList.Add(ObjectType.LargeGunLocker, structureList[4].gameObject);
            objectList.Add(ObjectType.RifleRackLocker, structureList[5].gameObject);
            objectList.Add(ObjectType.MiscLocker, structureList[6].gameObject);
            objectList.Add(ObjectType.MedkitLocker, structureList[9].gameObject);
            objectList.Add(ObjectType.AdrenalineLocker, structureList[10].gameObject);

            ObjectPrefabs = new ReadOnlyDictionary<ObjectType, GameObject>(objectList);

            PlayerSpawnPointObject.RegisterSpawnPoints();

            Timing.CallDelayed(1f, () =>
            {
                if (MapUtils.TryGetRandomMap(Config.LoadMapOnEvent.OnGenerated, out MapSchematic mapSchematic))
                    CurrentLoadedMap = mapSchematic;
            });
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers()"/>
        internal static void OnWaitingForPlayers()
        {
            RoomLightObject.RegisterFlickerableLights();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted()"/>
        internal static void OnRoundStarted()
        {
            if (MapUtils.TryGetRandomMap(Config.LoadMapOnEvent.OnRoundStarted, out MapSchematic mapSchematic))
                CurrentLoadedMap = mapSchematic;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Warhead.OnDetonated()"/>
        internal static void OnWarheadDetonated()
        {
            if (MapUtils.TryGetRandomMap(Config.LoadMapOnEvent.OnWarheadDetonated, out MapSchematic mapSchematic))
                CurrentLoadedMap = mapSchematic;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDroppingItem(DroppingItemEventArgs)"/>
        internal static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Item.IsToolGun() && ev.IsThrown)
            {
                ev.IsAllowed = false;

                ToolGuns[ev.Player.CurrentItem.Serial]++;

                if ((int)ToolGuns[ev.Player.CurrentItem.Serial] > ObjectPrefabs.Count - 1)
                {
                    ToolGuns[ev.Player.CurrentItem.Serial] = 0;
                }

                ObjectType mode = ToolGuns[ev.Player.CurrentItem.Serial];

                // ev.Player.ShowHint(!ev.Player.IsAimingDownWeapon && ev.Player.HasFlashlightModuleEnabled ? $"{Translation.ModeCreating}\n<b>({mode})</b>" : $"<b>{mode}</b>", 1f);
                ev.Player.ClearBroadcasts();
                ev.Player.Broadcast(1, !ev.Player.IsAimingDownWeapon && ev.Player.HasFlashlightModuleEnabled ? $"{Translation.ModeCreating}\n<b>({mode})</b>" : $"<b>{mode}</b>");
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnShooting(ShootingEventArgs)"/>
        internal static void OnShooting(ShootingEventArgs ev)
        {
            if (!ev.Shooter.CurrentItem.IsToolGun())
                return;

            ev.IsAllowed = false;

            // Creating an object
            if (ev.Shooter.HasFlashlightModuleEnabled && !ev.Shooter.IsAimingDownWeapon)
            {
                Vector3 forward = ev.Shooter.CameraTransform.forward;
                if (Physics.Raycast(ev.Shooter.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
                {
                    ObjectType mode = ToolGuns[ev.Shooter.CurrentItem.Serial];

                    if (mode == ObjectType.RoomLight)
                    {
                        Room colliderRoom = Map.FindParentRoom(hit.collider.gameObject);
                        if (SpawnedObjects.FirstOrDefault(x => x is RoomLightObject light && light.ForcedRoomType == colliderRoom.Type) != null)
                        {
                            ev.Shooter.ShowHint("There can be only one Light Controller per one room type!");
                            return;
                        }
                    }

                    if (ev.Shooter.TryGetSessionVariable(CopiedObjectSessionVarName, out MapEditorObject prefab) && prefab != null)
                    {
                        SpawnedObjects.Add(ObjectSpawner.SpawnPropertyObject(hit.point, prefab));

                        if (MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn)
                            SpawnedObjects.Last().UpdateIndicator();
                    }
                    else
                    {
                        ToolGunHandler.SpawnObject(hit.point, mode);
                    }
                }

                return;
            }

            if (ToolGunHandler.TryGetMapObject(ev.Shooter, out MapEditorObject mapObject))
            {
                // Deleting the object
                if (!ev.Shooter.HasFlashlightModuleEnabled && !ev.Shooter.IsAimingDownWeapon)
                {
                    ToolGunHandler.DeleteObject(ev.Shooter, mapObject);
                    return;
                }
            }

            // Copying to the ToolGun
            if (!ev.Shooter.HasFlashlightModuleEnabled && ev.Shooter.IsAimingDownWeapon)
            {
                ToolGunHandler.CopyObject(ev.Shooter, mapObject);
                return;
            }

            // Selecting the object
            if (ev.Shooter.HasFlashlightModuleEnabled && ev.Shooter.IsAimingDownWeapon)
            {
                ToolGunHandler.SelectObject(ev.Shooter, mapObject);
                return;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInteractingShootingTarget(InteractingShootingTargetEventArgs)"/>
        internal static void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev)
        {
            if (!ev.ShootingTarget.Base.TryGetComponent(out ShootingTargetObject shootingTargetComponent) || shootingTargetComponent == null)
                return;

            if (ev.TargetButton == ShootingTargetButton.Remove)
                ev.IsAllowed = false;
        }

        /// <inheritdoc cref="FileSystemWatcher.OnChanged(FileSystemEventArgs)"/>
        internal static void OnFileChanged(object sender, FileSystemEventArgs ev)
        {
            if (!Config.EnableFileSystemWatcher)
                return;

            string fileName = Path.GetFileNameWithoutExtension(ev.Name);

            if (fileName == CurrentLoadedMap?.Name)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    try
                    {
                        Log.Debug("Trying to deserialize the file... (called by FileSytemWatcher)", Config.Debug);
                        CurrentLoadedMap = Loader.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(ev.FullPath));
                        CurrentLoadedMap.Name = fileName;
                    }
                    catch (Exception e)
                    {
                        Log.Error($"You did something wrong in your MapSchematic file!\n{e.Message}");
                    }
                });
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnAimingDownSight(AimingDownSightEventArgs)"/>
        internal static void OnAimingDownSight(AimingDownSightEventArgs ev)
        {
            if (!ev.Player.CurrentItem.IsToolGun() || (ev.Player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null))
                return;

            ev.Player.ShowHint(ToolGunHandler.GetToolGunModeText(ev.Player, ev.AdsIn, ev.Player.HasFlashlightModuleEnabled), 1f);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDamagingShootingTarget(DamagingShootingTargetEventArgs)"/>
        internal static void OnDamagingShootingTarget(DamagingShootingTargetEventArgs ev)
        {
            if (ev.ShootingTarget.Base.TryGetComponent(out ShootingTargetObject shootingTargetComponent) && shootingTargetComponent.Base.IsFunctional)
                return;

            ev.IsAllowed = false;
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

            ev.Player.ShowHint(ToolGunHandler.GetToolGunModeText(ev.Player, ev.Player.IsAimingDownWeapon, ev.NewState), 1f);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnUnloadingWeapon(UnloadingWeaponEventArgs)"/>
        internal static void OnUnloadingWeapon(UnloadingWeaponEventArgs ev)
        {
            if (!ev.Firearm.IsToolGun())
                return;

            ev.IsAllowed = false;
        }

        /*
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnSearchPickupRequest(SearchingPickupEventArgs)"/>
        internal static void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (!ItemSpawnPointObject.LockedPickups.Contains(ev.Pickup))
                return;

            ev.IsAllowed = false;
            Schematic.OnButtonInteract(new ButtonInteractedEventArgs(ev.Pickup, ev.Player, ev.Pickup.Base.GetComponentInParent<SchematicObject>()));
        }
        */

        private static readonly Config Config = MapEditorReborn.Singleton.Config;
        private static readonly Translation Translation = MapEditorReborn.Singleton.Translation;
    }
}