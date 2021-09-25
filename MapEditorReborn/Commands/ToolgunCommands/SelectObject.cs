namespace MapEditorReborn.Commands
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    public class SelectObject : ICommand
    {
        /// <inheritdoc/>
        public string Command => "select";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "sel, choose" };

        /// <inheritdoc/>
        public string Description => "Selects the object which you are looking at.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            response = "";
            return true;
        }
    }
}
