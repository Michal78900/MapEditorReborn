// -----------------------------------------------------------------------
// <copyright file="Grab.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.ModifyingCommands.Position.SubCommands
{
    using System;
    using System.Collections.Generic;
    using API.Extensions;
    using API.Features.Objects;
    using CommandSystem;
    using Events.EventArgs;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using MEC;
    using UnityEngine;
    using static API.API;

    /// <summary>
    /// Grabs a specific <see cref="MapEditorObject"/>.
    /// </summary>
    public class Grab : ICommand
    {
        /// <inheritdoc/>
        public string Command => "grab";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Grabs an object.";

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
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

            if (GrabbingPlayers.ContainsKey(player))
            {
                ReleasingObjectEventArgs releasingEv = new(player, mapObject);
                Events.Handlers.MapEditorObject.OnReleasingObject(releasingEv);

                if (!releasingEv.IsAllowed)
                {
                    response = releasingEv.Response;
                    return true;
                }

                Timing.KillCoroutines(GrabbingPlayers[player]);
                GrabbingPlayers.Remove(player);
                response = "Ungrabbed";
                return true;
            }

            GrabbingObjectEventArgs grabbingEv = new(player, mapObject);
            Events.Handlers.MapEditorObject.OnGrabbingObject(grabbingEv);

            if (!grabbingEv.IsAllowed)
            {
                response = grabbingEv.Response;
                return true;
            }

            GrabbingPlayers.Add(player, Timing.RunCoroutine(GrabbingCoroutine(player, grabbingEv.Object)));

            response = "Grabbed";
            return true;
        }

        private IEnumerator<float> GrabbingCoroutine(Player player, MapEditorObject mapObject)
        {
            Vector3 position = player.CameraTransform.position;
            float multiplier = Vector3.Distance(position, mapObject.Position);
            Vector3 prevPos = position + (player.CameraTransform.forward * multiplier);
            int i = 0;

            while (!RoundSummary.singleton._roundEnded)
            {
                yield return Timing.WaitForOneFrame;

                if (mapObject == null && !player.TryGetSessionVariable(SelectedObjectSessionVarName, out mapObject))
                    break;

                Vector3 newPos = mapObject.Position = player.CameraTransform.position + (player.CameraTransform.forward * multiplier);

                i++;
                if (i == 60)
                {
                    i = 0;
                    player.ShowGameObjectHint(mapObject);
                }

                if (prevPos == newPos)
                    continue;

                prevPos = newPos;

                ChangingObjectPositionEventArgs ev = new(player, mapObject, prevPos);
                Events.Handlers.MapEditorObject.OnChangingObjectPosition(ev);

                if (!ev.IsAllowed)
                    break;

                mapObject.Position = prevPos;
                mapObject.UpdateIndicator();
            }

            GrabbingPlayers.Remove(player);
        }

        /// <summary>
        /// The <see cref="Dictionary{TKey, TValue}"/> which contains all <see cref="Player"/> and <see cref="CoroutineHandle"/> pairs.
        /// </summary>
        private static readonly Dictionary<Player, CoroutineHandle> GrabbingPlayers = new();
    }
}
