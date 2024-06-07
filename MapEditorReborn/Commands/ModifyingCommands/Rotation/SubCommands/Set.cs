// -----------------------------------------------------------------------
// <copyright file="Set.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.ModifyingCommands.Rotation.SubCommands
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
    /// Modifies object's rotation by setting it to a certain value.
    /// </summary>
    public class Set : ICommand
    {
        /// <inheritdoc/>
        public string Command => "set";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => string.Empty;

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mpr.rotation"))
            {
                response = "You don't have permission to execute this command. Required permission: mpr.rotation";
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

            if (!mapObject.IsRotatable)
            {
                response = "You can't modify this object's rotation!";
                return false;
            }

            if (arguments.Count >= 3 && TryGetVector(arguments.At(0), arguments.At(1), arguments.At(2), out Vector3 newRotation))
            {
                ChangingObjectRotationEventArgs ev = new(player, mapObject, newRotation);
                Events.Handlers.MapEditorObject.OnChangingObjectRotation(ev);

                if (!ev.IsAllowed)
                {
                    response = ev.Response;
                    return true;
                }

                mapObject.Rotation = GetRelativeRotation(ev.Rotation, mapObject.CurrentRoom);
                player.ShowGameObjectHint(mapObject);

                response = ev.Rotation.ToString("F3");
                return true;
            }

            response = "Invalid values.";
            return false;
        }
    }
}
