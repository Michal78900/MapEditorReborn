namespace MapEditorReborn.Patches
{
    using System.Collections.Generic;
    using HarmonyLib;
    using InventorySystem.Items.ThrowableProjectiles;
    using Mirror;
    using NorthwoodLib.Pools;
    using UnityEngine;

    // [HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.Explode))]
    internal static class ExplodeGrenadePatch
    {
        private static bool Prefix(ExplosionGrenade __instance, Footprinting.Footprint attacker, Vector3 position, ExplosionGrenade settingsReference)
        {
            HashSet<uint> hashSet = HashSetPool<uint>.Shared.Rent();
            HashSet<uint> hashSet2 = HashSetPool<uint>.Shared.Rent();
            float maxRadius = settingsReference._maxRadius;
            foreach (Collider collider in Physics.OverlapSphere(position, maxRadius, settingsReference._detectionMask))
            {
                if (NetworkServer.active)
                {
                    IExplosionTrigger explosionTrigger;
					if (collider.TryGetComponent(out explosionTrigger))
                    {
						explosionTrigger.OnExplosionDetected(attacker, position, maxRadius);
					}
					global::IDestructible destructible;
					Interactables.InteractableCollider interactableCollider;
					Interactables.Interobjects.DoorUtils.DoorVariant doorVariant;
					if (collider.TryGetComponent(out destructible))
					{
						if (!hashSet.Contains(destructible.NetworkId) && ExplosionGrenade.ExplodeDestructible(destructible, attacker, position, settingsReference))
						{
							hashSet.Add(destructible.NetworkId);
						}
					}
					else if (collider.TryGetComponent(out interactableCollider) && (doorVariant = interactableCollider.Target as Interactables.Interobjects.DoorUtils.DoorVariant) != null && hashSet2.Add(doorVariant.netId))
					{
						ExplosionGrenade.ExplodeDoor(doorVariant, position, settingsReference);
					}
				}

				Rigidbody rigidbody = collider.attachedRigidbody ?? collider.GetComponentInParent<Rigidbody>();
				if (rigidbody != null)
				{
					ExplosionGrenade.ExplodeRigidbody(rigidbody, position, maxRadius, settingsReference);
				}
			}
			HashSetPool<uint>.Shared.Return(hashSet);
			HashSetPool<uint>.Shared.Return(hashSet2);

            return false;
        }
    }
}
