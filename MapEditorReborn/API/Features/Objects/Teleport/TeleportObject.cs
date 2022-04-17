// -----------------------------------------------------------------------
// <copyright file="TeleportObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections.Generic;
    using Components;
    using Enums;
    using Events.EventArgs;
    using Exiled.API.Features;
    using Extensions;
    using MEC;
    using Mirror;
    using Serializable.Teleport;
    using UnityEngine;

    using Random = UnityEngine.Random;

    /// <summary>
    /// The component added to both child teleport object that were spawnwed by <see cref="TeleportControllerObject"/>.
    /// </summary>
    public class TeleportObject : MapEditorObject
    {
        public SerializableTeleport Base;

        public Dictionary<int, TeleportObject> TargetFromId = new Dictionary<int, TeleportObject>();

        public DateTime NextTimeUse;

        /*
        /// <summary>
        /// Gets or sets teleport chance.
        /// </summary>
        public float Chance { get; set; }

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        public TeleportControllerObject Controller { get; set; }

        /// <summary>
        /// Gets a value indicating whether the teleport is an entrance.
        /// </summary>
        public bool IsEntrance => Chance == -1f;
        */

        private static int Choose(List<TargetTeleporter> teleports)
        {
            float total = 0;

            foreach (TargetTeleporter elem in teleports)
            {
                total += elem.Chance;
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < teleports.Count; i++)
            {
                if (randomPoint < teleports[i].Chance)
                {
                    return teleports[i].Id;
                }
                else
                {
                    randomPoint -= teleports[i].Chance;
                }
            }

            return teleports[teleports.Count - 1].Id;
        }

        /*
        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject() => this.UpdateIndicator();
        */

        public TeleportObject Init(SerializableTeleport teleportSerializable, SchematicObject schematic)
        {
            Base = teleportSerializable;
            GetComponent<BoxCollider>().isTrigger = true;

            Timing.RunCoroutine(AddShitDelayed(schematic));

            return this;
        }

        private IEnumerator<float> AddShitDelayed(SchematicObject schematic)
        {
            yield return Timing.WaitUntilTrue(() => schematic.IsBuilt);

            foreach (var shit in Base.TargetTeleporters)
            {
                TargetFromId.Add(shit.Id, schematic.ObjectFromId[shit.Id].GetComponent<TeleportObject>());
            }
        }

        /*
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportObject"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="chance">The required <see cref="TeleportControllerObject"/>.</param>
        /// <param name="spawnIndicator">A value indicating whether the indicator should be spawned.</param>
        /// <returns>The initialized <see cref="TeleportObject"/> instance.</returns>
        public TeleportObject Init(TeleportControllerObject controller, float chance, bool spawnIndicator = false)
        {
            Controller = controller;
            Chance = chance;

            if (TryGetComponent(out BoxCollider boxCollider))
            {
                boxCollider.isTrigger = true;

                if (spawnIndicator)
                    UpdateObject();

                return this;
            }

            return null;
        }
        */

        private void OnTriggerEnter(Collider collider)
        {
            if (!CanBeTeleported(collider, out GameObject gameObject))
                return;

            Player player = Player.Get(gameObject);

            // Vector3 destination = IsEntrance ? Choose(Controller.ExitTeleports).Position : Controller.EntranceTeleport.Position;
            TeleportObject target = TargetFromId[Choose(Base.TargetTeleporters)];
            Vector3 destination = target.Position;

            // TeleportingEventArgs ev = new(this, IsEntrance, gameObject, player, destination);
            // Events.Handlers.Teleport.OnTeleporting(ev);

            // gameObject = ev.TeleportedObject;
            // player = ev.TeleportedPlayer;
            // destination = ev.Destination;

            // if (!ev.IsAllowed)
            // return;

            NextTimeUse = DateTime.Now.AddSeconds(Base.Cooldown);
            target.NextTimeUse = DateTime.Now.AddSeconds(target.Base.Cooldown);

            // Controller.LastUsed = DateTime.Now;

            if (player == null)
            {
                gameObject.transform.position = destination;
                return;
            }

            player.Position = destination;

            Vector2 syncRotation = player.Rotation;
            PlayerMovementSync.PlayerRotation newRotation = new(target.Base.PlayerRotationX, target.Base.PlayerRotationY);

            if (newRotation.x.HasValue)
                syncRotation.x = newRotation.x.Value;

            if (newRotation.y.HasValue)
                syncRotation.y = newRotation.y.Value;

            if (newRotation.x.HasValue || newRotation.y.HasValue)
            {
                player.ReferenceHub.playerMovementSync.NetworkRotationSync = syncRotation;
                player.ReferenceHub.playerMovementSync.ForceRotation(newRotation);
            }

            Log.Debug(1);

            if (Base.TeleportSoundId != -1)
                Exiled.API.Extensions.MirrorExtensions.SendFakeTargetRpc(player, ReferenceHub.HostHub.networkIdentity, typeof(AmbientSoundPlayer), "RpcPlaySound", Base.TeleportSoundId);

            Log.Debug(2);
        }

        private bool CanBeTeleported(Collider collider, out GameObject gameObject)
        {
            gameObject = null;

            bool flag =
                !CullingComponent.CullingColliders.Contains(collider) &&
                (!Map.IsLczDecontaminated || !Base.LockOnEvent.HasFlagFast(LockOnEvent.LightDecontaminated)) &&
                (!Warhead.IsDetonated || !Base.LockOnEvent.HasFlagFast(LockOnEvent.WarheadDetonated)) &&
                DateTime.Now >= NextTimeUse;

            if (!flag)
                return false;

            gameObject = collider.GetComponentInParent<NetworkIdentity>()?.gameObject;

            return gameObject.tag switch
            {
                "Player" => Base.TeleportFlags.HasFlagFast(TeleportFlags.Player),
                "Pickup" => Base.TeleportFlags.HasFlagFast(TeleportFlags.Pickup),
                _ => (gameObject.name.Contains("Projectile") && Base.TeleportFlags.HasFlagFast(TeleportFlags.ActiveGrenade)) ||
                     (gameObject.name.Contains("Pickup") && Base.TeleportFlags.HasFlagFast(TeleportFlags.Pickup)),
            };
        }

        // private void OnDestroy() => Controller.ExitTeleports.Remove(this);
    }
}
