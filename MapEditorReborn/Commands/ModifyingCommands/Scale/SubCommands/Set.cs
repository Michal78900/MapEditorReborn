﻿// -----------------------------------------------------------------------
// <copyright file="Set.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.ModifyingCommands.Scale.SubCommands
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
    /// Modifies object's scale by setting it to a certain value.
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
            if (!sender.CheckPermission("mpr.scale"))
            {
                response = "У вас недостаточно прав на выполнения этой команды.";
                return false;
            }

            Player player = Player.Get(sender);
            if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
            {
                if (!ToolGunHandler.TryGetMapObject(player, out mapObject))
                {
                    response = "Вы не выбрали объект!";
                    return false;
                }

                ToolGunHandler.SelectObject(player, mapObject);
            }

            if (!mapObject.IsScalable)
            {
                response = "Вы не можете изменять этот объект!";
                return false;
            }

            if (arguments.Count >= 3 && TryGetVector(arguments.At(0), arguments.At(1), arguments.At(2), out Vector3 newScale))
            {
                ChangingObjectScaleEventArgs ev = new(player, mapObject, newScale);
                Events.Handlers.MapEditorObject.OnChangingObjectScale(ev);

                if (!ev.IsAllowed)
                {
                    response = ev.Response;
                    return true;
                }

                mapObject.Scale = ev.Scale;
                player.ShowGameObjectHint(mapObject);
                mapObject.UpdateIndicator();

                response = ev.Scale.ToString("F3");
                return true;
            }

            response = "Введены неправильные значения.";
            return false;
        }
    }
}
