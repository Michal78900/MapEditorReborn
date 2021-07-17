namespace MapEditorReborn.Commands
{
    using System;
    using CommandSystem;

    public class Save : ICommand
    {
        /// <inheritdoc/>
        public string Command => "save";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "s" };

        /// <inheritdoc/>
        public string Description => "Saves the map.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "You need to provide a map name!";
                return false;
            }

            Handler.SaveMap(arguments.At(0));

            response = $"MapSchematic named {arguments.At(0)} has been successfully saved!";
            return true;
        }
    }
}
