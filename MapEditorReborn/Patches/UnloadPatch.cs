namespace MapEditorReborn.Patches
{
    using API.Extensions;
    using Exiled.API.Features.Items;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
#pragma warning disable SA1313

    /// <summary>
    /// Patches <see cref="AutomaticAmmoManager.ServerTryReload()"/> to prevent user from unloading the ToolGun.
    /// </summary>
    [HarmonyPatch(typeof(AutomaticAmmoManager), nameof(AutomaticAmmoManager.ServerTryUnload))]
    internal static class UnloadPatch
    {
        private static bool Prefix(AutomaticAmmoManager __instance) => !Item.Get(__instance._firearm).IsToolGun();
    }
}
