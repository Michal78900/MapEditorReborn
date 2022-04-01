namespace MapEditorReborn.Patches.Fixes
{
    using System.Collections.Generic;
    using API.Features.Components;
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using NorthwoodLib.Pools;
    using Scp914;
    using UnityEngine;

    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.Upgrade))]
    internal static class Scp914UpgradePatch
    {
        private static bool Prefix(Collider[] intake, Vector3 moveVector, Scp914Mode mode, Scp914KnobSetting setting)
        {
            HashSet<GameObject> hashSet = HashSetPool<GameObject>.Shared.Rent();
            bool upgradeDropped = (mode & Scp914Mode.Dropped) == Scp914Mode.Dropped;
            bool flag = (mode & Scp914Mode.Inventory) == Scp914Mode.Inventory;
            bool heldOnly = flag && (mode & Scp914Mode.Held) == Scp914Mode.Held;
            for (int i = 0; i < intake.Length; i++)
            {
                if (CullingComponent.CullingColliders.Contains(intake[i]))
                    continue;

                GameObject gameObject = intake[i].transform.root.gameObject;
                if (hashSet.Add(gameObject))
                {
                    if (ReferenceHub.TryGetHub(gameObject, out ReferenceHub ply))
                    {
                        Scp914Upgrader.ProcessPlayer(ply, flag, heldOnly, moveVector, setting);
                    }
                    else if (gameObject.TryGetComponent(out ItemPickupBase pickup))
                    {
                        Scp914Upgrader.ProcessPickup(pickup, upgradeDropped, moveVector, setting);
                    }
                }
            }

            HashSetPool<GameObject>.Shared.Return(hashSet);

            return false;
        }
    }
}
