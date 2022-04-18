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
    using System.Linq;
    using Components;
    using Enums;
    using Events.EventArgs;
    using Exiled.API.Features;
    using Extensions;
    using MEC;
    using Mirror;
    using Serializable;
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

        public TeleportObject Init(SerializableTeleport teleportSerializable)
        {
            Base = teleportSerializable;
            GetComponent<BoxCollider>().isTrigger = true;

            Timing.RunCoroutine(AddTargetsDelayed());

            return this;
        }

        public TeleportObject Init(SerializableTeleport teleportSerializable, SchematicObject schematic)
        {
            IsSchematicBlock = true;

            Base = teleportSerializable;
            GetComponent<BoxCollider>().isTrigger = true;

            Timing.RunCoroutine(AddTargetsDelayed(schematic));

            return this;
        }

        private IEnumerator<float> AddTargetsDelayed()
        {
            yield return Timing.WaitForSeconds(1f);

            try
            {
                foreach (TargetTeleporter target in Base.TargetTeleporters)
                {
                    TeleportObject foundTarget = API.SpawnedObjects.FirstOrDefault(x => x is TeleportObject teleport && teleport.Base.ObjectId == target.Id) as TeleportObject;

                    if (foundTarget is null)
                    {
                        Log.Warn("Could not find target teleport with id " + target.Id);
                        continue;
                    }

                    TargetFromId.Add(target.Id, foundTarget.GetComponent<TeleportObject>());
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private IEnumerator<float> AddTargetsDelayed(SchematicObject schematic)
        {
            yield return Timing.WaitUntilTrue(() => schematic.IsBuilt);

            foreach (TargetTeleporter target in Base.TargetTeleporters)
            {
                TargetFromId.Add(target.Id, schematic.ObjectFromId[target.Id].GetComponent<TeleportObject>());
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

            if (player != null && !Base.AllowedRoles.Contains(player.Role.Type.ToString()))
                return;

            TeleportObject target = TargetFromId[Choose(Base.TargetTeleporters)];

            // TeleportingEventArgs ev = new(this, IsEntrance, gameObject, player, destination);
            // Events.Handlers.Teleport.OnTeleporting(ev);

            // gameObject = ev.TeleportedObject;
            // player = ev.TeleportedPlayer;
            // destination = ev.Destination;

            // if (!ev.IsAllowed)
            // return;

            NextTimeUse = DateTime.Now.AddSeconds(Base.Cooldown);
            target.NextTimeUse = DateTime.Now.AddSeconds(target.Base.Cooldown);

            if (player == null)
            {
                gameObject.transform.position = target.Position;
                return;
            }

            player.Position = target.Position;

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

            if (Base.TeleportSoundId != -1)
                Exiled.API.Extensions.MirrorExtensions.SendFakeTargetRpc(player, ReferenceHub.HostHub.networkIdentity, typeof(AmbientSoundPlayer), "RpcPlaySound", Base.TeleportSoundId);
        }

        private bool CanBeTeleported(Collider collider, out GameObject gameObject)
        {
            gameObject = null;

            bool flag =
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
