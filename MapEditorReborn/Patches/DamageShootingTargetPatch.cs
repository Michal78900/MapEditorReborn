namespace MapEditorReborn.Patches
{
    using AdminToys;
    using API;
    using HarmonyLib;
    using InventorySystem.Items;
    using UnityEngine;
#pragma warning disable SA1313

    /// <summary>
    /// Patches the <see cref="ShootingTarget.Damage(float, IDamageDealer, Footprinting.Footprint, Vector3)"/> to disable functions of selected Shootin Targets.
    /// </summary>
    [HarmonyPatch(typeof(ShootingTarget), nameof(ShootingTarget.Damage))]
    internal static class DamageShootingTargetPatch
    {
        private static bool Prefix(ShootingTarget __instance) => !(__instance.TryGetComponent(out ShootingTargetComponent shootingTargetComponent) && !shootingTargetComponent.Base.IsFunctional);
    }
}