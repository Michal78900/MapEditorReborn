namespace MapEditorReborn.Commands
{
    using System;
    using System.Diagnostics;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Command used for opening folder in which maps are stored.
    /// </summary>
    public class OpenDirectory : ICommand
    {
        /// <inheritdoc/>
        public string Command => "opendirectory";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "od", "openfolder" };

        /// <inheritdoc/>
        public string Description => "Opens the MapEditorParent directory.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            Process.Start(MapEditorReborn.PluginDir);

            response = "Directory has been opened successfully!";
            return true;
        }
    }
}
