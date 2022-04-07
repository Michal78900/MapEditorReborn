// -----------------------------------------------------------------------
// <copyright file="SetClassIDAdvPatch.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Patches
{
#pragma warning disable SA1313
    using HarmonyLib;
    using MEC;

    using static CharacterClassManager;

    /// <summary>
    /// Patches the <see cref="CharacterClassManager.SetClassID"/> to prevent people from falling when the custom map is not fully loaded.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetClassID))]
    internal static class SetClassIDAdvPatch
    {
        private static bool Prefix(CharacterClassManager __instance, RoleType id, SpawnReason spawnReason)
        {
            if (spawnReason is SpawnReason.RoundStart)
            {
                Timing.CallDelayed(0.1f, () => __instance.SetClassIDAdv(id, false, spawnReason, false));
                return false;
            }

            return true;
        }
    }
}
