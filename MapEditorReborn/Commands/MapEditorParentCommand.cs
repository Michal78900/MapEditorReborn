// -----------------------------------------------------------------------
// <copyright file="MapEditorParentCommand.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;

namespace MapEditorReborn.Commands;

using System;
using CommandSystem;
using ModifyingCommands;
using ModifyingCommands.Position;
using ModifyingCommands.Rotation;
using ModifyingCommands.Scale;
using ToolgunCommands;
using UtilityCommands;

/// <summary>
/// The base parent command.
/// </summary>
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class MapEditorParentCommand : ParentCommand
{
    /// <inheritdoc/>
    public MapEditorParentCommand() => LoadGeneratedCommands();

    /// <inheritdoc/>
    public override string Command
    {
        get => "mapeditor";
    }

    /// <inheritdoc/>
    public override string[] Aliases { get; } = { "mp" };

    /// <inheritdoc/>
    public override string Description
    {
        get => "The MapEditorReborn parent command";
    }

    /// <inheritdoc/>
    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new CreateObject());
        RegisterCommand(new DeleteObject());
        RegisterCommand(new CopyObject());
        RegisterCommand(new SelectObject());

        RegisterCommand(new ToolGun());
        RegisterCommand(new GravityGun());
        RegisterCommand(new Save());
        RegisterCommand(new Load());
        RegisterCommand(new Unload());
        RegisterCommand(new ShowIndicators());
        RegisterCommand(new List());
        RegisterCommand(new OpenDirectory());
        RegisterCommand(new FixMaps());
        RegisterCommand(new Merge());

        RegisterCommand(new Properties());
        RegisterCommand(new Modify());
        RegisterCommand(new SetRoomType());
        RegisterCommand(new Position());
        RegisterCommand(new Rotation());
        RegisterCommand(new Scale());
    }

    /// <inheritdoc/>
    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = AllCommands
            //.Where(command => sender.CheckPermission($"mpr.{command.Command}"))
            .Aggregate("\nPlease enter a valid subcommand:", (current, command) => current + $"\n\n<color=yellow><b>- {command.Command} ({string.Join(", ", command.Aliases)})</b></color>\n<color=white>{command.Description}</color>");

        return false;
    }
}