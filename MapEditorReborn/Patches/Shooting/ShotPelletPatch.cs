namespace MapEditorReborn.Patches.Shooting
{
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
    using UnityEngine;

    [HarmonyPatch(typeof(BuckshotHitreg), nameof(BuckshotHitreg.ShootPellet))]
    internal static class ShotPelletPatch
    {
        private static bool Prefix(BuckshotHitreg __instance, Vector2 pelletSettings, Ray originalRay, Vector2 offsetVector)
        {
            Vector2 vector = Vector2.Lerp(pelletSettings, __instance.GenerateRandomPelletDirection, __instance.BuckshotRandomness) * __instance.BuckshotScale;
            Vector3 vector2 = originalRay.direction;
            vector2 = Quaternion.AngleAxis(vector.x + offsetVector.x, __instance.Hub.PlayerCameraReference.up) * vector2;
            vector2 = Quaternion.AngleAxis(vector.y + offsetVector.y, __instance.Hub.PlayerCameraReference.right) * vector2;
            Ray ray = new Ray(originalRay.origin, vector2);

            if (Physics.Raycast(ray, out RaycastHit hit, __instance.Firearm.BaseStats.MaxDistance(), StandardHitregBase.HitregMask))
            {
                IDestructible destructible = hit.collider.GetComponentInParent<IDestructible>();
                if (destructible != null)
                {
                    __instance.RestorePlayerPosition();
                    float damage = __instance.Firearm.BaseStats.DamageAtDistance(__instance.Firearm, hit.distance) / (float)__instance._buckshotSettings.MaxHits;
                    if (__instance.CanShoot(destructible.NetworkId) && destructible.Damage(damage, new PlayerStatsSystem.FirearmDamageHandler(__instance.Firearm, damage, false), hit.point))
                    {
                        __instance.ShowHitIndicator(destructible.NetworkId, damage, originalRay.origin);
                        __instance.PlaceBloodDecal(ray, hit, destructible);
                        return true;
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
