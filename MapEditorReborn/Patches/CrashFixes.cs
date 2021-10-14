namespace MapEditorReborn.Patches
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using HarmonyLib;

    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Modules;
    using Mirror;

    using NorthwoodLib.Pools;

    // These 2 transpilers were made by Gamehunt

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

    /// <summary>
    /// Patches <see cref="StandardHitregBase.ShowHitIndicator"/> to fix crash when shooting near dummy.
    /// </summary>
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.ShowHitIndicator))]
    internal static class DummyShowHitIndicatorFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;

            int index = newInstructions.FindIndex(inst => inst.opcode == OpCodes.Ldloc_0) + offset;

            Label okLabel = generator.DefineLabel();

            // if(referenceHub.networkIdentity.connectionToClient == null)
            // {
            //   return;
            // }
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ReferenceHub), nameof(ReferenceHub.networkIdentity))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.connectionToClient))),
                new CodeInstruction(OpCodes.Brtrue_S, okLabel),
                new CodeInstruction(OpCodes.Ret),
                new CodeInstruction(OpCodes.Ldloc_0).WithLabels(okLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
