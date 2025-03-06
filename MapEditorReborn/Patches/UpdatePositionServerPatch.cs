// -----------------------------------------------------------------------
// <copyright file="UpdatePositionServerPatch.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Patches
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using AdminToys;
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(AdminToyBase), nameof(AdminToyBase.UpdatePositionServer))]
    internal static class UpdatePositionServerPatch
    {
        private static bool Prefix(AdminToyBase __instance)
        {
            __instance.NetworkPosition = __instance.transform.position;
            __instance.NetworkRotation = __instance.transform.rotation;
            __instance.NetworkScale = __instance.transform.root != __instance.transform ? Vector3.Scale(__instance.transform.localScale, __instance.transform.root.localScale) : __instance.transform.localScale;

            return false;
        }
    }
}