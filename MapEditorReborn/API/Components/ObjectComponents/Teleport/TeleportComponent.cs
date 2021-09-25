namespace MapEditorReborn.API
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using MEC;
    using Mirror;
    using UnityEngine;

    public class TeleportComponent : MapEditorObject
    {
        public bool IsEntrance;

        public void Init(bool isEntrance) => IsEntrance = isEntrance;

        public IEnumerator<float> FuniCoin()
        {
            float num = spinCoin.Speed / 25;

            for (int i = 0; i < 25; i++)
            {
                spinCoin.Speed -= num;
                yield return Timing.WaitForSeconds(controller.Base.TeleportCooldown / 50);
            }

            for (int i = 0; i < num; i++)
            {
                spinCoin.Speed += 25f;
                yield return Timing.WaitForSeconds(controller.Base.TeleportCooldown / 50);
            }
        }

        private void Awake()
        {
            controller = transform.parent.GetComponent<TeleportControllerComponent>();

            gameObject.GetComponent<BoxCollider>().isTrigger = true;

            if (controller.Base.IsVisible)
            {
                coinPedestal = new Item(ItemType.Coin).Spawn(transform.position - Vector3.up, Quaternion.Euler(0f, 0f, 0f));
                coinPedestal.Base.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                coinPedestal.Scale = new Vector3(10f, 10f, 10f);
                if (IsEntrance)
                    coinPedestal.Scale = new Vector3(10f, -10f, 10f);

                spinCoin = coinPedestal.Base.gameObject.AddComponent<ItemSpiningComponent>();
                spinCoin.Speed = 200f;
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            if (!IsEntrance && !controller.Base.BothWayMode)
                return;

            if (DateTime.Now < (controller.LastUsed + TimeSpan.FromSeconds(controller.Base.TeleportCooldown)))
                return;

            Player player = Player.Get(collider.GetComponentInParent<NetworkIdentity>()?.gameObject);

            if (player == null)
                return;

            controller.LastUsed = DateTime.Now;

            if (IsEntrance)
            {
                player.Position = controller.ExitTeleport.transform.position;
            }
            else
            {
                player.Position = controller.EntranceTeleport.transform.position;
            }

            controller.OnTeleported();
        }

        private void OnDestroy()
        {
            if (coinPedestal != null)
                coinPedestal.Destroy();
        }

        private TeleportControllerComponent controller;
        private Pickup coinPedestal;
        private ItemSpiningComponent spinCoin;
    }
}
