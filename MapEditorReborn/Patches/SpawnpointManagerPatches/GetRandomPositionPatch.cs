namespace MapEditorReborn.Patches.SpawnpointManagerPatches
{
#pragma warning disable SA1313
    using API.Features.Components.ObjectComponents;
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(SpawnpointManager), nameof(SpawnpointManager.GetRandomPosition))]
    internal static class GetRandomPositionPatch
    {
        private static bool Prefix(RoleType roleType, ref GameObject __result)
        {
            switch (roleType)
            {
                case RoleType.Scp93989:
                    roleType = RoleType.Scp93953;
                    break;

                case RoleType.NtfSergeant:
                case RoleType.NtfSpecialist:
                case RoleType.NtfCaptain:
                    roleType = RoleType.NtfPrivate;
                    break;

                case RoleType.ChaosConscript:
                case RoleType.ChaosMarauder:
                case RoleType.ChaosRepressor:
                    roleType = RoleType.ChaosRifleman;
                    break;
            }

            if (!PlayerSpawnPointComponent.SpawnpointPositions.ContainsKey(roleType))
                return false;

            __result = PlayerSpawnPointComponent.SpawnpointPositions[roleType][Random.Range(0, PlayerSpawnPointComponent.SpawnpointPositions[roleType].Count)];
            return false;
        }
    }
}
