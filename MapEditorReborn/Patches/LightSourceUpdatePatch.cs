// -----------------------------------------------------------------------
// <copyright file="LightSourceUpdatePatch.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Patches
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using AdminToys;

    // [HarmonyPatch(typeof(LightSourceToy), nameof(LightSourceToy.LateUpdate))]
    internal static class LightSourceUpdatePatch
    {
        private static bool Prefix(LightSourceToy __instance)
        {
            if (__instance.transform.root.name.Contains("CustomSchematic"))
            {
                __instance.NetworkLightColor = __instance._light.color;
                __instance.NetworkLightIntensity = __instance._light.intensity;
                __instance.NetworkLightRange = __instance._light.range;
                return false;
            }

            return true;
        }
    }
}
