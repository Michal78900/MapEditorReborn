namespace MapEditorReborn.Commands
{
    using System;
    using System.IO;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Command used for listing all saved maps.
    /// </summary>
    public class List : ICommand
    {
        /// <inheritdoc/>
        public string Command => "list";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "li" };

        /// <inheritdoc/>
        public string Description => "Shows the list of all available maps.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            response = "\n\n<color=green><b>List of maps:</b></color>\n";

            foreach (string filePath in Directory.GetFiles(MapEditorReborn.MapsDir))
            {
                response += $"- <color=yellow>{Path.GetFileNameWithoutExtension(filePath)}</color>\n";
            }

            response += "\n<color=orange><b>List of schematics:</b></color>\n";

            foreach (string filePath in Directory.GetFiles(MapEditorReborn.SchematicsDir))
            {
                response += $"- <color=yellow>{Path.GetFileNameWithoutExtension(filePath)}</color>\n";
            }

            return true;
        }
    }
}
