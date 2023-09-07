// -----------------------------------------------------------------------
// <copyright file="GravityGunHandler.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.Handlers.Internal
{
    using System.Collections.Generic;
    using API.Enums;
    using API.Extensions;
    using API.Features.Objects;
    using Configs;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using UnityEngine;
    using static API.API;

    internal static class GravityGunHandler
    {
        internal static void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (ev.Item == null || !GravityGuns.ContainsKey(ev.Item.Serial))
                return;
        }

        internal static void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (ev.Firearm == null || !GravityGuns.ContainsKey(ev.Firearm.Serial))
                return;

            ev.IsAllowed = false;
            GravityGunMode mode = GravityGuns[ev.Player.CurrentItem.Serial];
            string translation = "";
            if (mode.HasFlag(GravityGunMode.Gravity))
            {
                mode = mode.SetFlag(GravityGunMode.Gravity, false);
                translation = new GravityGunTranslations().ModeNoGravity;
            }
            else
            {
                mode = mode.SetFlag(GravityGunMode.Gravity, true);
                translation = new GravityGunTranslations().ModeGravity;
            }

            GravityGuns[ev.Player.CurrentItem.Serial] = mode;

            ev.Player.ClearBroadcasts();
            ev.Player.Broadcast(1, $"{translation}");
        }

        internal static void OnShootingGun(DryfiringWeaponEventArgs ev)
        {
            if (!GravityGuns.ContainsKey(ev.Player.CurrentItem.Serial))
                return;

            ev.IsAllowed = false;
            ((Firearm)ev.Player.CurrentItem).Ammo = 0;

            GravityGunMode mode = GravityGuns[ev.Player.CurrentItem.Serial];

            if (grabbingPlayers.Contains(ev.Player))
            {
                grabbingPlayers.Remove(ev.Player);
                return;
            }

            Vector3 forward = ev.Player.CameraTransform.forward;
            if (Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out RaycastHit hit, 5f))
            {
                if (hit.collider.transform.root.TryGetComponent(out SchematicObject schematicObject) && schematicObject != null /*&& schematicObject.Base.IsPickable*/)
                {
                    if (!schematicObject.gameObject.TryGetComponent(out Rigidbody rigidbody))
                    {
                        rigidbody = schematicObject.gameObject.AddComponent<Rigidbody>();
                        rigidbody.mass = 1;
                    }

                    grabbingPlayers.Add(ev.Player);
                    Timing.RunCoroutine(GravityGunMovementCoroutine(ev.Player, rigidbody, mode));
                }
            }
        }

        internal static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!ev.Item.IsGravityGun() || !ev.IsThrown)
                return;

            ev.IsAllowed = false;
            GravityGunMode mode = GravityGuns[ev.Player.CurrentItem.Serial];
            string translation = "";
            // if else because flags make things really weird with switch.
            if (mode.HasFlag(GravityGunMode.Movement) && mode.HasFlag(GravityGunMode.Rotate))
            {
                mode = mode.SetFlag(GravityGunMode.Rotate, false);
                translation = new GravityGunTranslations().ModeMoveOnly;
            }
            else if (mode.HasFlag(GravityGunMode.Movement) && !mode.HasFlag(GravityGunMode.Rotate))
            {
                mode = mode.SetFlag(GravityGunMode.Movement, false);
                mode = mode.SetFlag(GravityGunMode.Rotate, true);
                translation = new GravityGunTranslations().ModeRotateOnly;
            }
            else if (!mode.HasFlag(GravityGunMode.Movement) && mode.HasFlag(GravityGunMode.Rotate))
            {
                mode = mode.SetFlag(GravityGunMode.Rotate, true);
                mode = mode.SetFlag(GravityGunMode.Movement, true);
                translation = new GravityGunTranslations().ModeMoveAndRotate;
            }

            GravityGuns[ev.Player.CurrentItem.Serial] = mode;
            ev.Player.ClearBroadcasts();
            ev.Player.Broadcast(1, $"{translation}");
        }

        private static IEnumerator<float> GravityGunMovementCoroutine(Player player, Rigidbody rigidbody, GravityGunMode mode)
        {
            rigidbody.isKinematic = true;

            if (!mode.HasFlag(GravityGunMode.Gravity))
            {
                rigidbody.mass = 0;
                rigidbody.useGravity = false;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            }
            else
            {
                rigidbody.mass = 1;
                rigidbody.useGravity = true;
                rigidbody.constraints = RigidbodyConstraints.None;
            }


            if (!mode.HasFlag(GravityGunMode.Movement))
                rigidbody.constraints = RigidbodyConstraints.FreezePosition;
            else if (!mode.HasFlag(GravityGunMode.Rotate))
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;


            Log.Debug($"GG Mode: {mode}");
            //else
            //rigidbody.transform.eulerAngles = Vector3.zero;
            bool move = mode.HasFlag(GravityGunMode.Movement);
            bool rotate = mode.HasFlag(GravityGunMode.Rotate);
            while (grabbingPlayers.Contains(player) && player.CurrentItem.IsGravityGun())
            {
                yield return Timing.WaitForOneFrame;

                if (move)
                    rigidbody.MovePosition(player.CameraTransform.position + (player.CameraTransform.forward * 2f));

                if (rotate)
                    rigidbody.transform.eulerAngles = player.Transform.rotation.eulerAngles;
            }

            rigidbody.isKinematic = false;
            rigidbody.freezeRotation = true;
            grabbingPlayers.Remove(player);
        }

        private static List<Player> grabbingPlayers = new();
    }
}