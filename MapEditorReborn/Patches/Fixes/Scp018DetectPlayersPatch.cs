// -----------------------------------------------------------------------
// <copyright file="Scp018DetectPlayersPatch.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Patches.Fixes
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using API.Features.Components;
    using HarmonyLib;
    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;

    [HarmonyPatch(typeof(Scp018Projectile), nameof(Scp018Projectile.DetectPlayers))]
    internal static class Scp018DetectPlayersPatch
    {
        private static bool Prefix(Scp018Projectile __instance)
        {
            Vector3 prevPosition = __instance._prevPosition;
            __instance._prevPosition = __instance.Rb.position;

            if (!Physics.Linecast(prevPosition, __instance.Rb.position, out RaycastHit raycastHit, 13))
                return false;

            if (CullingComponent.CullingColliders.Contains(raycastHit.collider))
                return false;

            if (!ReferenceHub.TryGetHub(raycastHit.transform.root.gameObject, out ReferenceHub referenceHub))
                return false;

            float num = __instance.CurrentDamage * Random.Range(0.9f, 1.1f) * (referenceHub.characterClassManager.IsHuman() ? 75 : 800);
            if (num < 10f)
                return false;

            __instance.Rb.AddForce(Vector3.left + Vector3.forward, ForceMode.Force);
            referenceHub.playerStats.DealDamage(new PlayerStatsSystem.Scp018DamageHandler(__instance, num, __instance.IgnoreFriendlyFire));
            __instance._cooldownTimer = __instance._damageCooldown;

            return false;
        }
    }
}
