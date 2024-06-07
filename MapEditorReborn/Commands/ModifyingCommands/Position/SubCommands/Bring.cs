// -----------------------------------------------------------------------
// <copyright file="Bring.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.ModifyingCommands.Position.SubCommands
{
    using System;
    using API.Extensions;
    using API.Features.Objects;
    using CommandSystem;
    using Events.EventArgs;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using UnityEngine;
    using static API.API;

    /// <summary>
    /// Modifies object's position by setting it to the sender's current position.
    /// </summary>
    public class Bring : ICommand
    {
        /// <inheritdoc/>
        public string Command => "bring";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => string.Empty;

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mpr.position"))
            {
                response = "You don't have permission to execute this command. Required permission: mpr.position";
                return false;
            }

            Player player = Player.Get(sender);
            if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
            {
                if (!ToolGunHandler.TryGetMapObject(player, out mapObject))
                {
                    response = "You haven't selected any object!";
                    return false;
                }

                ToolGunHandler.SelectObject(player, mapObject);
            }

            if (mapObject is RoomLightObject)
            {
                response = "You can't modify this object's position!";
                return false;
            }

            Vector3 newPosition = player.Position;

            if (mapObject.name.Contains("Door"))
                newPosition += Vector3.down * 1.33f;

            BringingObjectEventArgs bringingEv = new(player, mapObject, newPosition);
            Events.Handlers.MapEditorObject.OnBringingObject(bringingEv);

            if (!bringingEv.IsAllowed)
            {
                response = bringingEv.Response;
                return true;
            }

            newPosition = bringingEv.Position;

            ChangingObjectPositionEventArgs positionEv = new(player, mapObject, newPosition);
            Events.Handlers.MapEditorObject.OnChangingObjectPosition(positionEv);

            if (!positionEv.IsAllowed)
            {
                response = positionEv.Response;
                return true;
            }

            mapObject.Position = positionEv.Position;
            mapObject.UpdateIndicator();
            player.ShowGameObjectHint(mapObject);

            response = positionEv.Position.ToString("F3");
            return true;
        }
    }
}
