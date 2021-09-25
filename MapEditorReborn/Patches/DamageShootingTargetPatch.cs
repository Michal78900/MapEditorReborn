namespace MapEditorReborn.Patches
{
    using API;
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms.Utilities;
    using UnityEngine;

    [HarmonyPatch(typeof(ShootingTarget), nameof(ShootingTarget.Damage))]
    internal static class DamageShootingTargetPatch
    {
        private static bool Prefix(ShootingTarget __instance, float damage, IDamageDealer item, Footprinting.Footprint attackerFootprint, Vector3 exactHit) => !(__instance.TryGetComponent(out ShootingTargetComponent shootingTargetComponent) && !shootingTargetComponent.Base.IsFunctional);
    }
}