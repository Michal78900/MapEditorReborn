﻿// -----------------------------------------------------------------------
// <copyright file="Position.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.ModifyingCommands.Position;

using System;
using API.Features.Objects;
using SubCommands;
using static API.API;

/// <summary>
/// Command used for modifing object's position.
/// </summary>
public class Position : ParentCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Position"/> class.
    /// </summary>
    public Position() => LoadGeneratedCommands();

    /// <inheritdoc/>
    public override string Command
    {
        get => "position";
    }

    /// <inheritdoc/>
    public override string[] Aliases { get; } = { "pos" };

    /// <inheritdoc/>
    public override string Description
    {
        get => "Modifies object's posistion.";
    }

    /// <inheritdoc/>
    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new Add());
        RegisterCommand(new Set());
        RegisterCommand(new Bring());
        RegisterCommand(new Grab());
    }

    /// <inheritdoc/>
    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get<MERPlayer>(sender);
        if (player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null)
        {
            response = $"Object current position: {mapObject.RelativePosition}\n";
            return true;
        }

        response = "\nUsage:\n";
        response += "mp position set (x) (y) (z)\n";
        response += "mp position add (x) (y) (z)\n";
        response += "mp position bring\n";
        response += "mp position grab";

        return false;
    }
}