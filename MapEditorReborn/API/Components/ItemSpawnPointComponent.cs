namespace MapEditorReborn.API
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using MEC;
    using Mirror;
    using UnityEngine;

    using Random = UnityEngine.Random;

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
        /// The chance for spawning a item.
        /// </summary>
        public int SpawnChance = 100;

        /// <summary>
        /// The <see cref="Pickup"/> spawned by the <see cref="ItemSpawnPointObject"/>. May be <see langword="null"/>.
        /// </summary>
        public Pickup AttachedPickup = null;

        private void Start()
        {
            if (Random.Range(0, 101) > SpawnChance)
                return;

            if (Enum.TryParse(ItemName, out ItemType parsedItem))
            {
                AttachedPickup = Item.Spawn(parsedItem, parsedItem.GetDefaultDurability(), gameObject.transform.position, gameObject.transform.rotation);
            }
            else
            {
                Timing.RunCoroutine(SpawnCustomItem());
            }
        }

        private IEnumerator<float> SpawnCustomItem()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);

            if (CustomItem.TrySpawn(ItemName, gameObject.transform.position, out Pickup customItem))
            {
                customItem.transform.rotation = gameObject.transform.rotation;
                AttachedPickup = customItem;
            }
        }

        private void OnDestroy()
        {
            if (AttachedPickup != null)
                NetworkServer.Destroy(AttachedPickup.gameObject);
        }
    }
}
