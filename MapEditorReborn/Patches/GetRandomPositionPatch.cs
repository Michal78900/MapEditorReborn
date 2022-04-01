namespace MapEditorReborn.Patches
{
#pragma warning disable SA1313
    using API.Enums;
    using API.Features.Objects;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="SpawnpointManager.GetRandomPosition(RoleType)"/>.
    /// </summary>
    [HarmonyPatch(typeof(SpawnpointManager), nameof(SpawnpointManager.GetRandomPosition))]
    internal static class GetRandomPositionPatch
    {
        private static bool Prefix(RoleType roleType, ref GameObject __result)
        {
            SpawnableTeam spawnableTeam = SpawnableTeam.None;

            switch (roleType)
            {
                case RoleType.Scp049:
                    spawnableTeam = SpawnableTeam.Scp049;
                    break;

                case RoleType.Scp0492:
                    spawnableTeam = SpawnableTeam.Scp0492;
                    break;

                case RoleType.Scp079:
                    spawnableTeam = SpawnableTeam.Scp079;
                    break;

                case RoleType.Scp096:
                    spawnableTeam = SpawnableTeam.Scp096;
                    break;

                case RoleType.Scp106:
                    spawnableTeam = SpawnableTeam.Scp106;
                    break;

                case RoleType.Scp173:
                    spawnableTeam = SpawnableTeam.Scp173;
                    break;

                case RoleType.Scp93953:
                case RoleType.Scp93989:
                    spawnableTeam = SpawnableTeam.Scp939;
                    break;

                case RoleType.ClassD:
                    spawnableTeam = SpawnableTeam.ClassD;
                    break;

                case RoleType.Scientist:
                    spawnableTeam = SpawnableTeam.Scientist;
                    break;

                case RoleType.FacilityGuard:
                    spawnableTeam = SpawnableTeam.FacilityGuard;
                    break;

                case RoleType.NtfPrivate:
                case RoleType.NtfSergeant:
                case RoleType.NtfSpecialist:
                case RoleType.NtfCaptain:
                    spawnableTeam = SpawnableTeam.MTF;
                    break;

                case RoleType.ChaosRifleman:
                case RoleType.ChaosConscript:
                case RoleType.ChaosMarauder:
                case RoleType.ChaosRepressor:
                    spawnableTeam = SpawnableTeam.Chaos;
                    break;

                case RoleType.Tutorial:
                    spawnableTeam = SpawnableTeam.Tutorial;
                    break;
            }

            if (!PlayerSpawnPointObject.SpawnpointPositions.ContainsKey(spawnableTeam))
                return false;

            __result = PlayerSpawnPointObject.SpawnpointPositions[spawnableTeam][Random.Range(0, PlayerSpawnPointObject.SpawnpointPositions[spawnableTeam].Count)];
            return false;
        }
    }
}
