﻿// -----------------------------------------------------------------------
// <copyright file="Add.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.Position.SubCommands
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
    /// Modifies object's position by adding a certain value to the current one.
    /// </summary>
    public class Add : ICommand
    {
        /// <inheritdoc/>
        public string Command => "add";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => string.Empty;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mpr.position"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.position";
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
                else
                {
                    ToolGunHandler.SelectObject(player, mapObject);
                }
            }

            if (mapObject is RoomLightObject)
            {
                response = "You can't modify this object's position!";
                return false;
            }

            if (arguments.Count >= 3 && TryGetVector(arguments.At(0), arguments.At(1), arguments.At(2), out Vector3 newPosition))
            {
                ChangingObjectPositionEventArgs ev = new(player, mapObject, newPosition, true);
                Events.Handlers.MapEditorObject.OnChangingObjectPosition(ev);

                if (!ev.IsAllowed)
                {
                    response = ev.Response;
                    return true;
                }

                mapObject.transform.position += ev.Position;

                mapObject.UpdateObject();
                mapObject.UpdateIndicator();
                player.ShowGameObjectHint(mapObject);

                response = ev.Position.ToString("F3");
                return true;
            }

            response = "Invalid values.";
            return false;
        }
    }
}
