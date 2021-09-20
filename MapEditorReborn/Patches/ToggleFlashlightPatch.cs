namespace MapEditorReborn.Patches
{
    using API;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.BasicMessages;
#pragma warning disable SA1313

    /// <summary>
    /// Pathches the <see cref="Firearm.OnStatusChanged(FirearmStatus, FirearmStatus)"/> for the interface use of the ToolGun.
    /// </summary>
    [HarmonyPatch(typeof(Firearm), nameof(Firearm.OnStatusChanged))]
    internal static class ToggleFlashlightPatch
    {
        private static void Postfix(Firearm __instance, FirearmStatus prevValue, FirearmStatus newValue)
        {
            Player player = Player.Get(__instance?.Owner);

            if (player == null)
                return;

            if (!player.CurrentItem.IsToolGun() || player.SessionVariables.ContainsKey(Handler.SelectedObjectSessionVarName))
                return;

            if (prevValue.Flags == newValue.Flags)
                return;

            if (newValue.Flags.HasFlagFast(FirearmStatusFlags.FlashlightEnabled))
            {
                if (player.IsAimingDownWeapon)
                {
                    player.ShowHint(Translation.ModeSelecting, 1f);
                }
                else
                {
                    player.ShowHint(Translation.ModeCreating, 1f);
                }
            }
            else
            {
                if (player.IsAimingDownWeapon)
                {
                    player.ShowHint(Translation.ModeCopying, 1f);
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
