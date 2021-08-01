namespace MapEditorReborn.Commands
{
    using System;
    using System.Linq;
    using API;
    using CommandSystem;
    using Exiled.API.Extensions;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;
    using MEC;
    using Mirror;
    using RemoteAdmin;
    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// Command used for showing indicators.
    /// </summary>
    public class ShowIndicators : ICommand
    {
        /// <inheritdoc/>
        public string Command => "showindicators";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "si" };

        /// <inheritdoc/>
        public string Description => "Shows indicators for both player and item spawn points.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            if (Handler.Indicators.Count != 0)
            {
                foreach (GameObject indicator in Handler.Indicators.Keys)
                {
                    NetworkServer.Destroy(indicator);
                }

                Handler.Indicators.Clear();

                response = "Removed all indicators!";
                return true;
            }

            foreach (GameObject gameObject in Handler.SpawnedObjects)
            {
                switch (gameObject.name)
                {
                    case "PlayerSpawnPointObject(Clone)":
                        {
                            SpawnDummyIndicator(gameObject.transform.position, new Vector3(0.2f, 0.2f, 0.2f), gameObject.tag.ConvertToRoleType(), gameObject);

                            break;
                        }

                    case "ItemSpawnPointObject(Clone)":
                        {
                            ItemSpawnPointComponent itemSpawnPointComponent = gameObject.GetComponent<ItemSpawnPointComponent>();

                            SpawnPickupIndicator(gameObject.transform.position, gameObject.transform.rotation, itemSpawnPointComponent.ItemName, gameObject);

                            break;
                        }
                }
            }

            response = "Indicators have been shown!";
            return true;
        }

        public static void SpawnPickupIndicator(Vector3 position, Quaternion rotation, string name, GameObject callingItemSpawnPointObject)
        {
            ItemType parsedItem;

            if (CustomItem.TryGet(name, out CustomItem custom))
            {
                parsedItem = custom.Type;
            }
            else
            {
                parsedItem = (ItemType)Enum.Parse(typeof(ItemType), name, true);
            }

            GameObject pickupGameObject = Item.Spawn(parsedItem, parsedItem.GetDefaultDurability(), position + (Vector3.up * 0.1f), rotation).gameObject;
            NetworkServer.UnSpawn(pickupGameObject);

            Rigidbody rigidbody = pickupGameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.freezeRotation = true;
            rigidbody.mass = 100000;

            Collider collider = pickupGameObject.GetComponent<Collider>();
            collider.enabled = false;

            if (parsedItem.IsWeapon())
                pickupGameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            pickupGameObject.AddComponent<ItemSpiningComponent>();

            Handler.Indicators.Add(pickupGameObject, callingItemSpawnPointObject);

            NetworkServer.Spawn(pickupGameObject);
        }

        public static void SpawnDummyIndicator(Vector3 posistion, Vector3 scale, RoleType type, GameObject callingPlayerSpawnPointObject)
        {
            GameObject dummyObject = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));

            dummyObject.transform.localScale = scale;
            dummyObject.transform.position = posistion;

            QueryProcessor processor = dummyObject.GetComponent<QueryProcessor>();

            processor.NetworkPlayerId = QueryProcessor._idIterator++;
            processor._ipAddress = "127.0.0.WAN";

            CharacterClassManager ccm = dummyObject.GetComponent<CharacterClassManager>();
            ccm.CurClass = type;
            ccm.GodMode = true;

            string dummyNickname;

            switch (type)
            {
                case RoleType.NtfCadet:
                    dummyNickname = "MTF";
                    break;

                case RoleType.Scp93953:
                    dummyNickname = "SCP939";
                    break;

                default:
                    dummyNickname = type.ToString();
                    break;
            }

            NicknameSync nicknameSync = dummyObject.GetComponent<NicknameSync>();
            nicknameSync.Network_myNickSync = $"PLAYER SPAWNPOINT";
            nicknameSync.CustomPlayerInfo = $"{dummyNickname}\nSPAWN POINT";
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            NetworkServer.Spawn(dummyObject);
            PlayerManager.players.Add(dummyObject);
            Handler.Indicators.Add(dummyObject, callingPlayerSpawnPointObject);

            ReferenceHub rh = dummyObject.GetComponent<ReferenceHub>();
            Timing.CallDelayed(0.5f, () =>
            {
                dummyObject.AddComponent<DummySpiningComponent>().Hub = rh;
                rh.playerMovementSync.OverridePosition(posistion, 0f, true);
            });
        }
    }
}
