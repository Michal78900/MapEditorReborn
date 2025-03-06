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
    using Enums;
    using Events.EventArgs;
    using Events.Handlers;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Extensions;
    using MEC;
    using Mirror;
    using Serializable;
    using UnityEngine;
    using Map = Exiled.API.Features.Map;
    using Random = UnityEngine.Random;

    public class TeleportObject : MapEditorObject
    {
        public SerializableTeleport Base;

        public Dictionary<int, TeleportObject> TargetFromId = new();

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

                randomPoint -= teleports[i].Chance;
            }

            return teleports[teleports.Count - 1].Id;
        }

        private static int GetUniqId()
        {
            int id = 0;

            // Get currently used ids.
            HashSet<int> usedIds = new(API.SpawnedObjects.Where(x => x is TeleportObject).Select(x => ((TeleportObject)x).Base.ObjectId));

            // Increment id until it is unique.
            while (usedIds.Contains(id))
                id++;

            return id;
        }

        public TeleportObject Init(SerializableTeleport serializableTeleport, bool first = false)
        {
            Base = serializableTeleport;

            if (first)
            {
                Base.ObjectId = GetUniqId();

                TeleportObject lastTeleport = API.SpawnedObjects.LastOrDefault(x => x is TeleportObject teleport) as TeleportObject;

                if (lastTeleport is not null)
                    Base.TargetTeleporters[0].Id = lastTeleport.Base.ObjectId;
            }

            GetComponent<BoxCollider>().isTrigger = true;
            RefreshTargets();

            return this;
        }

        public override void UpdateObject()
        {
            RefreshTargets();
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
            _prevposition = Position;
            _prevRotation = Rotation;
            _prevScale = Scale;
        }

        internal void FixTransform()
        {
            if (_prevposition is not null)
                Position = _prevposition.Value;

            if (_prevRotation is not null)
                Rotation = _prevRotation.Value;

            if (_prevScale is not null)
                Scale = _prevScale.Value;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!CanBeTeleported(collider, out GameObject gameObject))
                return;

            Player player = Player.Get(gameObject);
            if (player is not null && !Base.AllowedRoles.Contains(player.Role.Type.ToString()))
                return;

            if (player is not null && player.Role.Base.ActiveTime < 0.25f)
                return;

            int choosenTeleporter = Choose(Base.TargetTeleporters);
            if (choosenTeleporter == -1)
                return;

            TeleportObject target = TargetFromId[choosenTeleporter];


            Log.Debug($"Defined Rotation ({(target.Base.PlayerRotationX.HasValue ? target.Base.PlayerRotationX.Value.ToString() : "Not Defined")}, {(target.Base.PlayerRotationY.HasValue ? target.Base.PlayerRotationY.Value.ToString() : "Not Defined")})");
            Vector2 PlayerRotation = new Vector2
            {
                x = target.Base.PlayerRotationX ?? player.Rotation.x,
                y = target.Base.PlayerRotationY ?? player.Rotation.y
            };
            // new(target.Base.PlayerRotationX, target.Base.PlayerRotationY)
            TeleportingEventArgs ev = new(this, target, player, gameObject, target.Position, PlayerRotation, Base.TeleportSoundId);
            Teleport.OnTeleporting(ev);

            if (!ev.IsAllowed)
                return;

            gameObject = ev.GameObject;
            player = ev.Player;
            Vector3 destination = ev.Destination;
            // PlayerMovementSync.PlayerRotation playerRotation = ev.PlayerRotation;
            int teleportSoundId = ev.TeleportSoundId;

            NextTimeUse = DateTime.Now.AddSeconds(Base.Cooldown);
            target.NextTimeUse = DateTime.Now.AddSeconds(target.Base.Cooldown);

            if (player is null)
            {
                gameObject.transform.position = destination;
                return;
            }

            player.Position = destination;

            player.Rotation = Quaternion.Euler(PlayerRotation);
            Log.Debug($"Final Player Rotation ({PlayerRotation.x}, {PlayerRotation.y})");


            if (teleportSoundId != -1)
            {
                Log.Assert(teleportSoundId >= 0 && teleportSoundId <= 31, $"The teleport sound id must be between 0 and 31. It is currently {teleportSoundId} for teleport with {Base.ObjectId} ID.");
                MirrorExtensions.SendFakeTargetRpc(player, ReferenceHub.HostHub.networkIdentity, typeof(AmbientSoundPlayer), "RpcPlaySound", teleportSoundId);
            }
        }

        private bool CanBeTeleported(Collider collider, out GameObject gameObject)
        {
            gameObject = null;

            if (TargetFromId.Count == 0)
                return false;

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

        private Vector3? _prevposition;
        private Quaternion? _prevRotation;
        private Vector3? _prevScale;
    }
}