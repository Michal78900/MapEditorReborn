// -----------------------------------------------------------------------
// <copyright file="Indicator.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features
{
    using System;
    using AdminToys;
    using Components;
    using Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomItems.API.Features;
    using Extensions;
    using InventorySystem.Items.Pickups;
    using Mirror;
    using Objects;
    using UnityEngine;
    using static API;
    using Object = UnityEngine.Object;

    /// <summary>
    /// A tool used to mark and save positions.
    /// </summary>
    public static class Indicator
    {
        /// <summary>
        /// Spawns a <see cref="IndicatorObject"/> given a specified <see cref="ItemSpawnPointObject"/>.
        /// </summary>
        /// <param name="itemSpawnPoint">The specified <see cref="PlayerSpawnPointObject"/>.</param>
        /// <param name="indicator">The <see cref="IndicatorObject"/> attached to the specified <see cref="PlayerSpawnPointObject"/>.</param>
        public static void SpawnObjectIndicator(ItemSpawnPointObject itemSpawnPoint, IndicatorObject indicator = null)
        {
            ItemType parsedItem;

            if (CustomItem.TryGet(itemSpawnPoint.Base.Item, out CustomItem custom))
            {
                parsedItem = custom.Type;
            }
            else
            {
                parsedItem = (ItemType)Enum.Parse(typeof(ItemType), itemSpawnPoint.Base.Item, true);
            }

            if (indicator != null)
            {
                if (indicator.TryGetComponent(out ItemPickupBase ipb) && ipb.Info.ItemId == parsedItem)
                {
                    indicator.transform.position = itemSpawnPoint.transform.position;
                    return;
                }

                SpawnedObjects.Remove(indicator);
                indicator.Destroy();
            }

            Vector3 scale = parsedItem.IsWeapon() ? new Vector3(0.25f, 0.25f, 0.25f) : Vector3.one;

            Pickup pickup = Item.Create(parsedItem).CreatePickup(itemSpawnPoint.transform.position + (Vector3.up * 0.1f * scale.y), Quaternion.identity, scale);
            pickup.IsLocked = true;

            GameObject pickupGameObject = pickup.Base.gameObject;

            if (pickupGameObject.TryGetComponent(out Rigidbody rb))
                rb.isKinematic = true;

            pickupGameObject.AddComponent<ItemSpinningComponent>();

            SpawnedObjects.Add(pickupGameObject.AddComponent<IndicatorObject>().Init(itemSpawnPoint));
            NetworkServer.Spawn(pickupGameObject);
        }

        /// <summary>
        /// Spawns a <see cref="IndicatorObject"/> given a specified <see cref="PlayerSpawnPointObject"/>.
        /// </summary>
        /// <param name="playerSpawnPoint">The specified <see cref="PlayerSpawnPointObject"/>.</param>
        /// <param name="indicator">The <see cref="IndicatorObject"/> attached to the specified <see cref="PlayerSpawnPointObject"/>.</param>
        public static void SpawnObjectIndicator(PlayerSpawnPointObject playerSpawnPoint, IndicatorObject indicator = null)
        {
            PrimitiveObjectToy primitive;

            if (indicator != null && indicator.TryGetComponent(out primitive))
            {
                primitive.transform.position = playerSpawnPoint.Position;
                return;
            }

            if (Object.Instantiate(ObjectType.Primitive.GetObjectByMode(), playerSpawnPoint.Position, Quaternion.identity).TryGetComponent(out primitive))
            {
                primitive.NetworkPrimitiveType = PrimitiveType.Cube;
                primitive.NetworkMaterialColor = Color.green;
                primitive.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
                primitive.NetworkMovementSmoothing = 60;
            }

            SpawnedObjects.Add(primitive.gameObject.AddComponent<IndicatorObject>().Init(playerSpawnPoint));
            NetworkServer.Spawn(primitive.gameObject);
        }

        /// <summary>
        /// Spawns a <see cref="IndicatorObject"/> given a specified <see cref="RagdollSpawnPointObject"/>.
        /// </summary>
        /// <param name="ragdollSpawnPoint">The specified <see cref="RagdollSpawnPointObject"/>.</param>
        /// <param name="indicator">The <see cref="IndicatorObject"/> attached to the specified <see cref="RagdollSpawnPointObject"/>.</param>
        public static void SpawnObjectIndicator(RagdollSpawnPointObject ragdollSpawnPoint, IndicatorObject indicator = null)
        {
        }

        /// <summary>
        /// Spawns a <see cref="IndicatorObject"/> given a specified <see cref="LightSourceObject"/>.
        /// </summary>
        /// <param name="lightSource">The specified <see cref="LightSourceObject"/>.</param>
        /// <param name="indicator">The <see cref="IndicatorObject"/> attached to the specified <see cref="LightSourceObject"/>.</param>
        public static void SpawnObjectIndicator(LightSourceObject lightSource, IndicatorObject indicator = null)
        {
            if (indicator != null)
            {
                indicator.transform.position = lightSource.transform.position;
                return;
            }

            Pickup pickup = Item.Create(ItemType.SCP2176).CreatePickup(lightSource.transform.position, Quaternion.Euler(180f, 0f, 0f), Vector3.one * 2f);
            pickup.IsLocked = true;

            GameObject pickupGameObject = pickup.Base.gameObject;
            if (pickupGameObject.gameObject.TryGetComponent(out Rigidbody rb))
                rb.isKinematic = true;

            SpawnedObjects.Add(pickupGameObject.AddComponent<IndicatorObject>().Init(lightSource));
            NetworkServer.Spawn(pickupGameObject);
        }

        /// <summary>
        /// Spawns a <see cref="IndicatorObject"/> given a specified <see cref="TeleportObject"/>.
        /// </summary>
        /// <param name="teleport">The specified <see cref="TeleportObject"/>.</param>
        /// <param name="indicator">The <see cref="IndicatorObject"/> attached to the specified <see cref="TeleportObject"/>.</param>
        public static void SpawnObjectIndicator(TeleportObject teleport, IndicatorObject indicator = null)
        {
            PrimitiveObjectToy primitive;

            if (indicator != null && indicator.TryGetComponent(out primitive))
            {
                primitive.transform.position = teleport.Position;
                return;
            }

            if (Object.Instantiate(ObjectType.Primitive.GetObjectByMode(), teleport.Position, Quaternion.identity).TryGetComponent(out primitive))
            {
                primitive.NetworkPrimitiveType = PrimitiveType.Cube;
                primitive.NetworkMaterialColor = Color.cyan;
                primitive.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
                primitive.NetworkMovementSmoothing = 60;
            }

            SpawnedObjects.Add(primitive.gameObject.AddComponent<IndicatorObject>().Init(teleport));
            NetworkServer.Spawn(primitive.gameObject);
        }
    }
}