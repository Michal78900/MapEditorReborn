namespace MapEditorReborn.Patches.Fixes
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(CheckpointKiller), nameof(CheckpointKiller.OnTriggerEnter))]
    internal static class CheckpointKillerPatch
    {
        private static bool Prefix(CheckpointKiller __instance, Collider other) =>
            !API.Features.Components.CullingComponents.CullingComponent.CullingColliders.Contains(other);
    }
}
