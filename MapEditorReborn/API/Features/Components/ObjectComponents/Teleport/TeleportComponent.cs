namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using System;
    using Exiled.API.Features;
    using Extensions;
    using Mirror;
    using UnityEngine;

    using Random = UnityEngine.Random;

    /// <summary>
    /// Component added to both child teleport object that were spawnwed by <see cref="TeleportControllerComponent"/>.
    /// </summary>
    public class TeleportComponent : MapEditorObject
    {
        /// <summary>
        /// Instantiates the teleporter.
        /// </summary>
        /// <returns>Instance of this compoment.</returns>
        public TeleportComponent Init(float chance, bool spawnIndicator = false)
        {
            Chance = chance;
            Controller = transform.parent.GetComponent<TeleportControllerComponent>();
            GetComponent<BoxCollider>().isTrigger = true;

            if (spawnIndicator)
                UpdateObject();

            return this;
        }

        /// <summary>
        /// Gets a value indicating whether the teleport is an entrance.
        /// </summary>
        public bool IsEntrance => Chance == -1f;

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
