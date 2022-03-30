// -----------------------------------------------------------------------
// <copyright file="ServerPerformShotPatch.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Patches.Shooting
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using HarmonyLib;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Modules;
    using UnityEngine;

    [HarmonyPatch(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.ServerPerformShot))]
    internal static class ServerPerformShotPatch
    {
        private static bool Prefix(SingleBulletHitreg __instance, Ray ray)
        {
            FirearmBaseStats baseStats = __instance.Firearm.BaseStats;
            Vector3 a = (new Vector3(Random.value, Random.value, Random.value) - (Vector3.one / 2f)).normalized * Random.value;
            float num = baseStats.GetInaccuracy(__instance.Firearm, __instance.Firearm.AdsModule.ServerAds, __instance.Hub.playerMovementSync.PlayerVelocity.magnitude, __instance.Hub.playerMovementSync.Grounded);
            if (__instance._usesRecoilPattern)
            {
                __instance._recoilPattern.ApplyShot(1f / __instance.Firearm.ActionModule.CyclicRate);
                num += __instance._recoilPattern.GetInaccuracy();
            }

            ray.direction = Quaternion.Euler(num * a) * ray.direction;
            if (Physics.Raycast(ray, out RaycastHit hit, baseStats.MaxDistance(), StandardHitregBase.HitregMask))
            {
                IDestructible destructible = hit.collider.GetComponentInParent<IDestructible>(); // GetComponentInParent instead TryGetComponent
                if (destructible != null)
                {
                    __instance.RestorePlayerPosition();
                    float damage = baseStats.DamageAtDistance(__instance.Firearm, hit.distance);
                    if (destructible.Damage(damage, new PlayerStatsSystem.FirearmDamageHandler(__instance.Firearm, damage, true), hit.point))
                    {
                        Hitmarker.SendHitmarker(__instance.Conn, 1f);
                        __instance.ShowHitIndicator(destructible.NetworkId, damage, ray.origin);
                        __instance.PlaceBloodDecal(ray, hit, destructible);
                        return false;
                    }
                }
                else
                {
                    __instance.PlaceBulletholeDecal(ray, hit);
                }
            }

            return false;
        }
    }
}
