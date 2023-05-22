﻿// -----------------------------------------------------------------------
// <copyright file="OpenDirectory.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;

namespace MapEditorReborn.Commands.UtilityCommands;

using System;
using System.Diagnostics;

/// <summary>
/// Command used for opening folder in which maps are stored.
/// </summary>
public class OpenDirectory : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "opendirectory";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = { "od", "openfolder" };

    /// <inheritdoc/>
    public string Description
    {
        get => "Opens the MapEditorParent directory.";
    }

    /// <inheritdoc/>
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // if (!sender.CheckPermission($"mpr.{Command}"))
        // {
        //     response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
        //     return false;
        // }

        Process.Start(MapEditorReborn.PluginDir);

        response = "Directory has been opened successfully!";
        return true;
    }
}