namespace MapEditorReborn.API
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using MEC;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Component added to both child teleport object that were spawnwed by <see cref="TeleportControllerComponent"/>.
    /// </summary>
    public class TeleportComponent : MapEditorObject
    {
        /// <summary>
        /// Instantiates the teleporter.
        /// </summary>
        /// <param name="isEntrance">A value indicating whether the teleport is an entrance.</param>
        /// <returns>Instance of this compoment.</returns>
        public TeleportComponent Init(bool isEntrance)
        {
            IsEntrance = isEntrance;
            Controller = transform.parent.GetComponent<TeleportControllerComponent>();
            GetComponent<BoxCollider>().isTrigger = true;

            UpdateObject();

            return this;
        }

        /// <summary>
        /// A value indicating whether the teleport is an entrance.
        /// </summary>
        public bool IsEntrance;

        /// <summary>
        /// The Controller of this teleport.
        /// </summary>
        public TeleportControllerComponent Controller;

        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject()
        {
            if (Controller.Base.IsVisible)
            {
                if (coinPedestal == null)
                {
                    coinPedestal = new Item(ItemType.Coin).Spawn(Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
                    coinPedestal.Base.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                    coinPedestal.Scale = new Vector3(10f, 10f, 10f);

                    coinPedestal.Locked = true;

                    spinCoin = coinPedestal.Base.gameObject.AddComponent<ItemSpiningComponent>();
                    spinCoin.Speed = 200f;
                }

                coinPedestal.Position = transform.position - (Vector3.up * 0.9f);
            }
            else
            {
                coinPedestal?.Destroy();
                coinPedestal = null;
            }

            if (!first)
            {
                Controller.UpdateIndicator();
            }
            else
            {
                first = false;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<float> SlowdownCoin()
        {
            float num = spinCoin.Speed / 25;

            for (int i = 0; i < 25; i++)
            {
                spinCoin.Speed -= num;
                yield return Timing.WaitForSeconds(Controller.Base.TeleportCooldown / 50);
            }

            for (int i = 0; i < num; i++)
            {
                spinCoin.Speed += 25f;
                yield return Timing.WaitForSeconds(Controller.Base.TeleportCooldown / 50);
            }
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
                player.Position = Controller.ExitTeleport.transform.position;
            }
            else
            {
                player.Position = Controller.EntranceTeleport.transform.position;
            }

            Controller.OnTeleported();
        }

        private void OnDestroy()
        {
            if (coinPedestal != null)
                coinPedestal.Destroy();
        }

        private Pickup coinPedestal;
        private ItemSpiningComponent spinCoin;
        private bool first = true;
    }
}
