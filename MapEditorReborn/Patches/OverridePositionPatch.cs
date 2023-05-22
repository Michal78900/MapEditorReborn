// // -----------------------------------------------------------------------
// // <copyright file="OverridePositionPatch.cs" company="MapEditorReborn">
// // Copyright (c) MapEditorReborn. All rights reserved.
// // Licensed under the CC BY-SA 3.0 license.
// // </copyright>
// // -----------------------------------------------------------------------
//
// using MapEditorReborn.API.Extensions;
// using MapEditorReborn.Exiled.Enums;
// using PlayerRoles.FirstPersonControl;
// using PluginAPI.Core;
//
// namespace MapEditorReborn.Patches;
//
// #pragma warning disable SA1313 // Parameter names should begin with lower-case letter
//
// using System.Collections.Generic;
// using API.Enums;
// using API.Features.Objects;
// using Configs;
// using HarmonyLib;
// using MEC;
// using UnityEngine;
//
// using static API.API;
//
// [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition))]
// internal static class OverridePositionPatch
// {
//     internal static void ResetValues()
//     {
//         SurfaceSchematics.Clear();
//         FacilitySchematics.Clear();
//     }
//
//     private static void Prefix(PlayerMovementSync __instance, Vector3 pos)
//     {
//         Player player = Player.Get(__instance.gameObject);
//
//         if (player.TryGetSessionVariable("MapEditorReborn_LastYPos", out float lastY))
//         {
//             if ((lastY > 900f && pos.y < 900f) || (lastY < 900f && pos.y > 900f))
//             {
//                 player.SessionVariables["MapEditorReborn_LastYPos"] = pos.y;
//             }
//             else
//             {
//                 return;
//             }
//         }
//         else
//         {
//             player.SessionVariables.Add("MapEditorReborn_LastYPos", pos.y);
//         }
//
//         if (SurfaceSchematics.Count == 0 || FacilitySchematics.Count == 0)
//         {
//             foreach (var mapEditorObject in SpawnedObjects)
//             {
//                 if (mapEditorObject is SchematicObject schematic && schematic.Base.CullingType == CullingType.Zone)
//                 {
//                     if (mapEditorObject.RoomType == RoomType.Surface)
//                     {
//                         SurfaceSchematics.Add(schematic);
//                     }
//                     else
//                     {
//                         FacilitySchematics.Add(schematic);
//                     }
//                 }
//             }
//         }
//
//         if (SurfaceSchematics.Count == 0 || FacilitySchematics.Count == 0)
//             return;
//
//         if (pos.y > 900f)
//         {
//             foreach (var schematic in FacilitySchematics)
//             {
//                 player.DestroySchematic(schematic);
//             }
//
//             foreach (var schematic in SurfaceSchematics)
//             {
//                 uint i = 0;
//
//                 foreach (var networkIdentity in schematic.NetworkIdentities)
//                 {
//                     if (Config.SchematicBlockSpawnDelay >= 0)
//                     {
//                         Timing.CallDelayed(Config.SchematicBlockSpawnDelay * i, () => player.SpawnNetworkIdentity(networkIdentity));
//                         i++;
//                     }
//                     else
//                     {
//                         player.SpawnNetworkIdentity(networkIdentity);
//                     }
//                 }
//             }
//         }
//         else
//         {
//             foreach (var schematic in SurfaceSchematics)
//             {
//                 player.DestroySchematic(schematic);
//             }
//
//             foreach (var schematic in FacilitySchematics)
//             {
//                 uint i = 0;
//
//                 foreach (var networkIdentity in schematic.NetworkIdentities)
//                 {
//                     if (Config.SchematicBlockSpawnDelay >= 0)
//                     {
//                         Timing.CallDelayed(Config.SchematicBlockSpawnDelay * i, () => player.SpawnNetworkIdentity(networkIdentity));
//                         i++;
//                     }
//                     else
//                     {
//                         player.SpawnNetworkIdentity(networkIdentity);
//                     }
//                 }
//             }
//         }
//     }
//
//     private static readonly List<SchematicObject> SurfaceSchematics = new();
//     private static readonly List<SchematicObject> FacilitySchematics = new();
//     private static readonly Config Config = MapEditorReborn.Singleton.Config;
// }