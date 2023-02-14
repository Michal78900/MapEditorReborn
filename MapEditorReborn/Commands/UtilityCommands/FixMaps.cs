// -----------------------------------------------------------------------
// <copyright file="FixMaps.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using CommandSystem;

    internal class FixMaps : ICommand
    {
        /// <inheritdoc/>
        public string Command => "fixmaps";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => string.Empty;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            foreach (string filePath in Directory.GetFiles(MapEditorReborn.MapsDir))
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
}
