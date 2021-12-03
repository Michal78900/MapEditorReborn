namespace MapEditorReborn.Commands
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Command used for unloading <see cref="MapSchematic"/>.
    /// </summary>
    public class UnLoad : ICommand
    {
        /// <inheritdoc/>
        public string Command => "unload";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "unl" };

        /// <inheritdoc/>
        public string Description => "Unloads currently loaded map.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            Methods.CurrentLoadedMap = null;
            response = "Map has been successfully unloaded!";
            return true;
        }
    }
}
