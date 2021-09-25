namespace MapEditorReborn.Patches
{
    using API;
    using Exiled.API.Features.Items;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;

    [HarmonyPatch(typeof(AutomaticAmmoManager), nameof(AutomaticAmmoManager.ServerTryUnload))]
    internal static class UnloadPatch
    {
        private static bool Prefix(AutomaticAmmoManager __instance) => !Item.Get(__instance._firearm).IsToolGun();
    }
}
