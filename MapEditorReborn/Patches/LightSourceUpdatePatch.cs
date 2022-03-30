// -----------------------------------------------------------------------
// <copyright file="LightSourceUpdatePatch.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Patches
{
    using AdminToys;
    using HarmonyLib;

    [HarmonyPatch(typeof(LightSourceToy), nameof(LightSourceToy.Update))]
    internal static class LightSourceUpdatePatch
    {
        private static bool Prefix(LightSourceToy __instance)
        {
            if (__instance.transform.root.name.Contains("CustomSchematic"))
            {
                __instance.NetworkLightColor = __instance._light.color;
                __instance.NetworkLightIntensity = __instance._light.intensity;
                __instance.NetworkLightRange = __instance._light.range;
                __instance.NetworkLightShadows = __instance._light.shadows != UnityEngine.LightShadows.None;
                return false;
            }

            return true;
        }
    }
}
