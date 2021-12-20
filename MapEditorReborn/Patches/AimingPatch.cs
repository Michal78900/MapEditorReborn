namespace MapEditorReborn.Patches
{
    using API;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
#pragma warning disable SA1313

    /// <summary>
    /// Pathches the <see cref="StandardAds.ServerAds"/> for the interface use of the ToolGun.
    /// </summary>
    [HarmonyPatch(typeof(StandardAds), nameof(StandardAds.ServerAds), MethodType.Setter)]
    internal static class AimingPatch
    {
        private static void Postfix(StandardAds __instance, ref bool value)
        {
            Player player = Player.Get(__instance.Firearm.Owner);

            if (!player.CurrentItem.IsToolGun() || (player.TryGetSessionVariable(Methods.SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null))
                return;

            player.ShowHint(Methods.GetToolGunModeText(player, value, player.HasFlashlightModuleEnabled), 1f);
        }
    }
}
