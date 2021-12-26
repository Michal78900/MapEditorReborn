namespace MapEditorReborn.API.Extensions
{
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using UnityEngine;

    /// <summary>
    /// The extensions class containig a few useful methods.
    /// </summary>
    public static class Extenstions
    {
        /// <summary>
        /// Checks if the <see cref="Inventory.SyncItemInfo"/> is a ToolGun.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns><see langword="true"/> if the <paramref name="item"/> is a Tool Gun, otherwise <see langword="false"/>.</returns>
        public static bool IsToolGun(this Item item) => item != null && Methods.ToolGuns.ContainsKey(item.Serial);

        /// <summary>
        /// Used for showing details about the <see cref="GameObject"/> to a specifc <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to which the message should be shown.</param>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> which details are gonna be shown.</param>
        public static void ShowGameObjectHint(this Player player, MapEditorObject mapObject)
        {
            string message = "<size=30>Selected object type: <color=yellow><b>{objectType}</b></color></size>\n";

            if (!(mapObject is RoomLightComponent))
            {
                Vector3 relativePosition = mapObject.RelativePosition;
                Vector3 relativeRotation = mapObject.RelativeRotation;

                message += $"<size=20>" +
                           $"Position {string.Format("X: <color=yellow><b>{0:F3}</b></color> Y: <color=yellow><b>{1:F3}</b></color> Z: <color=yellow><b>{2:F3}</b></color>", relativePosition.x, relativePosition.y, relativePosition.z)} | " +
                           $"Rotation {string.Format("X: <color=yellow><b>{0:F3}</b></color> Y: <color=yellow><b>{1:F3}</b></color> Z: <color=yellow><b>{2:F3}</b></color>", relativeRotation.x, relativeRotation.y, relativeRotation.z)} | " +
                           $"Scale {string.Format("X: <color=yellow><b>{0:F3}</b></color> Y: <color=yellow><b>{1:F3}</b></color> Z: <color=yellow><b>{2:F3}</b></color>", mapObject.Scale.x, mapObject.Scale.y, mapObject.Scale.z)}\n" +
                           $"RoomType: <color=yellow><b>{mapObject.RoomType}</b></color></size>" +
                           $"</size>\n";
            }

            switch (mapObject)
            {
                case DoorObjectComponent door:
                    {
                        message = message.Replace("{objectType}", door.Base.DoorType.ToString());

                        message += $"<size=20>" +
                                   $"IsOpened: {(door.Base.IsOpen ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}\n" +
                                   $"IsLocked: {(door.Base.IsLocked ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}\n" +
                                   $"KeycardPermissions: <color=yellow><b>{door.Base.KeycardPermissions} ({(ushort)door.Base.KeycardPermissions})</b></color>\n" +
                                   $"IgnoredDamageSources: <color=yellow><b>{door.Base.IgnoredDamageSources} ({(byte)door.Base.IgnoredDamageSources})</b></color>\n" +
                                   $"DoorHealth: <color=yellow><b>{door.Base.DoorHealth}</b></color>\n" +
                                   $"OpenOnWarheadActivation: {(door.Base.OpenOnWarheadActivation ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}" +
                                   $"</size>";

                        break;
                    }

                case WorkStationObjectComponent workstation:
                    {
                        message = message.Replace("{objectType}", "Workstation");

                        message += $"<size=20>" +
                                   $"IsInteractable: {(workstation.Base.IsInteractable ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}</b></color>" +
                                   $"</size>";
                        break;
                    }

                case PlayerSpawnPointComponent playerSpawnPoint:
                    {
                        message = message.Replace("{objectType}", "PlayerSpawnPoint");

                        RoleType type = playerSpawnPoint.tag.ConvertToRoleType();
                        string name = type.ToString();

                        switch (type)
                        {
                            case RoleType.NtfPrivate:
                                name = "MTF";
                                break;

                            case RoleType.Scp93953:
                                name = "SCP939";
                                break;
                        }

                        message += $"<size=20>SpawnpointType: <color=yellow><b>{name}</b></color></size>";

                        break;
                    }

                case ItemSpawnPointComponent itemSpawnPoint:
                    {
                        message = message.Replace("{objectType}", "ItemSpawnPoint");

                        message += $"<size=20>" +
                                   $"Item: <color=yellow><b>{itemSpawnPoint.Base.Item}</b></color>\n" +
                                   $"AttachmentsCode: <color=yellow><b>{itemSpawnPoint.Base.AttachmentsCode}</b></color>\n" +
                                   $"SpawnChance: <color=yellow><b>{itemSpawnPoint.Base.SpawnChance}</b></color>\n" +
                                   $"NumberOfItems: <color=yellow><b>{itemSpawnPoint.Base.NumberOfItems}</b></color>\n" +
                                   $"UseGravity: {(itemSpawnPoint.Base.UseGravity ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}\n" +
                                   $"CanBePickedUp: {(itemSpawnPoint.Base.CanBePickedUp ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}" +
                                   $"</size>";

                        break;
                    }

                case RagdollSpawnPointComponent ragdollSpawnPoint:
                    {
                        message = message.Replace("{objectType}", "RagdollSpawnPoint");

                        message += $"<size=20>" +
                                   $"Name: <color=yellow><b>{ragdollSpawnPoint.Base.Name}</b></color>\n" +
                                   $"RoleType: <color=yellow><b>{ragdollSpawnPoint.Base.RoleType}</b></color>\n" +
                                   $"DeathReason: <color=yellow><b>{ragdollSpawnPoint.Base.DeathReason}</b></color>\n" +
                                   $"SpawnChance: <color=yellow><b>{ragdollSpawnPoint.Base.SpawnChance}</b></color>" +
                                   $"</size>";

                        break;
                    }

                case ShootingTargetComponent shootingTarget:
                    {
                        message = message.Replace("{objectType}", shootingTarget.Base.TargetType + "ShootingTarget");

                        message += $"<size=20>" +
                                   $"Type: <color=yellow><b>{shootingTarget.Base.TargetType}</b></color>\n" +
                                   $"IsFunctional: {(shootingTarget.Base.IsFunctional ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}" +
                                   $"</size>";

                        break;
                    }

                case PrimitiveObjectComponent primitive:
                    {
                        message = message.Replace("{objectType}", $"Primitive{primitive.Base.PrimitiveType}");

                        break;
                    }

                case LightSourceComponent lightSource:
                    {
                        message = message.Replace("{objectType}", "LightSource");

                        break;
                    }

                case RoomLightComponent roomLights:
                    {
                        message = message.Replace("{objectType}", "RoomLight");

                        message += $"<size=20>" +
                                   $"RoomType: <color=yellow><b>{mapObject.ForcedRoomType}</b></color>\n" +
                                   $"Color: <color=#{ColorUtility.ToHtmlStringRGBA(roomLights.GetColorFromString(roomLights.Base.Color))}><b>{roomLights.Base.Color}</b></color>\n" +
                                   $"ShiftSpeed: <color=yellow><b>{roomLights.Base.ShiftSpeed}</b></color>\n" +
                                   $"OnlyWarheadLight: {(roomLights.Base.OnlyWarheadLight ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}" +
                                   $"</size>";

                        break;
                    }

                case TeleportComponent teleport:
                    {
                        message = message.Replace("{objectType}", $"Teleporter{(teleport.IsEntrance ? "Entrance" : "Exit")}");

                        message += $"<size=20>" +
                                   $"TeleportCooldown: <color=yellow><b>{teleport.Controller.Base.TeleportCooldown}</b></color>\n" +
                                   $"BothWayMode: {(teleport.Controller.Base.BothWayMode ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}" +
                                   $"{(teleport.IsEntrance ? string.Empty : $"\nChance: <color=yellow><b>{teleport.Chance}</b></color>")}" +
                                   $"</size>";

                        break;
                    }

                case SchematicObjectComponent schematic:
                    {
                        message = message.Replace("{objectType}", schematic.name);

                        break;
                    }
            }

            player.ShowHint(message, 9999f);
        }

        /// <summary>
        /// Gets or sets the object's <see cref="GameObject"/> prefab, by ToolGun's <see cref="ToolGunMode"/>.
        /// </summary>
        /// <param name="toolGunMode">The <see cref="ToolGunMode"/>.</param>
        /// <returns>The <see cref="GameObject"/> prefab of an object.</returns>
        public static GameObject GetObjectByMode(this ToolGunMode toolGunMode) => Methods.ObjectPrefabs.TryGetValue(toolGunMode, out GameObject obj) ? obj : null;

        /// <summary>
        /// Gets or sets the <see cref="DoorType"/> from the <see cref="Door"/> by it's name.
        /// </summary>
        /// <param name="door">The door to check."/>.</param>
        /// <returns><see cref="DoorType"/> of the door.</returns>
        public static DoorType GetDoorTypeByName(this Door door)
        {
            switch (door.Base.gameObject.name)
            {
                case "LCZ BreakableDoor(Clone)":
                    return DoorType.LightContainmentDoor;

                case "HCZ BreakableDoor(Clone)":
                    return DoorType.HeavyContainmentDoor;

                case "EZ BreakableDoor(Clone)":
                    return DoorType.EntranceDoor;

                default:
                    return DoorType.UnknownDoor;
            }
        }

        /// <summary>
        /// Gets or sets the door's <see cref="GameObject"/> prefab, by it's <see cref="DoorType"/>.
        /// </summary>
        /// <param name="doorType">The DoorType of the door.</param>
        /// <returns>The <see cref="GameObject"/> prefab of a door.</returns>
        public static GameObject GetDoorObjectByType(this DoorType doorType)
        {
            switch (doorType)
            {
                case DoorType.LightContainmentDoor:
                    return ToolGunMode.LczDoor.GetObjectByMode();

                case DoorType.HeavyContainmentDoor:
                    return ToolGunMode.HczDoor.GetObjectByMode();

                case DoorType.EntranceDoor:
                    return ToolGunMode.EzDoor.GetObjectByMode();

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the shooting target's <see cref="GameObject"/> prefab, by it's <see cref="ShootingTargetObject"/>.
        /// </summary>
        /// <param name="targetType">The <see cref="ShootingTargetType"/> of the <see cref="ShootingTargetObject"/> to check.</param>
        /// <returns>The <see cref="GameObject"/> prefab of a shooting target.</returns>
        public static GameObject GetShootingTargetObjectByType(this ShootingTargetType targetType)
        {
            switch (targetType)
            {
                case ShootingTargetType.Sport:
                    return ToolGunMode.SportShootingTarget.GetObjectByMode();

                case ShootingTargetType.ClassD:
                    return ToolGunMode.DboyShootingTarget.GetObjectByMode();

                case ShootingTargetType.Binary:
                    return ToolGunMode.BinaryShootingTarget.GetObjectByMode();

                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts a <see cref="RoleType"/> to a nametag that it's spawnpoint uses.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleType"/> to convert.</param>
        /// <returns>A name of the spawnpoint nametag or the "SP_173" when the <see cref="RoleType"/> is invalid.</returns>
        public static string ConvertToSpawnPointTag(this RoleType roleType)
        {
            switch (roleType)
            {
                case RoleType.Scp049:
                    return "SP_049";

                case RoleType.Scp079:
                    return "SP_079";

                case RoleType.Scp096:
                    return "SCP_096";

                case RoleType.Scp106:
                    return "SP_106";

                case RoleType.Scp173:
                    return "SP_173";

                case RoleType.Scp93953:
                case RoleType.Scp93989:
                    return "SCP_939";

                case RoleType.ClassD:
                    return "SP_CDP";

                case RoleType.Scientist:
                    return "SP_RSC";

                case RoleType.FacilityGuard:
                    return "SP_GUARD";

                case RoleType.NtfPrivate:
                case RoleType.NtfSergeant:
                case RoleType.NtfSpecialist:
                case RoleType.NtfCaptain:
                    return "SP_MTF";

                case RoleType.ChaosRifleman:
                case RoleType.ChaosConscript:
                case RoleType.ChaosMarauder:
                case RoleType.ChaosRepressor:
                    return "SP_CI";

                /*
            case RoleType.Tutorial:
                return "TUT Spawn";
                */

                default:
                    {
                        Log.Error($"{roleType} is an invalid role!");
                        return "Untagged";
                    }
            }
        }

        /// <summary>
        /// Converts a spawnpoint's nametag to it's <see cref="RoleType"/>.
        /// </summary>
        /// <param name="spawnPointTag">The nametag to convert.</param>
        /// <returns>The <see cref="RoleType"/>> that this nametag uses.</returns>
        public static RoleType ConvertToRoleType(this string spawnPointTag)
        {
            switch (spawnPointTag)
            {
                case "SP_049":
                    return RoleType.Scp049;

                case "SP_079":
                    return RoleType.Scp079;

                case "SCP_096":
                    return RoleType.Scp096;

                case "SP_106":
                    return RoleType.Scp106;

                case "SP_173":
                    return RoleType.Scp173;

                case "SCP_939":
                    // case RoleType.Scp93989:
                    return RoleType.Scp93953;

                case "SP_CDP":
                    return RoleType.ClassD;

                case "SP_RSC":
                    return RoleType.Scientist;

                case "SP_GUARD":
                    return RoleType.FacilityGuard;

                case "SP_MTF":
                    // case RoleType.NtfLieutenant:
                    // case RoleType.NtfScientist:
                    // case RoleType.NtfCommander:
                    return RoleType.NtfPrivate;

                case "SP_CI":
                    return RoleType.ChaosRifleman;

                default:
                    {
                        return RoleType.Tutorial;
                    }
            }
        }

        /// Creates or updates <see cref="MapEditorObject"/>'s indicator (if it exists).
        /// </summary>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> to update.</param>
        public static void UpdateIndicator(this MapEditorObject mapObject)
        {
            IndicatorObjectComponent indicator = mapObject.AttachedIndicator;

            switch (mapObject)
            {
                case ItemSpawnPointComponent itemSpawnPoint:
                    {
                        Methods.SpawnObjectIndicator(itemSpawnPoint, indicator);
                        break;
                    }

                case PlayerSpawnPointComponent playerSpawnPoint:
                    {
                        Methods.SpawnObjectIndicator(playerSpawnPoint, indicator);
                        break;
                    }

                case RagdollSpawnPointComponent ragdollSpawnPoint:
                    {
                        Methods.SpawnObjectIndicator(ragdollSpawnPoint, indicator);
                        break;
                    }

                case LightSourceComponent lightSource:
                    {
                        Methods.SpawnObjectIndicator(lightSource, indicator);
                        break;
                    }

                case TeleportComponent teleport:
                    {
                        Methods.SpawnObjectIndicator(teleport, indicator);
                        break;
                    }
            }
        }

        /// <inheritdoc cref="Item.Spawn(Vector3, Quaternion)"/>
        public static Pickup Create(this Item item, Vector3 position, Quaternion rotation = default, Vector3? scale = null)
        {
            item.Base.PickupDropModel.Info.ItemId = item.Type;
            item.Base.PickupDropModel.Info.Position = position;
            item.Base.PickupDropModel.Info.Weight = item.Weight;
            item.Base.PickupDropModel.Info.Rotation = new LowPrecisionQuaternion(rotation);
            item.Base.PickupDropModel.NetworkInfo = item.Base.PickupDropModel.Info;

            InventorySystem.Items.Pickups.ItemPickupBase ipb = Object.Instantiate(item.Base.PickupDropModel, position, rotation);
            if (ipb is InventorySystem.Items.Firearms.FirearmPickup firearmPickup)
            {
                if (item is Firearm firearm)
                {
                    firearmPickup.Status = new InventorySystem.Items.Firearms.FirearmStatus(firearm.Ammo, InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }
                else
                {
                    byte ammo;
                    switch (item.Base)
                    {
                        case InventorySystem.Items.Firearms.AutomaticFirearm auto:
                            ammo = auto._baseMaxAmmo;
                            break;
                        case InventorySystem.Items.Firearms.Shotgun shotgun:
                            ammo = shotgun._ammoCapacity;
                            break;
                        case InventorySystem.Items.Firearms.Revolver _:
                            ammo = 6;
                            break;
                        default:
                            ammo = 0;
                            break;
                    }

                    firearmPickup.Status = new InventorySystem.Items.Firearms.FirearmStatus(ammo, InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }

                firearmPickup.NetworkStatus = firearmPickup.Status;
            }

            if (scale.HasValue)
                ipb.transform.localScale = scale.Value;

            ipb.InfoReceived(default, item.Base.PickupDropModel.NetworkInfo);

            return Pickup.Get(ipb);
        }

        /// <inheritdoc cref="Exiled.API.Extensions.ReflectionExtensions.CopyProperties(object, object)"/>
        public static T CopyProperties<T>(this T target, object source)
        {
            Exiled.API.Extensions.ReflectionExtensions.CopyProperties(target, source);
            return target;
        }
    }
}
