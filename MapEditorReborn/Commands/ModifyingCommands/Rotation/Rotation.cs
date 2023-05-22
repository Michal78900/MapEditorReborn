// -----------------------------------------------------------------------
// <copyright file="Rotation.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using MapEditorReborn.Factories;
using PluginAPI.Core;

namespace MapEditorReborn.Commands.ModifyingCommands.Rotation;

using System;
using API.Features.Objects;
using SubCommands;
using static API.API;

/// <summary>
/// Command used for modifing object's rotation.
/// </summary>
public class Rotation : ParentCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Rotation"/> class.
    /// </summary>
    public Rotation() => LoadGeneratedCommands();

    /// <inheritdoc/>
    public override string Command
    {
        get => "rotation";
    }

    /// <inheritdoc/>
    public override string[] Aliases { get; } = { "rot" };

    /// <inheritdoc/>
    public override string Description
    {
        get => "Modifies object's rotation.";
    }

    /// <inheritdoc/>
    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new Add());
        RegisterCommand(new Set());
        RegisterCommand(new Rotate());
    }

    /// <inheritdoc/>
    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get<MERPlayer>(sender);
        if (player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null)
        {
            response = $"Object current rotation: {mapObject.RelativeRotation}\n";
            return true;
        }

        response = "\nUsage:";
        response += "\nmp rotation set (x) (y) (z)";
        response += "\nmp rotation add (x) (y) (z)";
        return false;
    }
}