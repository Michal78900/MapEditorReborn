namespace MapEditorReborn.API
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;
    using InventorySystem.Items.Firearms.Attachments;
    using MEC;
    using UnityEngine;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Used for handling ItemSpawnPoint's spawning items.
    /// </summary>
    public class ItemSpawnPointComponent : MapEditorObject
    {
        /// <summary>
        /// The name of <see cref="ItemType"/> or <see cref="CustomItem"/>.
        /// </summary>
        public string ItemName = "KeycardJanitor";

        /// <summary>
        /// The attachemts of the a item.
        /// </summary>
        public int AttachmentsCode = -1;

        /// <summary>
        /// The chance for spawning a item.
        /// </summary>
        public int SpawnChance = 100;

        /// <summary>
        /// The number of spawned items.
        /// </summary>
        public uint NumberOfItems = 1;

        /// <summary>
        /// Initializes the <see cref="ItemSpawnPointComponent"/>.
        /// </summary>
        /// <param name="itemSpawnPoint">The <see cref="ItemSpawnPointComponent"/> to initialize.</param>
        public void Init(ItemSpawnPointObject itemSpawnPoint = null)
        {
            if (itemSpawnPoint != null)
            {
                ItemName = itemSpawnPoint.Item;
                AttachmentsCode = GetAttachmentsCode(itemSpawnPoint.AttachmentsCode);
                SpawnChance = itemSpawnPoint.SpawnChance;
                NumberOfItems = itemSpawnPoint.NumberOfItems;
            }

            if (Random.Range(0, 101) > SpawnChance)
                return;

            if (Enum.TryParse(ItemName, out ItemType parsedItem))
            {
                for (int i = 0; i < NumberOfItems; i++)
                {
                    Item item = new Item(parsedItem);
                    Pickup pickup = item.Spawn(transform.position, transform.rotation);

                    if (pickup.Base is InventorySystem.Items.Firearms.FirearmPickup firearmPickup)
                    {
                        uint code = AttachmentsCode != -1 ? (item.Base as InventorySystem.Items.Firearms.Firearm).ValidateAttachmentsCode((uint)AttachmentsCode) : AttachmentsUtils.GetRandomAttachmentsCode(parsedItem);

                        firearmPickup.NetworkStatus = new InventorySystem.Items.Firearms.FirearmStatus(firearmPickup.NetworkStatus.Ammo, firearmPickup.NetworkStatus.Flags, code);
                    }

                    attachedPickups.Add(pickup);
                }
            }
            else
            {
                for (int i = 0; i < NumberOfItems; i++)
                {
                    Timing.RunCoroutine(SpawnCustomItem());
                }
            }
        }

        private IEnumerator<float> SpawnCustomItem()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);

            if (CustomItem.TrySpawn(ItemName, transform.position, out Pickup customItem))
            {
                customItem.Rotation = transform.rotation;
                attachedPickups.Add(customItem);
            }
        }

        private int GetAttachmentsCode(string attachmentsString)
        {
            if (attachmentsString == "-1")
                return -1;

            int attachementsCode = 0;

            if (attachmentsString.Contains("+"))
            {
                string[] array = attachmentsString.Split(new char[] { '+' });

                for (int j = 0; j < array.Length; j++)
                {
                    if (int.TryParse(array[j], out int num))
                    {
                        attachementsCode += num;
                    }
                }
            }
            else
            {
                attachementsCode = int.Parse(attachmentsString);
            }

            return attachementsCode;
        }

        private void OnDestroy()
        {
            foreach (Pickup pickup in attachedPickups)
            {
                pickup.Destroy();
            }
        }

        private List<Pickup> attachedPickups = new List<Pickup>();
    }
}
