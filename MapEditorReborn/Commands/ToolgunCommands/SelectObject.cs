namespace MapEditorReborn.Commands
{
    using System;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using UnityEngine;

    /// <summary>
    /// Command used for selecting the objects.
    /// </summary>
    public class SelectObject : ICommand
    {
        /// <inheritdoc/>
        public string Command => "select";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "sel", "choose" };

        /// <inheritdoc/>
        public string Description => "Selects the object which you are looking at.";

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

            IndicatorObjectComponent indicator = mapObject?.GetComponent<IndicatorObjectComponent>();

            if (indicator != null)
            {
                mapObject = indicator.AttachedMapEditorObject;
            }

            if (mapObject == null || !Handler.SpawnedObjects.Contains(mapObject))
            {
                Handler.SelectObject(player, mapObject);
                response = "You've unselected the object!";
                return true;
            }

            Handler.SelectObject(player, mapObject);

            response = "You've successfully selected the object!";
            return true;
        }
    }
}
