namespace MapEditorReborn.Patches.SpawnpointManagerPatches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(SpawnpointManager), nameof(SpawnpointManager.FillSpawnPoints))]
    internal static class FillSpawnPointsPatch
    {
        private static bool Prefix() => false;
    }
}
