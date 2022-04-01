namespace MapEditorReborn.Events.Handlers.Internal
{
    using System.Collections.Generic;
    using API.Extensions;
    using API.Features.Objects;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;
    using InventorySystem.Items.Flashlight;
    using MEC;
    using UnityEngine;
    using Utils.Networking;

    using static API.API;

    internal static class GravityGunHandler
    {
        internal static void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (ev.NewItem == null || !GravityGuns.Contains(ev.NewItem.Serial))
                return;

            Timing.CallDelayed(0.01f, () =>
            {
                (ev.NewItem as Flashlight).Active = false;
                new FlashlightNetworkHandler.FlashlightMessage(ev.NewItem.Serial, false).SendToAuthenticated(0);
            });
        }

        internal static void OnTogglingFlashlight(TogglingFlashlightEventArgs ev)
        {
            if (!GravityGuns.Contains(ev.Flashlight.Serial))
                return;

            ev.IsAllowed = false;
            Timing.CallDelayed(0.25f, () =>
            {
                ev.Flashlight.Base.IsEmittingLight = false;
                new FlashlightNetworkHandler.FlashlightMessage(ev.Flashlight.Serial, false).SendToAuthenticated(0);
            });

            if (grabbingPlayers.Contains(ev.Player))
            {
                grabbingPlayers.Remove(ev.Player);
                return;
            }

            Vector3 forward = ev.Player.CameraTransform.forward;
            if (Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out RaycastHit hit, 5f))
            {
                if (hit.collider.transform.root.TryGetComponent(out SchematicObject schematicObject) && schematicObject != null && schematicObject.Base.IsPickable)
                {
                    if (!schematicObject.gameObject.TryGetComponent(out Rigidbody rigidbody))
                    {
                        rigidbody = schematicObject.gameObject.AddComponent<Rigidbody>();
                        rigidbody.mass = 1;
                    }

                    grabbingPlayers.Add(ev.Player);
                    Timing.RunCoroutine(GravityGunMovementCoroutine(ev.Player, rigidbody));
                }
            }
        }

        private static IEnumerator<float> GravityGunMovementCoroutine(Player player, Rigidbody rigidbody)
        {
            rigidbody.isKinematic = true;
            rigidbody.transform.eulerAngles = Vector3.zero;

            while (grabbingPlayers.Contains(player) && player.CurrentItem.IsGravityGun())
            {
                yield return Timing.WaitForOneFrame;
                rigidbody.MovePosition(player.CameraTransform.position + (player.CameraTransform.forward * 2f));
            }

            rigidbody.isKinematic = false;
            grabbingPlayers.Remove(player);
        }

        private static List<Player> grabbingPlayers = new List<Player>();
    }
}
