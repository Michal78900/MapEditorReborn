﻿// -----------------------------------------------------------------------
// <copyright file="Rotate.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.ModifyingCommands.Rotation.SubCommands;

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
/// Rotates a specific <see cref="MapEditorObject"/>.
/// </summary>
public class Rotate : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "rotate";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = Array.Empty<string>();

    /// <inheritdoc/>
    public string Description
    {
        get => string.Empty;
    }

    /// <inheritdoc/>
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // if (!sender.CheckPermission("mpr.rotation"))
        // {
        //     response = $"You don't have permission to execute this command. Required permission: mpr.rotation";
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

        if (RotatingPlayers.ContainsKey(player))
        {
            Timing.KillCoroutines(RotatingPlayers[player]);
            RotatingPlayers.Remove(player);
            response = "Ungrabbed";
            return true;
        }

        RotatingPlayers.Add(player, Timing.RunCoroutine(RotatingCoroutine(player, mapObject)));

        response = "Grabbed";
        return true;
    }

    private IEnumerator<float> RotatingCoroutine(MERPlayer player, MapEditorObject mapObject)
    {
        Vector3 playerStartPos = player.Position;
        var i = 0;

        while (!RoundSummary.singleton._roundEnded)
        {
            yield return Timing.WaitForOneFrame;

            if (mapObject == null && !player.TryGetSessionVariable(SelectedObjectSessionVarName, out mapObject))
                break;

            i++;
            if (i == 60)
            {
                i = 0;
                player.ShowGameObjectHint(mapObject);
            }

            if (playerStartPos == player.Position)
                continue;

            ChangingObjectRotationEventArgs ev = new(player, mapObject, Round((playerStartPos - player.Position) * 10f), true);
            Events.Handlers.MapEditorObject.OnChangingObjectRotation(ev);

            if (!ev.IsAllowed)
                break;

            mapObject.transform.eulerAngles += ev.Rotation;
            mapObject.UpdateObject();
            mapObject.UpdateIndicator();
            player.Position = playerStartPos;
        }

        RotatingPlayers.Remove(player);
    }

    private Vector3 Round(Vector3 vector)
    {
        vector.x = Mathf.Round(vector.x);
        vector.y = Mathf.Round(vector.y);
        vector.z = Mathf.Round(vector.z);

        return vector;
    }

    /// <summary>
    /// The <see cref="Dictionary{TKey, TValue}"/> which contains all <see cref="Player"/> and <see cref="CoroutineHandle"/> pairs.
    /// </summary>
    public static Dictionary<Player, CoroutineHandle> RotatingPlayers = new();
}