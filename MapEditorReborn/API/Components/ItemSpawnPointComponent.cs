namespace MapEditorReborn.API
{
    using System;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Used for handling custom shit.
    /// </summary>
    public class ItemSpawnPointComponent : MonoBehaviour
    {
        /// <summary>
        /// The name of <see cref="ItemType"/> or <see cref="CustomItem"/>.
        /// </summary>
        public string ItemName = "KeycardJanitor";

        /// <summary>
        /// The <see cref="Pickup"/> spawned by the <see cref="ItemSpawnPointObject"/>. May be <see langword="null"/>.
        /// </summary>
        public Pickup AttachedPickup = null;

        private void Start()
        {
            if (CustomItem.TrySpawn(ItemName, gameObject.transform.position, out Pickup customItem))
            {
                customItem.transform.rotation = gameObject.transform.rotation;
                AttachedPickup = customItem;
            }
            else
            {
                ItemType parsedItem = (ItemType)Enum.Parse(typeof(ItemType), ItemName, true);

                AttachedPickup = Item.Spawn(parsedItem, parsedItem.GetDefaultDurability(), gameObject.transform.position, gameObject.transform.rotation);
            }
        }

        private void OnDestroy()
        {
            if (AttachedPickup != null)
                NetworkServer.Destroy(AttachedPickup.gameObject);
        }
    }
}
