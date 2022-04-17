// -----------------------------------------------------------------------
// <copyright file="CheckpointKillerPatch.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Patches.Fixes
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using HarmonyLib;
    using UnityEngine;

    // [HarmonyPatch(typeof(CheckpointKiller), nameof(CheckpointKiller.OnTriggerEnter))]
    internal static class CheckpointKillerPatch
    {
        private static bool Prefix(CheckpointKiller __instance, Collider other) =>
            !API.Features.Components.CullingComponent.CullingColliders.Contains(other);
    }
}
