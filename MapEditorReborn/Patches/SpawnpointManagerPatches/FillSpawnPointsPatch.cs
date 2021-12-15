namespace MapEditorReborn.Patches.SpawnpointManagerPatches
{
    using API;
    using Exiled.API.Features;
    using HarmonyLib;

    [HarmonyPatch(typeof(SpawnpointManager), nameof(SpawnpointManager.FillSpawnPoints))]
    internal static class FillSpawnPointsPatch
    {
        private static bool Prefix() => false;
    }
}
