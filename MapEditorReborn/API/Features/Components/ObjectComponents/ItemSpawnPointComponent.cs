namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;
    using Features.Objects;
    using InventorySystem.Items.Firearms.Attachments;
    using MEC;
    using UnityEngine;

    using Random = UnityEngine.Random;

    /// <summary>
    /// Component added to spawned ItemSpawnPoint. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class ItemSpawnPointComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="ItemSpawnPointComponent"/>.
        /// </summary>
        /// <param name="itemSpawnPoint">The <see cref="ItemSpawnPointObject"/> to initialize.</param>
        /// <returns>Instance of this compoment.</returns>
        public ItemSpawnPointComponent Init(ItemSpawnPointObject itemSpawnPoint)
        {
            Base = itemSpawnPoint;

            ForcedRoomType = itemSpawnPoint.RoomType != RoomType.Unknown ? itemSpawnPoint.RoomType : FindRoom().Type;
            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public ItemSpawnPointObject Base;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            OnDestroy();

            if (Random.Range(0, 101) > Base.SpawnChance)
                return;

            if (Enum.TryParse(Base.Item, out ItemType parsedItem))
            {
                for (int i = 0; i < Base.NumberOfItems; i++)
                {
                    Item item = new Item(parsedItem);
                    Pickup pickup = item.Spawn(transform.position, transform.rotation);

                    if (!Base.UseGravity && pickup.Base.gameObject.TryGetComponent(out Rigidbody rb))
                        rb.isKinematic = true;

                    if (!Base.CanBePickedUp)
                        pickup.Locked = true;

                    if (pickup.Base is InventorySystem.Items.Firearms.FirearmPickup firearmPickup)
                    {
                        int rawCode = GetAttachmentsCode(Base.AttachmentsCode);
                        uint code = rawCode != -1 ? (item.Base as InventorySystem.Items.Firearms.Firearm).ValidateAttachmentsCode((uint)rawCode) : AttachmentsUtils.GetRandomAttachmentsCode(parsedItem);

                        firearmPickup.NetworkStatus = new InventorySystem.Items.Firearms.FirearmStatus(firearmPickup.NetworkStatus.Ammo, firearmPickup.NetworkStatus.Flags, code);
                    }

                    pickup.Scale = transform.localScale;

                    AttachedPickups.Add(pickup);
                }
            }
            else
            {
                Timing.RunCoroutine(SpawnCustomItem());
            }
        }

        private IEnumerator<float> SpawnCustomItem()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);

            for (int i = 0; i < Base.NumberOfItems; i++)
            {
                if (CustomItem.TrySpawn(Base.Item, transform.position, out Pickup customItem))
                {
                    customItem.Rotation = transform.rotation;
                    customItem.Scale = Base.Scale;
                    AttachedPickups.Add(customItem);
                }
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
            foreach (Pickup pickup in AttachedPickups)
            {
                pickup?.Destroy();
            }

            AttachedPickups.Clear();
        }

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="Pickup"/> which contains all attached pickups.
        /// </summary>
        public List<Pickup> AttachedPickups { get; set; } = new List<Pickup>();
    }
}
