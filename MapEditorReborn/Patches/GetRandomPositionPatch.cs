// // -----------------------------------------------------------------------
// // <copyright file="GetRandomPositionPatch.cs" company="MapEditorReborn">
// // Copyright (c) MapEditorReborn. All rights reserved.
// // Licensed under the CC BY-SA 3.0 license.
// // </copyright>
// // -----------------------------------------------------------------------
//
// namespace MapEditorReborn.Patches;
//
// #pragma warning disable SA1313
//
// using API.Enums;
// using API.Features.Objects;
// using HarmonyLib;
// using UnityEngine;
//
// /// <summary>
// /// Patches <see cref="SpawnpointManager.GetRandomPosition(RoleType)"/>.
// /// </summary>
// [HarmonyPatch(typeof(SpawnpointManager), nameof(SpawnpointManager.GetRandomPosition))]
// internal static class GetRandomPositionPatch
// {
//     private static bool Prefix(RoleType roleType, ref GameObject __result)
//     {
//         var spawnableTeam = roleType switch
//         {
//             RoleType.Scp049 => SpawnableTeam.Scp049,
//             RoleType.Scp0492 => SpawnableTeam.Scp0492,
//             RoleType.Scp079 => SpawnableTeam.Scp079,
//             RoleType.Scp096 => SpawnableTeam.Scp096,
//             RoleType.Scp106 => SpawnableTeam.Scp106,
//             RoleType.Scp173 => SpawnableTeam.Scp173,
//             RoleType.Scp93953 => SpawnableTeam.Scp939,
//             RoleType.Scp93989 => SpawnableTeam.Scp939,
//             RoleType.ClassD => SpawnableTeam.ClassD,
//             RoleType.Scientist => SpawnableTeam.Scientist,
//             RoleType.FacilityGuard => SpawnableTeam.FacilityGuard,
//             RoleType.NtfPrivate => SpawnableTeam.MTF,
//             RoleType.NtfSergeant => SpawnableTeam.MTF,
//             RoleType.NtfSpecialist => SpawnableTeam.MTF,
//             RoleType.NtfCaptain => SpawnableTeam.MTF,
//             RoleType.ChaosRifleman => SpawnableTeam.Chaos,
//             RoleType.ChaosConscript => SpawnableTeam.Chaos,
//             RoleType.ChaosMarauder => SpawnableTeam.Chaos,
//             RoleType.ChaosRepressor => SpawnableTeam.Chaos,
//             RoleType.Tutorial => SpawnableTeam.Tutorial,
//             _ => SpawnableTeam.None,
//         };
//
//         if (spawnableTeam == SpawnableTeam.None)
//         {
//             __result = null;
//             return false;
//         }
//
//         if (!PlayerSpawnPointObject.SpawnpointPositions.ContainsKey(spawnableTeam) || PlayerSpawnPointObject.SpawnpointPositions[spawnableTeam].Count == 0)
//         {
//             __result = null;
//             return false;
//         }
//
//         __result = PlayerSpawnPointObject.SpawnpointPositions[spawnableTeam][Random.Range(0, PlayerSpawnPointObject.SpawnpointPositions[spawnableTeam].Count)];
//         return false;
//     }
// }