// -----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Extensions
{
    using Enums;
    using Events.EventArgs;
    using Exiled.API.Features;
    using Features;
    using Features.Objects;
    using UnityEngine;
    using static API;

    /// <summary>
    /// A set of useful extensions to easily interact with <see cref="GameObject"/> and base <see cref="MapEditorObject"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the object's <see cref="GameObject"/> prefab given a specified <see cref="ObjectType"/>.
        /// </summary>
        /// <param name="toolGunMode">The <see cref="ObjectType"/>.</param>
        /// <returns>The corresponding <see cref="GameObject"/> prefab.</returns>
        public static GameObject GetObjectByMode(this ObjectType toolGunMode) => ObjectPrefabs.TryGetValue(toolGunMode, out GameObject obj) ? obj : null;

        /// <summary>
        /// Shows details about the specified <see cref="GameObject"/> to a specifc <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to which the message should be shown.</param>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> which details are gonna be shown.</param>
        public static void ShowGameObjectHint(this Player player, MapEditorObject mapObject)
        {
            string message = "<size=30>Selected object type: <color=yellow><b>{objectType}</b></color></size>\n";

            if (mapObject is not RoomLightObject)
            {
                Vector3 relativePosition = mapObject.RelativePosition;
                Vector3 relativeRotation = mapObject.RelativeRotation;

                message += "<size=20>" +
                           $"Position X: <color=yellow><b>{relativePosition.x:F3}</b></color> Y: <color=yellow><b>{relativePosition.y:F3}</b></color> Z: <color=yellow><b>{relativePosition.z:F3}</b></color> | " +
                           $"Rotation X: <color=yellow><b>{relativeRotation.x:F3}</b></color> Y: <color=yellow><b>{relativeRotation.y:F3}</b></color> Z: <color=yellow><b>{relativeRotation.z:F3}</b></color> | " +
                           $"Scale X: <color=yellow><b>{mapObject.Scale.x:F3}</b></color> Y: <color=yellow><b>{mapObject.Scale.y:F3}</b></color> Z: <color=yellow><b>{mapObject.Scale.z:F3}</b></color>\n" +
                           $"RoomType: <color=yellow><b>{mapObject.RoomType}</b></color></size>" +
                           "</size>\n";
            }

            switch (mapObject)
            {
                case DoorObject door:
                    {
                        message = message.Replace("{objectType}", door.Base.DoorType.ToString());

                        message += "<size=20>" +
                                   $"IsOpened: {(door.Base.IsOpen ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}\n" +
                                   $"IsLocked: {(door.Base.IsLocked ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}\n" +
                                   $"KeycardPermissions: <color=yellow><b>{door.Base.KeycardPermissions} ({(ushort)door.Base.KeycardPermissions})</b></color>\n" +
                                   $"IgnoredDamageSources: <color=yellow><b>{door.Base.IgnoredDamageSources} ({(byte)door.Base.IgnoredDamageSources})</b></color>\n" +
                                   $"DoorHealth: <color=yellow><b>{door.Base.DoorHealth}</b></color>\n" +
                                   // $"OpenOnWarheadActivation: {(door.Base.OpenOnWarheadActivation ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}" +
                                   "</size>";

                        break;
                    }

                case WorkstationObject workstation:
                    {
                        message = message.Replace("{objectType}", "Workstation");

                        message += "<size=20>" +
                                   $"IsInteractable: {(workstation.Base.IsInteractable ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}</b></color>" +
                                   "</size>";
                        break;
                    }

                case PlayerSpawnPointObject playerSpawnPoint:
                    {
                        message = message.Replace("{objectType}", "PlayerSpawnPoint");

                        message += $"<size=20>SpawnpointType: <color=yellow><b>{playerSpawnPoint.Base.SpawnableTeam}</b></color></size>";

                        break;
                    }

                case ItemSpawnPointObject itemSpawnPoint:
                    {
                        message = message.Replace("{objectType}", "ItemSpawnPoint");

                        message += "<size=20>" +
                                   $"Item: <color=yellow><b>{itemSpawnPoint.Base.Item}</b></color>\n" +
                                   $"AttachmentsCode: <color=yellow><b>{itemSpawnPoint.Base.AttachmentsCode}</b></color>\n" +
                                   $"SpawnChance: <color=yellow><b>{itemSpawnPoint.Base.SpawnChance}</b></color>\n" +
                                   $"NumberOfItems: <color=yellow><b>{itemSpawnPoint.Base.NumberOfItems}</b></color>\n" +
                                   $"UseGravity: {(itemSpawnPoint.Base.UseGravity ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}\n" +
                                   $"CanBePickedUp: {(itemSpawnPoint.Base.CanBePickedUp ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}" +
                                   "</size>";

                        break;
                    }

                case RagdollSpawnPointObject ragdollSpawnPoint:
                    {
                        message = message.Replace("{objectType}", "RagdollSpawnPoint");

                        message += "<size=20>" +
                                   $"Name: <color=yellow><b>{ragdollSpawnPoint.Base.Name}</b></color>\n" +
                                   $"RoleType: <color=yellow><b>{ragdollSpawnPoint.Base.RoleType}</b></color>\n" +
                                   $"DeathReason: <color=yellow><b>{ragdollSpawnPoint.Base.DeathReason}</b></color>\n" +
                                   $"SpawnChance: <color=yellow><b>{ragdollSpawnPoint.Base.SpawnChance}</b></color>" +
                                   "</size>";

                        break;
                    }

                case ShootingTargetObject shootingTarget:
                    {
                        message = message.Replace("{objectType}", shootingTarget.Base.TargetType + "ShootingTarget");

                        message += "<size=20>" +
                                   $"Type: {shootingTarget.Base.TargetType}</b></color>\n" +
                                   $"IsFunctional: {(shootingTarget.Base.IsFunctional ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}" +
                                   "</size>";

                        break;
                    }

                case PrimitiveObject primitive:
                    {
                        message = message.Replace("{objectType}", $"Primitive{primitive.Base.PrimitiveType}");

                        message += "<size=20>" +
                                   $"Color: <color=#{ColorUtility.ToHtmlStringRGBA(MapEditorObject.GetColorFromString(primitive.Base.Color))}><b>{primitive.Base.Color}</b></color>\n" +
                                   $"PrimitiveFlags: <color=yellow><b>{primitive.Base.PrimitiveFlags}</b></color>\n" +
                                   "</size>";

                        break;
                    }

                case LightSourceObject lightSource:
                    {
                        message = message.Replace("{objectType}", "LightSource");

                        break;
                    }

                case RoomLightObject roomLights:
                    {
                        message = message.Replace("{objectType}", "RoomLight");

                        message += "<size=20>" +
                                   $"RoomType: <color=yellow><b>{mapObject.ForcedRoomType}</b></color>\n" +
                                   $"Color: <color=#{ColorUtility.ToHtmlStringRGBA(MapEditorObject.GetColorFromString(roomLights.Base.Color))}><b>{roomLights.Base.Color}</b></color>\n" +
                                   $"ShiftSpeed: <color=yellow><b>{roomLights.Base.ShiftSpeed}</b></color>\n" +
                                   $"OnlyWarheadLight: {(roomLights.Base.OnlyWarheadLight ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}" +
                                   "</size>";

                        break;
                    }

                case TeleportObject teleport:
                    {
                        message = message.Replace("{objectType}", $"Teleporter-{teleport.Base.ObjectId}");

                        /*
                        message += $"<size=20>" +
                                   $"",
                                   $"</size>";
                        */

                        break;
                    }

                case LockerObject locker:
                    {
                        message = message.Replace("{objectType}", $"Locker{locker.Base.LockerType}");

                        break;
                    }

                case SchematicObject schematic:
                    {
                        message = message.Replace("{objectType}", schematic.name);

                        break;
                    }
            }

            ShowingObjectHintEventArgs ev = new(player, mapObject, message);
            Events.Handlers.MapEditorObject.OnShowingObjectHint(ev);

            if (!ev.IsAllowed)
                return;

            message = ev.Hint;

            player.ShowHint(message, 9999f);
        }

        /// <summary>
        /// Creates or updates <see cref="MapEditorObject"/>'s indicator, if any.
        /// </summary>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> to update.</param>
        public static void UpdateIndicator(this MapEditorObject mapObject)
        {
            IndicatorObject indicator = mapObject.AttachedIndicator;

            switch (mapObject)
            {
                case ItemSpawnPointObject itemSpawnPoint:
                    {
                        Indicator.SpawnObjectIndicator(itemSpawnPoint, indicator);
                        break;
                    }

                case PlayerSpawnPointObject playerSpawnPoint:
                    {
                        Indicator.SpawnObjectIndicator(playerSpawnPoint, indicator);
                        break;
                    }

                case RagdollSpawnPointObject ragdollSpawnPoint:
                    {
                        Indicator.SpawnObjectIndicator(ragdollSpawnPoint, indicator);
                        break;
                    }

                case LightSourceObject lightSource:
                    {
                        Indicator.SpawnObjectIndicator(lightSource, indicator);
                        break;
                    }

                case TeleportObject teleport:
                    {
                        Indicator.SpawnObjectIndicator(teleport, indicator);
                        break;
                    }

                case SchematicObject schematic:
                    {
                        foreach (GameObject block in schematic.AttachedBlocks)
                        {
                            if (block.TryGetComponent(out TeleportObject teleport))
                                teleport.UpdateIndicator();
                        }

                        break;
                    }
            }
        }
    }
}
