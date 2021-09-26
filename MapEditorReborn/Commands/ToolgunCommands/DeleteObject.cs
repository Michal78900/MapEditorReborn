namespace MapEditorReborn.Commands
{
    using System;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using UnityEngine;

    /// <summary>
    /// Command used for deleting the objects.
    /// </summary>
    public class DeleteObject : ICommand
    {
        /// <inheritdoc/>
        public string Command => "delete";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "del", "remove", "rm" };

        /// <inheritdoc/>
        public string Description => "Deletes the object which you are looking at.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            // Player player = Player.Get(sender);
            Player player = Player.Get(sender as CommandSender);

            Vector3 forward = player.CameraTransform.forward;
            if (!Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
            {
                response = "You aren't looking at any Map Editor object!";
                return false;
            }

            MapEditorObject mapObject = hit.collider.GetComponentInParent<MapEditorObject>();

            if (mapObject != null)
            {
                IndicatorObjectComponent indicator = mapObject.GetComponent<IndicatorObjectComponent>();

                if (indicator != null)
                {
                    Handler.DeleteObject(player, indicator.AttachedMapEditorObject);

                    Handler.SpawnedObjects.Remove(indicator);
                    indicator.Destroy();

                    response = "You've successfully deleted the object through it's indicator!";
                    return true;
                }
            }

            if (mapObject == null || !Handler.SpawnedObjects.Contains(mapObject))
            {
                response = "You aren't looking at any Map Editor object!";
                return false;
            }

            Handler.DeleteObject(player, mapObject);

            response = "You've successfully deleted the object!";
            return true;
        }
    }
}
