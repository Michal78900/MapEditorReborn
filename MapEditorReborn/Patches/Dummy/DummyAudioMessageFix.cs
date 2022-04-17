﻿// -----------------------------------------------------------------------
// <copyright file="DummyAudioMessageFix.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Patches.Dummy
{
#pragma warning disable SA1118
#pragma warning disable SA1402

    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using HarmonyLib;

    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using Mirror;

    using NorthwoodLib.Pools;

    // Credits for these two transpilers go to Gamehunt

    /// <summary>
    /// Patches <see cref="FirearmExtensions.ServerSendAudioMessage"/> to fix crash when shooting near dummy.
    /// </summary>
    [HarmonyPatch(typeof(FirearmExtensions), nameof(FirearmExtensions.ServerSendAudioMessage))]
    internal static class DummyAudioMessageFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 3;
            const int continueOffset = 2;

            int baseIndex = newInstructions.FindLastIndex(inst => inst.opcode == OpCodes.Callvirt && ((MethodInfo)inst.operand) == AccessTools.PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner)));

            Label continueLabel = (Label)newInstructions[baseIndex + continueOffset].operand;

            // if(referenceHub.networkIdentity.connectrionToClient == null)
            // {
            //   continue;
            // }
            newInstructions.InsertRange(baseIndex + offset, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, 5),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ReferenceHub), nameof(ReferenceHub.networkIdentity))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.connectionToClient))),
                new CodeInstruction(OpCodes.Brfalse_S, continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}