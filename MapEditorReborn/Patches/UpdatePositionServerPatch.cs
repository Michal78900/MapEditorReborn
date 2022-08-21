// -----------------------------------------------------------------------
// <copyright file="UpdatePositionServerPatch.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Patches
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using System.Collections.Generic;
    using System.Reflection.Emit;
    using AdminToys;
    using HarmonyLib;

    [HarmonyPatch(typeof(AdminToyBase), nameof(AdminToyBase.UpdatePositionServer))]
    internal static class UpdatePositionServerPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> _)
        {
            yield return new(OpCodes.Ret);
        }
    }
}