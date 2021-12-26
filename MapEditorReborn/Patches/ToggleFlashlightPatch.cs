namespace MapEditorReborn.Patches
{
    using API.Extensions;
    using API.Features.Components;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Items.Firearms;
    using static API.API;
#pragma warning disable SA1313

    /// <summary>
    /// Pathches the <see cref="Firearm.Status"/> for the interface use of the ToolGun.
    /// </summary>
    [HarmonyPatch(typeof(Firearm), nameof(Firearm.Status), MethodType.Setter)]
    internal static class ToggleFlashlightPatch
    {
        private static void Prefix(Firearm __instance, ref FirearmStatus value)
        {
            Player player = Player.Get(__instance?.Owner);

            if (player == null || __instance.Status.Flags == value.Flags || !player.CurrentItem.IsToolGun() || (player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null))
                return;

            player.ShowHint(ToolGunHandler.GetToolGunModeText(player, player.IsAimingDownWeapon, value.Flags.HasFlag(FirearmStatusFlags.FlashlightEnabled)), 1f);
        }
    }
}
