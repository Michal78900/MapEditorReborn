namespace MapEditorReborn.API
{
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;
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
        public static bool IsToolGun(this Inventory.SyncItemInfo item)
        {
            return Handler.ToolGuns.ContainsKey(item.uniq);
        }

        /// <summary>
        /// Used for showing details about the <see cref="GameObject"/> to a specifc <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to which the message should be shown.</param>
        /// <param name="gameObject">The GameObject from which the details should be used.</param>
        public static void ShowGamgeObjectHint(this Player player, GameObject gameObject)
        {
            string message = string.Empty;

            message += $"<size=30>Selected object type: <color=yellow><b>{(gameObject.TryGetComponent(out DoorVariant door) ? door.GetDoorTypeByName().ToString() : gameObject.name.Replace("(Clone)", string.Empty))}</b></color></size>\n";
            message += $"<size=20>" +
                       $"Position {string.Format("X: <color=yellow><b>{0:F3}</b></color> Y: <color=yellow><b>{1:F3}</b></color> Z: <color=yellow><b>{2:F3}</b></color>", gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z)} | " +
                       $"Rotation {string.Format("X: <color=yellow><b>{0:F3}</b></color> Y: <color=yellow><b>{1:F3}</b></color> Z: <color=yellow><b>{2:F3}</b></color>", gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z)} | " +
                       $"Scale {string.Format("X: <color=yellow><b>{0:F3}</b></color> Y: <color=yellow><b>{1:F3}</b></color> Z: <color=yellow><b>{2:F3}</b></color>", gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z)}" +
                       $"</size>\n";

            switch (gameObject.name)
            {
                case "LCZ BreakableDoor(Clone)":
                case "HCZ BreakableDoor(Clone)":
                case "EZ BreakableDoor(Clone)":
                    {
                        BreakableDoor breakableDoor = door as BreakableDoor;

                        message += $"<size=20>" +
                                   $"IsOpened: {(door.NetworkTargetState ? "<color=green><b></b>TRUE</color>" : "<color=red><b></b>FALSE</color>")}\n" +
                                   $"IsLocked: {(door.NetworkActiveLocks == 64 ? "<color=green><b></b>TRUE</color>" : "<color=red><b></b>FALSE</color>")}\n" +
                                   $"KeycardPermissions: <color=yellow><b>{door.RequiredPermissions.RequiredPermissions} ({(ushort)door.RequiredPermissions.RequiredPermissions})</b></color>\n" +
                                   $"IgnoredDamageSources: <color=yellow><b>{breakableDoor._ignoredDamageSources} ({(byte)breakableDoor._ignoredDamageSources})</b></color>\n" +
                                   $"DoorHealth: <color=yellow><b>{breakableDoor._remainingHealth}</b></color>\n" +
                                   $"OpenOnWarheadActivation: {(door.GetComponent<DoorObjectComponent>().OpenOnWarheadActivation ? "<color=green><b></b>TRUE</color>" : "<color=red><b></b>FALSE</color>")}" +
                                   $"</size>";

                        break;
                    }

                case "Work Station(Clone)":
                    {
                        break;
                    }

                case "PlayerSpawnPointObject(Clone)":
                    {
                        RoleType type = gameObject.tag.ConvertToRoleType();
                        string name = string.Empty;

                        switch (type)
                        {
                            case RoleType.NtfCadet:
                                name = "MTF";
                                break;

                            case RoleType.Scp93953:
                                name = "SCP939";
                                break;

                            default:
                                name = type.ToString();
                                break;
                        }

                        message += $"<size=20>SpawnpointType: <color=yellow><b>{name}</b></color></size>";

                        break;
                    }

                case "ItemSpawnPointObject(Clone)":
                    {
                        ItemSpawnPointComponent itemSpawnPointComponent = gameObject.GetComponent<ItemSpawnPointComponent>();

                        message += $"<size=20>" +
                                   $"ItemType: <color=yellow><b>{itemSpawnPointComponent.ItemName}</b></color>\n" +
                                   $"SpawnChance: <color=yellow><b>{itemSpawnPointComponent.SpawnChance}</b></color>" +
                                   $"</size>";

                        break;
                    }
            }

            player.ShowHint(message, 9999f);
        }

        /// <summary>
        /// Gets the door's <see cref="GameObject"/> prefab, by it's <see cref="DoorType"/>.
        /// </summary>
        /// <param name="doorType">The DoorType of the door.</param>
        /// <returns>The <see cref="GameObject"/> prefab of a door.</returns>
        public static GameObject GetDoorObjectByType(this DoorType doorType)
        {
            switch (doorType)
            {
                case DoorType.LightContainmentDoor:
                    return Handler.LczDoorObj;

                case DoorType.HeavyContainmentDoor:
                    return Handler.HczDoorObj;

                case DoorType.EntranceDoor:
                    return Handler.EzDoorObj;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the object's <see cref="GameObject"/> prefab, by ToolGun's <see cref="ToolGunMode"/>.
        /// </summary>
        /// <param name="toolGunMode">The <see cref="ToolGunMode"/>.</param>
        /// <returns>The <see cref="GameObject"/> prefab of an object.</returns>
        public static GameObject GetObjectByMode(this ToolGunMode toolGunMode)
        {
            switch (toolGunMode)
            {
                case ToolGunMode.LczDoor:
                    return Handler.LczDoorObj;

                case ToolGunMode.HczDoor:
                    return Handler.HczDoorObj;

                case ToolGunMode.EzDoor:
                    return Handler.EzDoorObj;

                case ToolGunMode.WorkStation:
                    return Handler.WorkstationObj;

                case ToolGunMode.ItemSpawnPoint:
                    return Handler.ItemSpawnPointObj;

                case ToolGunMode.PlayerSpawnPoint:
                    return Handler.PlayerSpawnPointObj;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="DoorType"/> from the <see cref="DoorVariant"/> by it's name.
        /// </summary>
        /// <param name="doorVariant">The door to check."/>.</param>
        /// <returns><see cref="DoorType"/> of the door.</returns>
        public static DoorType GetDoorTypeByName(this DoorVariant doorVariant)
        {
            switch (doorVariant.name)
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
        /// Converts a <see cref="RoleType"/> to a nametag that it's spawnpoint uses.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleType"/> to convert.</param>
        /// <returns>A name of the spawnpoint nametag or the "TUT_SPAWN" when the <see cref="RoleType"/> is invalid.</returns>
        public static string ConvertToSpawnPointTag(this RoleType roleType)
        {
            switch (roleType)
            {
                case RoleType.Scp049:
                    return "SP_049";

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

                case RoleType.NtfCadet:
                case RoleType.NtfLieutenant:
                case RoleType.NtfScientist:
                case RoleType.NtfCommander:
                    return "SP_MTF";

                case RoleType.ChaosInsurgency:
                    return "SP_CI";

                case RoleType.Tutorial:
                    return "TUT Spawn";

                default:
                    {
                        Log.Error($"{roleType} is an invalid role!");
                        return "TUT Spawn";
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
                    return RoleType.NtfCadet;

                case "SP_CI":
                    return RoleType.ChaosInsurgency;

                case "TUT Spawn":
                    return RoleType.Tutorial;

                default:
                    {
                        Log.Error($"{spawnPointTag} is a invalid spawnpoint tag name!");
                        return RoleType.None;
                    }
            }
        }

        /// <summary>
        /// Checks if player has enabled flashlight mounted on a gun (ToolGun).
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns><see langword="true"/> if the flashlight is enabled, <see langword="false"/> if not.</returns>
        public static bool HasFlashlightEnabled(this Player player)
        {
            return player.ReferenceHub.weaponManager.syncFlash && player.ReferenceHub.weaponManager.weapons[player.ReferenceHub.weaponManager.curWeapon].mod_others.Any((item) => item.isActive && item.name == "Flashlight");
        }

        /// <summary>
        /// Updates GameObject's indicator (if it exists) and the player's hint (is the object is selected).
        /// </summary>
        /// <param name="gameObject">GameObject whose indicator should be updated.</param>
        /// <param name="player">Player sender that updated this GameObject.</param>
        public static void UpdateObject(this GameObject gameObject, Player player = null)
        {
            if (gameObject.name == "ItemSpawnPointObject(Clone)")
            {
                Handler.SpawnPickupIndicator(gameObject);
            }
            else if (gameObject.name == "PlayerSpawnPointObject(Clone)")
            {
                Handler.SpawnDummyIndicator(gameObject);
            }

            if (player != null)
            {
                if (player.SessionVariables.TryGetValue(Handler.SelectedObjectSessionVarName, out object selectedObject) && (GameObject)selectedObject == gameObject)
                {
                    player.ShowGamgeObjectHint(gameObject);
                }
            }
        }
    }
}
