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

    /// <summary>
    /// Patches the <see cref="CharacterClassManager.SetClassIDAdv(RoleType, bool, CharacterClassManager.SpawnReason, bool)"/> to prevent people from falling when the custom map is not fully loaded.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetClassIDAdv))]
    internal static class SetClassIDAdvPatch
    {
        private static bool Prefix(CharacterClassManager __instance, RoleType id, bool lite, CharacterClassManager.SpawnReason spawnReason, bool isHook = false)
        {
            if (spawnReason is CharacterClassManager.SpawnReason.RoundStart)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    __instance.SetClassIDAdv(id, lite, CharacterClassManager.SpawnReason.Respawn, isHook);
                });

                return false;
            }

            return true;
        }
    }
}
