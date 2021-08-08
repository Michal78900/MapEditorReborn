namespace MapEditorReborn.Commands
{
    using System;
    using API;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using Mirror;
    using UnityEngine;

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
                            Handler.SpawnDummyIndicator(gameObject.transform.position, gameObject.tag.ConvertToRoleType(), gameObject);

                            break;
                        }

                    case "ItemSpawnPointObject(Clone)":
                        {
                            ItemSpawnPointComponent itemSpawnPointComponent = gameObject.GetComponent<ItemSpawnPointComponent>();

                            Handler.SpawnPickupIndicator(gameObject.transform.position, gameObject.transform.rotation, itemSpawnPointComponent.ItemName, gameObject);

                            break;
                        }

                    case "RagdollSpawnPointObject(Clone)":
                        {
                            Handler.SpawnDummyIndicator(gameObject.transform.position, gameObject.GetComponent<RagdollObjectComponent>().RagdollRoleType, gameObject);

                            break;
                        }
                }
            }

            response = "Indicators have been shown!";
            return true;
        }
    }
}
