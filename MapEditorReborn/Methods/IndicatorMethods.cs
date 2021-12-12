namespace MapEditorReborn
{
    using System;
    using AdminToys;
    using API;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;
    using MEC;
    using Mirror;
    using Mirror.LiteNetLib4Mirror;
    using RemoteAdmin;
    using UnityEngine;

    using Object = UnityEngine.Object;

    public static partial class Methods
    {
        /// <summary>
        /// Spawns indicator that indiactes ItemSpawnPoint object.
        /// </summary>
        /// <param name="itemSpawnPoint">The <see cref="ItemSpawnPointComponent"/> that is used for spawning the indicator.</param>
        /// <param name="indicator">The <see cref="IndicatorObjectComponent"/> that already exists and may be use to just update the indicator.</param>
        public static void SpawnObjectIndicator(ItemSpawnPointComponent itemSpawnPoint, IndicatorObjectComponent indicator = null)
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
                if (indicator.GetComponent<InventorySystem.Items.Pickups.ItemPickupBase>().Info.ItemId == parsedItem)
                {
                    indicator.transform.position = itemSpawnPoint.transform.position;
                    return;
                }
                else
                {
                    SpawnedObjects.Remove(indicator);
                    indicator.Destroy();
                }
            }

            Vector3 scale = Exiled.API.Extensions.ItemExtensions.IsWeapon(parsedItem) ? new Vector3(0.25f, 0.25f, 0.25f) : Vector3.one;

            Pickup pickup = new Item(parsedItem).Create(itemSpawnPoint.transform.position + (Vector3.up * 0.1f * scale.y), Quaternion.identity, scale);
            pickup.Locked = true;

            GameObject pickupGameObject = pickup.Base.gameObject;
            pickupGameObject.GetComponent<Rigidbody>().isKinematic = true;
            pickupGameObject.AddComponent<ItemSpiningComponent>();

            SpawnedObjects.Add(pickupGameObject.AddComponent<IndicatorObjectComponent>().Init(itemSpawnPoint));
            NetworkServer.Spawn(pickupGameObject);
        }

        /// <summary>
        /// Spawns indicator that indiactes PlayerSpawnPoint object.
        /// </summary>
        /// <param name="playerSpawnPoint">The <see cref="PlayerSpawnPointComponent"/> that is used for spawning the indicator.</param>
        /// <param name="indicator">The <see cref="IndicatorObjectComponent"/> that already exists and may be use to just update the indicator.</param>
        public static void SpawnObjectIndicator(PlayerSpawnPointComponent playerSpawnPoint, IndicatorObjectComponent indicator = null)
        {
            if (indicator != null)
            {
                SpawnedObjects.Remove(indicator);
                NetworkServer.Destroy(indicator.gameObject);
            }

            Vector3 position = playerSpawnPoint.transform.position;

            GameObject dummyObject = Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.playerPrefab);
            dummyObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            dummyObject.transform.position = position;

            RoleType roleType = playerSpawnPoint.tag.ConvertToRoleType();

            QueryProcessor processor = dummyObject.GetComponent<QueryProcessor>();

            processor.NetworkPlayerId = QueryProcessor._idIterator++;
            processor._ipAddress = "127.0.0.WAN";

            CharacterClassManager ccm = dummyObject.GetComponent<CharacterClassManager>();
            ccm.CurClass = playerSpawnPoint.tag.ConvertToRoleType();
            ccm.GodMode = true;

            string dummyNickname = roleType.ToString();

            switch (roleType)
            {
                case RoleType.NtfPrivate:
                    dummyNickname = "MTF";
                    break;

                case RoleType.Scp93953:
                    dummyNickname = "SCP939";
                    break;
            }

            NicknameSync nicknameSync = dummyObject.GetComponent<NicknameSync>();
            nicknameSync.Network_myNickSync = "PLAYER SPAWNPOINT";
            nicknameSync.CustomPlayerInfo = $"{dummyNickname}\nSPAWN POINT";
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            SpawnedObjects.Add(dummyObject.AddComponent<IndicatorObjectComponent>().Init(playerSpawnPoint));
            NetworkServer.Spawn(dummyObject);

            ReferenceHub rh = dummyObject.GetComponent<ReferenceHub>();
            Timing.CallDelayed(0.1f, () =>
            {
                rh.playerMovementSync.OverridePosition(position, 0f);
            });
        }

        /// <summary>
        /// Spawns indicator that indiactes RagdollSpawnPoint object.
        /// </summary>
        /// <param name="ragdollSpawnPoint">The <see cref="RagdollSpawnPointComponent"/> that is used for spawning the indicator.</param>
        /// <param name="indicator">The <see cref="IndicatorObjectComponent"/> that already exists and may be use to just update the indicator.</param>
        public static void SpawnObjectIndicator(RagdollSpawnPointComponent ragdollSpawnPoint, IndicatorObjectComponent indicator = null)
        {
            if (indicator != null)
            {
                SpawnedObjects.Remove(indicator);
                NetworkServer.Destroy(indicator.gameObject);
            }

            Vector3 position = ragdollSpawnPoint.transform.position;

            GameObject dummyObject = Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.playerPrefab);
            dummyObject.transform.localScale = new Vector3(-0.2f, -0.2f, -0.2f);
            dummyObject.transform.position = position;

            RoleType roleType = ragdollSpawnPoint.Base.RoleType;

            QueryProcessor processor = dummyObject.GetComponent<QueryProcessor>();
            processor.NetworkPlayerId = QueryProcessor._idIterator++;
            processor._ipAddress = "127.0.0.WAN";

            CharacterClassManager ccm = dummyObject.GetComponent<CharacterClassManager>();
            ccm.CurClass = roleType;
            ccm.GodMode = true;

            string dummyNickname = roleType.ToString();

            switch (roleType)
            {
                case RoleType.NtfPrivate:
                    dummyNickname = "MTF";
                    break;

                case RoleType.ChaosRifleman:
                    dummyNickname = "CI";
                    break;

                case RoleType.Scp93953:
                    dummyNickname = "SCP939";
                    break;
            }

            NicknameSync nicknameSync = dummyObject.GetComponent<NicknameSync>();
            nicknameSync.Network_myNickSync = "RAGDOLL SPAWNPOINT";
            nicknameSync.CustomPlayerInfo = $"{dummyNickname} RAGDOLL\nSPAWN POINT";
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            SpawnedObjects.Add(dummyObject.AddComponent<IndicatorObjectComponent>().Init(ragdollSpawnPoint));
            NetworkServer.Spawn(dummyObject);

            ReferenceHub rh = dummyObject.GetComponent<ReferenceHub>();
            Timing.CallDelayed(0.1f, () =>
            {
                rh.playerMovementSync.OverridePosition(position, 0f);
            });
        }

        public static void SpawnObjectIndicator(LightSourceComponent lightSource, IndicatorObjectComponent indicator = null)
        {
            if (indicator != null)
            {
                indicator.transform.position = lightSource.transform.position;
                return;
            }

            Pickup pickup = new Item(ItemType.SCP2176).Create(lightSource.transform.position, Quaternion.Euler(180f, 0f, 0f), Vector3.one * 2f);
            pickup.Locked = true;
            GameObject pickupGameObject = pickup.Base.gameObject;
            pickupGameObject.GetComponent<Rigidbody>().isKinematic = true;

            SpawnedObjects.Add(pickupGameObject.AddComponent<IndicatorObjectComponent>().Init(lightSource));
            NetworkServer.Spawn(pickupGameObject);

        }

        public static void SpawnObjectIndicator(TeleportComponent teleport, IndicatorObjectComponent indicator = null)
        {
            PrimitiveObjectToy primitive;

            if (indicator != null)
            {
                primitive = indicator.GetComponent<PrimitiveObjectToy>();
                primitive.transform.position = teleport.transform.position;
                primitive.transform.localScale = teleport.transform.localScale;
                primitive.UpdatePositionServer();
                return;
            }

            primitive = Object.Instantiate(ToolGunMode.Primitive.GetObjectByMode(), teleport.transform.position, Quaternion.identity).GetComponent<PrimitiveObjectToy>();
            primitive.NetworkPrimitiveType = PrimitiveType.Cube;
            primitive.NetworkMaterialColor = teleport.IsEntrance ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
            primitive.NetworkScale = -teleport.Scale;

            SpawnedObjects.Add(primitive.gameObject.AddComponent<IndicatorObjectComponent>().Init(teleport));
            NetworkServer.Spawn(primitive.gameObject);
        }
    }
}
