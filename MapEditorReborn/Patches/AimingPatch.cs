namespace MapEditorReborn.Patches
{
    using API;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Items.Firearms;
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
            Player player = Player.Get(__instance._firearm.Owner);

            if (!player.CurrentItem.IsToolGun() || player.SessionVariables.ContainsKey(Handler.SelectedObjectSessionVarName))
                return;

            if (value)
            {
                if (player.HasFlashlightModuleEnabled)
                {
                    player.ShowHint(Translation.ModeSelecting, 1f);
                }
                else
                {
                    player.ShowHint(Translation.ModeCopying, 1f);
                }
            }
            else
            {
                if (player.HasFlashlightModuleEnabled)
                {
                    player.ShowHint(Translation.ModeCreating, 1f);
                }
                else
                {
                    player.ShowHint(Translation.ModeDeleting, 1f);
                }
            }
        }

        private static readonly Translation Translation = MapEditorReborn.Singleton.Translation;
    }
}
