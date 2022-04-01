namespace MapEditorReborn.Patches.Dummy
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
    using Mirror;

    using NorthwoodLib.Pools;

    // Credits for these two transpilers go to Gamehunt

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
