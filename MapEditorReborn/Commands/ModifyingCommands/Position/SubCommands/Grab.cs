// -----------------------------------------------------------------------
// <copyright file="Grab.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.ModifyingCommands.Position.SubCommands;

using System;
using System.Collections.Generic;
using API.Extensions;
using API.Features.Objects;
using Events.EventArgs;
using Events.Handlers.Internal;
using MEC;
using UnityEngine;
using static API.API;

/// <summary>
/// Grabs a specific <see cref="MapEditorObject"/>.
/// </summary>
public class Grab : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "grab";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = Array.Empty<string>();

    /// <inheritdoc/>
    public string Description
    {
        get => "Grabs an object.";
    }

    /// <inheritdoc/>
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // if (!sender.CheckPermission($"mpr.{Command}"))
        // {
        //     response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
        //     return false;
        // }

        var player = Player.Get<MERPlayer>(sender);
        if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
        {
            if (!ToolGunHandler.TryGetMapObject(player, out mapObject))
            {
                response = "You haven't selected any object!";
                return false;
            }

            ToolGunHandler.SelectObject(player, mapObject);
        }

        if (grabbingPlayers.ContainsKey(player))
        {
            ReleasingObjectEventArgs releasingEv = new(player, mapObject, true);
            Events.Handlers.MapEditorObject.OnReleasingObject(releasingEv);

            if (!releasingEv.IsAllowed)
            {
                response = releasingEv.Response;
                return true;
            }

            Timing.KillCoroutines(grabbingPlayers[player]);
            grabbingPlayers.Remove(player);
            response = "Ungrabbed";
            return true;
        }

        GrabbingObjectEventArgs grabbingEv = new(player, mapObject, true);
        Events.Handlers.MapEditorObject.OnGrabbingObject(grabbingEv);

        if (!grabbingEv.IsAllowed)
        {
            response = grabbingEv.Response;
            return true;
        }

        grabbingPlayers.Add(player, Timing.RunCoroutine(GrabbingCoroutine(player, grabbingEv.Object)));

        response = "Grabbed";
        return true;
    }

    private IEnumerator<float> GrabbingCoroutine(MERPlayer player, MapEditorObject mapObject)
    {
        Vector3 position = player.Camera.position;
        var multiplier = Vector3.Distance(position, mapObject.transform.position);
        var prevPos = position + (player.Camera.forward * multiplier);
        var i = 0;

        while (!RoundSummary.singleton._roundEnded)
        {
            yield return Timing.WaitForOneFrame;

            if (mapObject == null && !player.TryGetSessionVariable(SelectedObjectSessionVarName, out mapObject))
                break;

            var newPos = mapObject.transform.position = player.Camera.position + (player.Camera.forward * multiplier);

            i++;
            if (i == 60)
            {
                i = 0;
                player.ShowGameObjectHint(mapObject);
            }

            if (prevPos == newPos)
                continue;

            prevPos = newPos;

            ChangingObjectPositionEventArgs ev = new(player, mapObject, prevPos, true);
            Events.Handlers.MapEditorObject.OnChangingObjectPosition(ev);

            if (!ev.IsAllowed)
                break;

            mapObject.transform.position = prevPos;
            mapObject.UpdateObject();
            mapObject.UpdateIndicator();
        }

        grabbingPlayers.Remove(player);
    }

    /// <summary>
    /// The <see cref="Dictionary{TKey, TValue}"/> which contains all <see cref="Player"/> and <see cref="CoroutineHandle"/> pairs.
    /// </summary>
    private static Dictionary<Player, CoroutineHandle> grabbingPlayers = new();
}