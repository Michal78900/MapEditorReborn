namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using System;
    using Exiled.API.Features;
    using Extensions;
    using Mirror;
    using UnityEngine;

    using Random = UnityEngine.Random;

    /// <summary>
    /// The component added to both child teleport object that were spawnwed by <see cref="TeleportControllerComponent"/>.
    /// </summary>
    public class TeleportComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportComponent"/> class.
        /// </summary>
        /// <param name="chance">The required <see cref="TeleportControllerComponent"/>.</param>
        /// <param name="spawnIndicator">A value indicating whether the indicator should be spawned.</param>
        /// <returns>The initialized <see cref="TeleportComponent"/> instance.</returns>
        public TeleportComponent Init(float chance, bool spawnIndicator = false)
        {
            Chance = chance;
            Controller = transform.parent.GetComponent<TeleportControllerComponent>();

            if (transform.parent.TryGetComponent(out TeleportControllerComponent teleportControllerComponent) && TryGetComponent(out BoxCollider boxCollider))
            {
                Controller = teleportControllerComponent;
                boxCollider.isTrigger = true;

                if (spawnIndicator)
                    UpdateObject();

                return this;
            }

            return null;
        }

        /// <summary>
        /// Gets a value indicating whether the teleport is an entrance.
        /// </summary>
        public bool IsEntrance => Chance == -1f;

        /// <summary>
        /// The teleport chance.
        /// </summary>
        public float Chance;

        /// <summary>
        /// The Controller of this teleport.
        /// </summary>
        public TeleportControllerComponent Controller;

        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject()
        {
            this.UpdateIndicator();
        }

        private void OnTriggerStay(Collider collider)
        {
            if (!IsEntrance && !Controller.Base.BothWayMode)
                return;

            if (DateTime.Now < (Controller.LastUsed + TimeSpan.FromSeconds(Controller.Base.TeleportCooldown)))
                return;

            Player player = Player.Get(collider.GetComponentInParent<NetworkIdentity>()?.gameObject);

            if (player == null)
                return;

            Controller.LastUsed = DateTime.Now;

            if (IsEntrance)
            {
                TeleportComponent choosedExit = Choose(Controller.ExitTeleports.ToArray());

                player.Position = choosedExit.transform.position;
            }
            else
            {
                player.Position = Controller.EntranceTeleport.transform.position;
            }
        }

        private TeleportComponent Choose(TeleportComponent[] teleports)
        {
            float total = 0;

            foreach (var elem in teleports)
            {
                total += elem.Chance;
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < teleports.Length; i++)
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

            return teleports[teleports.Length - 1];
        }

        private void OnDestroy()
        {
            Controller.ExitTeleports.Remove(this);
        }
    }
}
