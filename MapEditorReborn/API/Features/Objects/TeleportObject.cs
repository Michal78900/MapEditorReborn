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

        private static int Choose(List<TargetTeleporter> teleports)
        {
            float total = 0;

            foreach (TargetTeleporter elem in teleports.ToList())
            {
                if (elem.Chance <= 0f)
                {
                    teleports.Remove(elem);

                    if (teleports.Count == 0)
                        return -1;

                    continue;
                }

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

        public TeleportObject Init(SerializableTeleport teleportSerializable)
        {
            Base = teleportSerializable;
            GetComponent<BoxCollider>().isTrigger = true;

            RefreshTargets();

            return this;
        }

        public void RefreshTargets()
        {
            Timing.CallDelayed(0.1f, () =>
            {
                TargetFromId.Clear();

                try
                {
                    foreach (MapEditorObject mapEditorObject in API.SpawnedObjects)
                    {
                        if (mapEditorObject == this)
                            continue;

                        if (mapEditorObject is SchematicObject schematic)
                        {
                            foreach (TargetTeleporter target in Base.TargetTeleporters)
                            {
                                if (schematic.ObjectFromId.TryGetValue(target.Id, out Transform transform) && !TargetFromId.ContainsKey(target.Id))
                                    TargetFromId.Add(target.Id, transform.GetComponent<TeleportObject>());
                            }

                            continue;
                        }

                        if (mapEditorObject is TeleportObject teleport)
                        {
                            foreach (TargetTeleporter target in Base.TargetTeleporters)
                            {
                                if (teleport.Base.ObjectId == target.Id && !TargetFromId.ContainsKey(target.Id))
                                    TargetFromId.Add(target.Id, teleport);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });
        }

        internal void SetPreviousTransform()
        {
            _prevPostion = Position;
            _prevRotation = Rotation;
            _prevScale = Scale;
        }

        internal void FixTransform()
        {
            if(_prevPostion is not null)
                Position = _prevPostion.Value;

            if (_prevRotation is not null)
                Rotation = _prevRotation.Value;

            if (_prevScale is not null)
                Scale = _prevScale.Value;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (TargetFromId.Count == 0)
                return;

            if (!CanBeTeleported(collider, out GameObject gameObject))
                return;

            Player player = Player.Get(gameObject);
            if (player is not null && !Base.AllowedRoles.Contains(player.Role.Type.ToString()))
                return;

            int choosenTeleporter = Choose(Base.TargetTeleporters);
            if (choosenTeleporter == -1)
                return;

            TeleportObject target = TargetFromId[choosenTeleporter];

            // TeleportingEventArgs ev = new(this, IsEntrance, gameObject, player, destination);
            // Events.Handlers.Teleport.OnTeleporting(ev);

            // gameObject = ev.TeleportedObject;
            // player = ev.TeleportedPlayer;
            // destination = ev.Destination;

            // if (!ev.IsAllowed)
            // return;

            NextTimeUse = DateTime.Now.AddSeconds(Base.Cooldown);
            target.NextTimeUse = DateTime.Now.AddSeconds(target.Base.Cooldown);

            if (player is null)
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

        private void OnDestroy()
        {
            Timing.CallDelayed(0.1f, () =>
            {
                try
                {
                    foreach (TeleportObject teleport in TargetFromId.Values)
                    {
                        teleport.RefreshTargets();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });
        }

        private Vector3? _prevPostion;
        private Quaternion? _prevRotation;
        private Vector3? _prevScale;
    }
}
