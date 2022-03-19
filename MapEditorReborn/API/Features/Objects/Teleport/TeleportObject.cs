namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections.Generic;
    using Components;
    using Enums;
    using Events.EventArgs;
    using Exiled.API.Features;
    using Extensions;
    using Mirror;
    using UnityEngine;

    using Random = UnityEngine.Random;

    /// <summary>
    /// The component added to both child teleport object that were spawnwed by <see cref="TeleportControllerObject"/>.
    /// </summary>
    public class TeleportObject : MapEditorObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportObject"/> class.
        /// </summary>
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

        /// <summary>
        /// The teleport chance.
        /// </summary>
        public float Chance;

        /// <summary>
        /// The controller of this teleport.
        /// </summary>
        public TeleportControllerObject Controller;

        /// <summary>
        /// Gets a value indicating whether the teleport is an entrance.
        /// </summary>
        public bool IsEntrance => Chance == -1f;

        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject() => this.UpdateIndicator();

        private void OnTriggerEnter(Collider collider)
        {
            if (!CanBeTeleported(collider, out GameObject gameObject))
                return;

            Player player = Player.Get(gameObject);
            Vector3 destination = IsEntrance ? Choose(Controller.ExitTeleports).Position : Controller.EntranceTeleport.Position;

            TeleportingEventArgs ev = new TeleportingEventArgs(this, IsEntrance, gameObject, player, destination);
            Events.Handlers.Teleport.OnTeleporting(ev);

            gameObject = ev.TeleportedObject;
            player = ev.TeleportedPlayer;
            destination = ev.Destination;

            if (!ev.IsAllowed)
                return;

            Controller.LastUsed = DateTime.Now;

            _ = player != null ? player.Position = destination : gameObject.transform.position = destination;
        }

        private bool CanBeTeleported(Collider collider, out GameObject gameObject)
        {
            gameObject = null;

            bool flag = (IsEntrance || Controller.Base.BothWayMode) &&
                !CullingComponent.CullingColliders.Contains(collider) &&
                (!Map.IsLczDecontaminated || !Controller.Base.LockOnEvent.HasFlagFast(LockOnEvent.LightDecontaminated)) &&
                (!Warhead.IsDetonated || !Controller.Base.LockOnEvent.HasFlagFast(LockOnEvent.WarheadDetonated)) &&
                DateTime.Now >= (Controller.LastUsed + TimeSpan.FromSeconds(Controller.Base.TeleportCooldown));

            if (!flag)
                return false;

            gameObject = collider.GetComponentInParent<NetworkIdentity>()?.gameObject;

            return gameObject.tag switch
            {
                "Player" => Controller.Base.TeleportFlags.HasFlagFast(TeleportFlags.Player),
                "Pickup" => Controller.Base.TeleportFlags.HasFlagFast(TeleportFlags.Pickup),
                _ => (gameObject.name.Contains("Projectile") && Controller.Base.TeleportFlags.HasFlagFast(TeleportFlags.ActiveGrenade)) ||
                     (gameObject.name.Contains("Pickup") && Controller.Base.TeleportFlags.HasFlagFast(TeleportFlags.Pickup)),
            };
        }

        private static TeleportObject Choose(List<TeleportObject> teleports)
        {
            float total = 0;

            foreach (var elem in teleports)
            {
                total += elem.Chance;
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < teleports.Count; i++)
            {
                if (randomPoint < teleports[i].Chance)
                {
                    return teleports[i];
                }
                else
                {
                    randomPoint -= teleports[i].Chance;
                }
            }

            return teleports[teleports.Count - 1];
        }

        private void OnDestroy() => Controller.ExitTeleports.Remove(this);
    }
}
