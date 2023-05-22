// -----------------------------------------------------------------------
// <copyright file="FixMaps.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;

namespace MapEditorReborn.Commands.UtilityCommands;

using System;
using System.IO;
using System.Text;

internal class FixMaps : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "fixmaps";
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
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();

        foreach (var filePath in Directory.GetFiles(MapEditorReborn.MapsDir))
        {
            StringBuilder stringBuilder = new(File.ReadAllText(filePath));
            stringBuilder.Replace("shooting_target_objects", "shooting_targets");
            stringBuilder.Replace("primitive_objects", "primitives");
            stringBuilder.Replace("light_source_objects", "light_sources");
            stringBuilder.Replace("teleport_objects", "teleports");
            stringBuilder.Replace("schematic_objects", "schematics");

            File.WriteAllText(filePath, stringBuilder.ToString());
        }

        stopWatch.Stop();

        response = $"Fixed all of the maps in {stopWatch.ElapsedMilliseconds} ms! ({stopWatch.ElapsedTicks} ticks)";
        return true;
    }
}