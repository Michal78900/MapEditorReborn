namespace MapEditorReborn.Patches
{
    using API;
    using API.Extensions;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Items.Firearms;
#pragma warning disable SA1313

    /// <summary>
    /// Pathches the <see cref="Firearm.OnStatusChanged(FirearmStatus, FirearmStatus)"/> for the interface use of the ToolGun.
    /// </summary>
    [HarmonyPatch(typeof(Firearm), nameof(Firearm.Status), MethodType.Setter)]
    internal static class ToggleFlashlightPatch
    {
        private static void Prefix(Firearm __instance, ref FirearmStatus value)
        {
            Player player = Player.Get(__instance?.Owner);

            if (player == null || __instance.Status.Flags == value.Flags || !player.CurrentItem.IsToolGun() || (player.TryGetSessionVariable(Methods.SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null))
                return;

            player.ShowHint(Methods.GetToolGunModeText(player, player.IsAimingDownWeapon, value.Flags.HasFlag(FirearmStatusFlags.FlashlightEnabled)), 1f);
        }
    }
}
