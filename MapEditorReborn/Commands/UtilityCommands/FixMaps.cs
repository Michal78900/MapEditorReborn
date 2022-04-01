namespace MapEditorReborn.Commands
{
    using System;
    using System.IO;
    using System.Text;
    using CommandSystem;

    internal class FixMaps : ICommand
    {
        /// <inheritdoc/>
        public string Command => "fixmaps";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => string.Empty;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            foreach (string filePath in Directory.GetFiles(MapEditorReborn.MapsDir))
            {
                StringBuilder stringBuilder = new StringBuilder(File.ReadAllText(filePath));
                stringBuilder.Replace("shooting_target_objects", "shooting_targets");
                stringBuilder.Replace("primitive_objects", "primitives");
                stringBuilder.Replace("light_source_objects", "light_sources");
                stringBuilder.Replace("teleport_objects", "teleports");
                stringBuilder.Replace("schematic_objects", "schematics");
                stringBuilder.Replace("entrance_teleporter_", string.Empty);

                File.WriteAllText(filePath, stringBuilder.ToString());
            }

            stopWatch.Stop();

            response = $"Fixed all of the maps in {stopWatch.ElapsedMilliseconds} ms! ({stopWatch.ElapsedTicks} ticks)";
            return true;
        }
    }
}
