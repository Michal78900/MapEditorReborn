namespace MapEditorReborn.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using API;
    using CommandSystem;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using MEC;
    using Mirror;
    using RemoteAdmin;
    using UnityEngine;

    using Object = UnityEngine.Object;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ShowIndicators : ICommand
    {
        /// <inheritdoc/>
        public string Command => "showindicators";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "si" };

        /// <inheritdoc/>
        public string Description => "";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Handler.Indicators.Count != 0)
            {
                foreach (GameObject indicator in Handler.Indicators)
                {
                    NetworkServer.Destroy(indicator.gameObject);
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
                            // Orginal code found in AdminTools
                            GameObject dummyGameObject = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));
                            CharacterClassManager ccm = dummyGameObject.GetComponent<CharacterClassManager>();
                            ccm.CurClass = gameObject.tag.ConvertToRoleType();
                            ccm.GodMode = true;
                            dummyGameObject.GetComponent<NicknameSync>().Network_myNickSync = $"{gameObject.tag.ConvertToRoleType()} spawn point";
                            dummyGameObject.GetComponent<QueryProcessor>().PlayerId = 9999;
                            dummyGameObject.GetComponent<QueryProcessor>().NetworkPlayerId = 9999;
                            dummyGameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                            dummyGameObject.transform.position = gameObject.transform.position + (Vector3.up * 0.1f);
                            dummyGameObject.transform.rotation = gameObject.transform.rotation;

                            // dummyGameObject.AddComponent<SpiningComponent>();

                            Handler.Indicators.Add(dummyGameObject);

                            NetworkServer.Spawn(dummyGameObject);

                            break;
                        }

                    case "ItemSpawnPointObject(Clone)":
                        {
                            ItemSpawnPointComponent itemSpawnPointComponent = gameObject.GetComponent<ItemSpawnPointComponent>();
                            GameObject pickupGameObject = null;

                            ItemType parsedItem = ItemType.None;

                            if (CustomItem.TryGet(itemSpawnPointComponent.ItemName, out CustomItem custom))
                            {
                                parsedItem = custom.Type;
                            }
                            else
                            {
                                parsedItem = (ItemType)Enum.Parse(typeof(ItemType), itemSpawnPointComponent.ItemName, true);
                            }

                            pickupGameObject = Item.Spawn(parsedItem, parsedItem.GetDefaultDurability(), gameObject.transform.position + (Vector3.up * 0.1f), gameObject.transform.rotation).gameObject;

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

                            pickupGameObject.AddComponent<SpiningComponent>();

                            Handler.Indicators.Add(pickupGameObject);

                            NetworkServer.Spawn(pickupGameObject);

                            break;
                        }
                }
            }

            response = "Indicators have been shown!";
            return true;
        }
    }
}
